using System.Collections.Concurrent;
using System.Threading.Tasks.Sources;
using System.Xml;

using UnitsNet;
using UnitsNet.Units;

using VectSharp;
using VectSharp.SVG;

namespace Papau.Levelmeter.LevelMeter;

public class SvgScalePainter
{
    private double _topTextOverflow = 0;
    private double _bottomTextOverflow = 0;

    public Task PaintAsync(GraduationMark[] graduationMarks, Stream outputStream, CancellationToken cancellationToken)
    {
        _topTextOverflow = 0;

        if (!graduationMarks.Any())
            return Task.CompletedTask;

        var svgPage = new Page(1, 1)
        {
            Background = Colours.White
        };

        var unit = LengthUnit.Millimeter;
        foreach (var mark in graduationMarks.Reverse())
        {
            DrawGraduationMark(svgPage, mark);
            unit = mark.Length.Unit;
        }
        
        var padding = Length.FromMillimeters(5).ToUnit(unit).Value;
        PaintCuttingRectangle(padding, graduationMarks, svgPage);

        var svgDoc = svgPage.SaveAsSVG(SVGContextInterpreter.TextOptions.ConvertIntoPaths);
        SetSvgDimensionsInXml(svgPage, unit, svgDoc);
        svgDoc.Save(outputStream);

        return Task.CompletedTask;
    }

    private void PaintCuttingRectangle(double padding, GraduationMark[] graduationMarks, Page svgPage)
    {
        svgPage.Crop();
        var page = svgPage.Graphics.GetBounds();

        var topMarker = graduationMarks.MinBy(m => m.Position.Y)!;
        var topYBound = (_topTextOverflow + (topMarker.Height.Value / 2)) * -1;
        var bottomMarker = graduationMarks.MaxBy(m => m.Position.Y)!;
        var bottomYBound = _bottomTextOverflow;

        // top left corner moved by padding to top and left
        var pos = new Point(0 - padding, topYBound - padding);

        var size = new Size(page.Size.Width + 2 * padding, page.Size.Height + bottomYBound + 2 * padding);
        svgPage.Graphics.StrokeRectangle(pos, size, Colours.Red, 0.1);

        svgPage.Crop();
    }

    private static void SetSvgDimensionsInXml(Page page, LengthUnit unit, XmlDocument svgDoc)
    {
        var size = page.Graphics.GetBounds();

        var widthAttribute = svgDoc.CreateAttribute("width");
        widthAttribute.Value = $"{size.Size.Width}{Length.GetAbbreviation(unit)}";
        var heightAttribute = svgDoc.CreateAttribute("height");
        heightAttribute.Value = $"{size.Size.Height}{Length.GetAbbreviation(unit)}";
        var rootAttributes = svgDoc.GetElementsByTagName("svg")[0]!.Attributes!;
        rootAttributes.Append(widthAttribute);
        rootAttributes.Append(heightAttribute);
    }

    private readonly static ConcurrentDictionary<string, FontFamily?> FontCache = [];

    private void DrawGraduationMark(Page scale, GraduationMark mark)
    {
        // assume middle of the marker marks the spot. This helps avoiding differences in spacing due to different marker heights.
        var markerPos = new Point(mark.Position.X.Value, mark.Position.Y.Value - mark.Height.Value / 2);
        var markerSize = new Size(mark.Length.Value, mark.Height.Value);
        scale.Graphics.FillRectangle(markerPos, markerSize, Colours.Black, "mark" + mark.Volume.ToString());

        if (!string.IsNullOrWhiteSpace(mark.Text))
        {
            var font = GetFont(mark);
            var textSize = font.MeasureText(mark.Text);
            var textPosition = GetTextPosition(mark, markerPos, textSize);

            _topTextOverflow = Math.Max(_topTextOverflow, markerPos.Y - textPosition.Y + (textSize.Height / 2));
            _bottomTextOverflow = Math.Min(_bottomTextOverflow, markerPos.Y - textPosition.Y - (textSize.Height / 2));

            scale.Graphics.FillText(textPosition, mark.Text, font, Colours.Black, TextBaselines.Middle, "text" + mark.Volume.ToString());
        }
    }

    private static Point GetTextPosition(GraduationMark mark, Point markerPos, Size textSize)
    {
        var fontX = mark.Position.X.Value + mark.Length.Value + mark.Font.OffsetX;
        // align in the middle of the marker
        var fontY = markerPos.Y + mark.Height.Value / 2 + mark.Font.OffsetY;

        // change text alignment
        fontX = ApplyAlignment(fontX, textSize, mark.Font.TextAlignment);

        return new Point(fontX, fontY);
    }

    private static Font GetFont(GraduationMark mark)
    {
        var fontFamily = FontCache.GetOrAdd(mark.Font.Family, f =>
        {
            var result = FontFamily.ResolveFontFamily(f);

            if (result is null || result.TrueTypeFile is null)
                throw new InvalidOperationException($"Font '{mark.Font}' not found or not a valid font!");

            return result;
        });

        var font = new Font(fontFamily, mark.Font.Size);
        return font;
    }

    private static double ApplyAlignment(double xPos, Size textSize, GraduationMarkSettings.TextAlignment textAlignment)
    {
        return textAlignment switch
        {
            GraduationMarkSettings.TextAlignment.Center => xPos - (textSize.Width / 2),
            GraduationMarkSettings.TextAlignment.Right => xPos - textSize.Width,
            _ => xPos
        };
    }
}