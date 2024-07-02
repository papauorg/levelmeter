using UnitsNet;
using UnitsNet.Units;

using VectSharp;
using VectSharp.SVG;

namespace Papau.Levelmeter.LevelMeter;

public class SvgScalePainter
{
    public Task PaintAsync(GraduationMark[] graduationMarks, Stream outputStream, CancellationToken cancellationToken)
    {
        var svgPage = new Page(1, 1); // initial page size does not matter

        var unit = LengthUnit.Millimeter;
        foreach (var mark in graduationMarks.Reverse())
        {
            DrawGraduationMark(svgPage, mark);
            unit = mark.Length.Unit;
        }

        // draw border around scale
        svgPage.Crop();
        var scaleSize = svgPage.Graphics.GetBounds();

        // calculate padding
        var scalePadding = scaleSize.Size.Width / 100 * 30;

        svgPage.Graphics.Translate(scalePadding / -2, scalePadding * -1);
        svgPage.Graphics.StrokeRectangle(0, 0, scaleSize.Size.Width + scalePadding, scaleSize.Size.Height + scalePadding, Colours.Black, 0.01);
        svgPage.Crop();
        scaleSize = svgPage.Graphics.GetBounds();

        var svgDoc = svgPage.SaveAsSVG(SVGContextInterpreter.TextOptions.ConvertIntoPaths);
        var widthAttribute = svgDoc.CreateAttribute("width");
        widthAttribute.Value = $"{scaleSize.Size.Width}{Length.GetAbbreviation(unit)}";
        var heightAttribute = svgDoc.CreateAttribute("height");
        heightAttribute.Value = $"{scaleSize.Size.Height}{Length.GetAbbreviation(unit)}";
        var rootAttributes = svgDoc.GetElementsByTagName("svg")[0]!.Attributes!;
        rootAttributes.Append(widthAttribute);
        rootAttributes.Append(heightAttribute);

        svgDoc.Save(outputStream);

        return Task.CompletedTask;
    }

    private static void DrawGraduationMark(Page scale, GraduationMark mark)
    {
        // assume middle of the marker marks the spot. This helps avoiding differences in spacing due to different marker heights.
        var markerPos = new Point(mark.Position.X.Value, mark.Position.Y.Value - mark.Height.Value / 2);
        var markerSize = new Size(mark.Length.Value, mark.Height.Value);
        scale.Graphics.FillRectangle(markerPos, markerSize, Colours.Black, "mark" + mark.Volume.ToString());

        if (!string.IsNullOrWhiteSpace(mark.Text))
        {
            var fontFamily = FontFamily.ResolveFontFamily(mark.Font.Family);

            if (fontFamily is null || fontFamily.TrueTypeFile is null)
                throw new InvalidOperationException($"Font '{mark.Font}' not found or not a valid font!");

            var font = new Font(fontFamily, mark.Font.Size);
            var fontX = mark.Position.X.Value + mark.Length.Value + mark.Font.OffsetX;
            var fontY = markerPos.Y + mark.Height.Value / 2 + mark.Font.OffsetY; // align in the middle of the marker

            // change text alignment
            fontX = ApplyAlignment(fontX, font.MeasureText(mark.Text), mark.Font.TextAlignment);

            scale.Graphics.FillText(fontX, fontY, mark.Text, font, Colours.Black, TextBaselines.Middle, "text" + mark.Volume.ToString());
        }
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