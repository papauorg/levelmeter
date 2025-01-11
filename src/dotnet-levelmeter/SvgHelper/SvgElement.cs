using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Papau.Levelmeter.SvgHelper;

public abstract record SvgElement
{
    public string Id { get; init; } = "";
    public string Fill { get; init; } = "";
    public string Stroke { get; init; } = "";
    public float StrokeWidth { get; init; } = 0;
    public PointF Position { get; init; } = PointF.Empty;
    public SizeF Size { get; init; } = SizeF.Empty;

    public virtual RectangleF GetBounds()
    {
        return new RectangleF(Position, Size);
    }

    public virtual XElement GetElement()
    {
        var elementType = GetType().GetCustomAttribute<XmlRootAttribute>(false)!.ElementName;
        var e = new XElement(SvgModel.Xmlns + elementType,
            GetAttributeIfNotEmpty("id", Id),
            GetAttributeIfNotEmpty("fill", Fill),
            GetAttributeIfNotEmpty("stroke", Stroke)
        );

        if (StrokeWidth > 0)
            e.Add(GetAttributeIfNotEmpty("stroke-width", StrokeWidth.ToString(CultureInfo.InvariantCulture)));

        if (Position != PointF.Empty)
        {

            e.Add(new XAttribute("x", Position.X.ToString(CultureInfo.InvariantCulture)));
            e.Add(new XAttribute("y", Position.Y.ToString(CultureInfo.InvariantCulture)));
        }

        if (Size != SizeF.Empty)
        {
            e.Add(new XAttribute("width", Size.Width.ToString(CultureInfo.InvariantCulture)));
            e.Add(new XAttribute("height", Size.Height.ToString(CultureInfo.InvariantCulture)));
        }

        return e;
    }

    protected static XAttribute? GetAttributeIfNotEmpty(string name, string value)
    {
        return !string.IsNullOrWhiteSpace(value) ? new XAttribute(name, value) : null;
    }
}