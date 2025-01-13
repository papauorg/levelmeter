using System.CodeDom.Compiler;
using System.Drawing;
using System.Globalization;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using System.Xml.Linq;

using Microsoft.VisualBasic;

using SkiaSharp;

using UnitsNet;
using UnitsNet.Units;

namespace Papau.Levelmeter.SvgHelper;

public class SvgModel
{
    public static XNamespace Xmlns => "http://www.w3.org/2000/svg";
    public static XNamespace Xmlns_xlink => "http://www.w3.org/1999/xlink";

    private readonly List<SvgElement> _definitions = [];
    private readonly List<SvgElement> _elements = [];

    public IReadOnlyCollection<SvgElement> Definitions => _definitions.AsReadOnly();
    public IReadOnlyCollection<SvgElement> Elements => _elements.AsReadOnly();
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Color Background { get; set; } = Color.White;
    public SizeF Padding { get; set; } = new(5, 5);

    public void FillRectangle(PointF markerPos, SizeF size, Color color, string name)
    {
        var reference = new SvgRectangle()
        {
            Id = name,
            Position = markerPos,
            Fill = GetSvgColor(color),
            Size = size,
        };

        _elements.Add(reference);
    }

    public void FillPath(SKPath textPath, PointF position, Color color)
    {
        var path = new SvgPath
        {
            Data = ConvertPathToSvg(textPath),
            Fill = GetSvgColor(color),
            Position = position,
            Size = new SizeF(textPath.Bounds.Width, textPath.Bounds.Height)
        };

        _elements.Add(path);
    }

    public void StrokeRectangle(PointF pos, SizeF size, Color color, float strokeWidth)
    {
        var rect = new SvgRectangle
        {
            Size = size,
            Position = pos,
            Fill = "none",
            Stroke = GetSvgColor(color),
            StrokeWidth = strokeWidth
        };

        _elements.Add(rect);
    }

    public async Task SaveToStream(Stream stream, LengthUnit unit, CancellationToken cancellationToken)
    {
        var normalizedElements = NormalizeElements();
        var elementBounds = CalculateBounds(normalizedElements);
        var totalSize = elementBounds.Size + 2 * Padding;

        var svgDoc = new XDocument(
            new XElement(Xmlns + "svg",
                new XAttribute("xmlns", Xmlns.ToString()),
                new XAttribute(XNamespace.Xmlns + "xlink", Xmlns_xlink.ToString()),
                new XAttribute("version", "1.1"),
                new XAttribute("width", $"{totalSize.Width}{Length.GetAbbreviation(unit)}"),
                new XAttribute("height", $"{totalSize.Height}{Length.GetAbbreviation(unit)}"),
                new XAttribute("viewBox", $"0 0 {totalSize.Width} {totalSize.Height}"),

                AddSimpleElementIfNotEmpty("title", Title),
                AddSimpleElementIfNotEmpty("description", Description),

                AddDefinitions(),
                new XElement(Xmlns + "style", "svg { background: #ffffff; }"),

                // insert bounding rectangle for cutting the scale
                new SvgRectangle { Id = "cutLine", Size = totalSize, Position = new PointF(0, 0), Fill = "none", Stroke = "red", StrokeWidth = 0.05f }.GetElement(),

                AddElements(elementBounds.Location, normalizedElements)
            )
        );

        await svgDoc.SaveAsync(stream, SaveOptions.None, cancellationToken).ConfigureAwait(false);
    }

    private XElement AddDefinitions()
    {
        return new XElement(Xmlns + "defs", _definitions.Select(d => d.GetElement()));
    }

    private XElement AddElements(PointF minPositions, IEnumerable<SvgElement> normalizedElements)
    {
        var g = new SvgGroup()
        {
            Transform = new PointF(Padding.Width + Math.Abs(minPositions.X), Padding.Height + Math.Abs(minPositions.Y)),
            Id = "scale"
        }.GetElement();

        g.Add(normalizedElements.Select(e => e.GetElement()));

        return g;
    }

    private IEnumerable<SvgElement> NormalizeElements()
    {
        var yOffset = 0f;
        var xOffset = 0f;

        if (_elements.Any())
        {
            yOffset = _elements.Min(e => e.Position.Y);
            xOffset = _elements.Min(e => e.Position.X);
        }

        var normalizedElements = _elements.Select(e => e with { Position = new PointF(e.Position.X - xOffset, e.Position.Y - yOffset) });
        return normalizedElements;
    }

    private static XElement? AddSimpleElementIfNotEmpty(string elementType, string content)
    {
        return string.IsNullOrWhiteSpace(content) ? null : new XElement(Xmlns + elementType, content);
    }

    private RectangleF CalculateBounds(IEnumerable<SvgElement> elements)
    {
        var b = elements
            .Select(e => e.GetBounds())
            .Aggregate(
                RectangleF.Empty,
                (agg, b) => new RectangleF(
                    new PointF(Math.Min(agg.X, b.X), Math.Min(agg.Y, b.Y)),
                    new SizeF(
                        Math.Max(Math.Abs(agg.X) + agg.Width, Math.Abs(b.X) + b.Width),
                        Math.Max(Math.Abs(agg.Y) + agg.Height, Math.Abs(b.Y) + b.Height)
                    )
                )
            );

        return b;
    }

    private static string GetSvgColor(Color color) => ColorTranslator.ToHtml(color);

    private static string ConvertPathToSvg(SKPath path)
    {
        var svgPathBuilder = new StringBuilder();
        var iterator = path.CreateRawIterator();

        SKPathVerb verb;
        var points = new SKPoint[4];

        while ((verb = iterator.Next(points)) != SKPathVerb.Done)
        {
            switch (verb)
            {
                case SKPathVerb.Move:
                    svgPathBuilder.Append($"M {points[0].X} {points[0].Y} ");
                    break;

                case SKPathVerb.Line:
                    svgPathBuilder.Append($"L {points[1].X} {points[1].Y} ");
                    break;

                case SKPathVerb.Cubic:
                    svgPathBuilder.Append($"C {points[1].X} {points[1].Y}, {points[2].X} {points[2].Y}, {points[3].X} {points[3].Y} ");
                    break;

                case SKPathVerb.Quad:
                    svgPathBuilder.Append($"Q {points[1].X} {points[1].Y}, {points[2].X} {points[2].Y} ");
                    break;

                case SKPathVerb.Close:
                    svgPathBuilder.Append("Z ");
                    break;
            }
        }

        return svgPathBuilder.ToString().Trim();
    }
}