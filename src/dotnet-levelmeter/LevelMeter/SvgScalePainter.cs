using System.Collections.Concurrent;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Threading.Tasks.Sources;
using System.Xml;

using Papau.Levelmeter.SvgHelper;

using SkiaSharp;

using UnitsNet;
using UnitsNet.Units;

namespace Papau.Levelmeter.LevelMeter;

public class SvgScalePainter
{
    public async Task PaintAsync(GraduationMark[] graduationMarks, Stream outputStream, CancellationToken cancellationToken)
    {
        if (!graduationMarks.Any())
            return;

        var svg = new SvgModel()
        {
            Background = Color.White
        };

        var unit = LengthUnit.Millimeter;
        foreach (var mark in graduationMarks.Reverse())
        {
            DrawGraduationMark(svg, mark);
            unit = mark.Length.Unit;
        }

        var padding = Length.FromMillimeters(5).ToUnit(unit).Value;
        svg.Padding = new SizeF((float)padding, (float)padding);

        await svg.SaveToStream(outputStream, unit, cancellationToken).ConfigureAwait(false);
    }

    private readonly static ConcurrentDictionary<string, SKTypeface> FontCache = [];

    private void DrawGraduationMark(SvgModel scale, GraduationMark mark)
    {
        // assume middle of the marker marks the spot. This helps avoiding differences in spacing due to different marker heights.
        var markerPos = new PointF((float)mark.Position.X.Value, (float)(mark.Position.Y.Value - mark.Height.Value / 2));

        scale.FillRectangle(markerPos, new SizeF((float)mark.Length.Value, (float)mark.Height.Value), Color.Black, $"mark_{mark.Volume.Value}");

        if (!string.IsNullOrWhiteSpace(mark.Text))
        {
            var font = GetFont(mark);
            var textPath = CreateTextPath(mark, font);
            var textSize = new SizeF(textPath.Bounds.Size.Width, textPath.Bounds.Size.Height);
            var textPosition = GetTextPosition(mark, markerPos, textSize, font);

            scale.FillPath(textPath, textPosition, Color.Black);

            // bounding box for debugging
            // scale.StrokeRectangle(textPosition, textSize, Color.Green, 0.05f);
        }
    }

    private static SKPath CreateTextPath(GraduationMark mark, SKFont font)
    {
        using var textPath = font.GetTextPath(mark.Text);

        // Get the current bounds of the path
        SKRect currentBounds = textPath.Bounds;

        // Calculate the translation to move the top-left corner to (0, 0)
        float translateX = -currentBounds.Left;
        float translateY = -currentBounds.Top;

        // Create a transformation matrix for cropping
        SKMatrix cropMatrix = SKMatrix.CreateTranslation(translateX, translateY);

        // Apply the transformation to the path
        var croppedPath = new SKPath();
        textPath.Transform(cropMatrix, croppedPath);
        return croppedPath;
    }

    private static PointF GetTextPosition(GraduationMark mark, PointF markerPos, SizeF textSize, SKFont font)
    {
        var fontX = mark.Position.X.Value + mark.Length.Value + mark.Font.OffsetX;

        // the position of the text is calculated by the top left corner of the path.
        // but we want to align the text by its baseline so we need to calculate the
        // offset between the top and the baseline
        var fontBaselineOffset = font.Metrics.StrikeoutPosition + font.Metrics.StrikeoutThickness / 2;
        if (!fontBaselineOffset.HasValue)
            fontBaselineOffset = (textSize.Height / 2) * -1; // fallback to half height of the path

        // align in the middle of the marker
        var fontY = markerPos.Y + mark.Height.Value / 2 + mark.Font.OffsetY + fontBaselineOffset;

        // change text alignment
        fontX = ApplyAlignment(fontX, textSize, mark.Font.TextAlignment);

        return new PointF((float)fontX, (float)fontY);
    }

    private static SKFont GetFont(GraduationMark mark)
    {
        var fontFamily = FontCache.GetOrAdd(mark.Font.Family, f => SKTypeface.FromFile(mark.Font.Family));

        return new SKFont
        {
            Typeface = fontFamily,
            Size = (float)mark.Font.Size,
            Edging = SKFontEdging.Antialias
        };
    }

    private static double ApplyAlignment(double xPos, SizeF textSize, GraduationMarkSettings.TextAlignment textAlignment)
    {
        return textAlignment switch
        {
            GraduationMarkSettings.TextAlignment.Center => xPos - (textSize.Width / 2),
            GraduationMarkSettings.TextAlignment.Right => xPos - textSize.Width,
            _ => xPos
        };
    }
}