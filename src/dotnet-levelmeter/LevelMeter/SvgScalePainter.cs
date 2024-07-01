
using UnitsNet;

using VectSharp;
using VectSharp.SVG;

public class SvgScalePainter
{

    public Task PaintAsync(GraduationMark[] graduationMarks, Stream outputStream, CancellationToken cancellationToken)
    {
        var svgPage = new Page(1, 1); // initial page size does not matter
       
        foreach(var mark in graduationMarks.Reverse())
            DrawGraduationMark(svgPage, mark);

        // draw border around scale
        svgPage.Crop();
        var scaleSize = svgPage.Graphics.GetBounds();

        // padding 15%
        var scalePadding = scaleSize.Size.Width / 100 * 15;

        svgPage.Graphics.Translate(scalePadding / -2, scalePadding / -2);
        svgPage.Graphics.StrokeRectangle(0, 0, scaleSize.Size.Width + scalePadding, scaleSize.Size.Height + scalePadding, Colours.Black, 0.01);
        svgPage.Crop();
        scaleSize = svgPage.Graphics.GetBounds();

        // assume mm units
        var svgDoc = svgPage.SaveAsSVG(SVGContextInterpreter.TextOptions.ConvertIntoPaths);
        var widthAttribute = svgDoc.CreateAttribute("width");
        widthAttribute.Value = $"{scaleSize.Size.Width}mm";
        var heightAttribute = svgDoc.CreateAttribute("height");
        heightAttribute.Value = $"{scaleSize.Size.Height}mm";
        var rootAttributes = svgDoc.GetElementsByTagName("svg")[0]!.Attributes!;
        rootAttributes.Append(widthAttribute);
        rootAttributes.Append(heightAttribute);

        svgDoc.Save(outputStream);

        return Task.CompletedTask;
    }

    private void DrawGraduationMark(Page scale, GraduationMark mark)
    {
        // assume middle of the marker marks the spot. This helps avoiding differences in spacing due to different marker heights.
        var markerPos = new Point(mark.Position.X.Millimeters, mark.Position.Y.Millimeters - (mark.Height.Millimeters / 2));
        var markerSize = new Size(mark.Length.Millimeters, mark.Height.Millimeters);
        scale.Graphics.FillRectangle(markerPos, markerSize, Colours.Black, "mark" + mark.Volume.ToString());
        
        // debug
        /*
        var path = new GraphicsPath();
        path.MoveTo(mark.Position.X.Millimeters, mark.Position.Y.Millimeters).LineTo(new Point(mark.Position.X.Millimeters + 10, mark.Position.Y.Millimeters));
        scale.Graphics.StrokePath(path, Colours.Red, 0.1f);
        */

        if (!string.IsNullOrWhiteSpace(mark.Text))
        {
            //var fontFamily = FontFamily.ResolveFontFamily(mark.Font);
            var fontFamily = FontFamily.ResolveFontFamily(FontFamily.StandardFontFamilies.Helvetica);

            if (fontFamily is null || fontFamily.TrueTypeFile is null)
                throw new InvalidOperationException($"Font '{mark.Font}' not found!");

            var font = new Font(fontFamily, mark.FontSize);

            var fontX = mark.Position.X + mark.Length + Length.FromMillimeters(Math.Max(2, mark.FontSize / 2));
            var fontY = markerPos.Y + (mark.Height.Millimeters / 2); // align in the middle of the marker

            scale.Graphics.FillText(fontX.Millimeters, fontY, mark.Text, font, Colours.Black, TextBaselines.Middle, "text" + mark.Volume.ToString());
        }
    }

}