using System.Drawing;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Papau.Levelmeter.SvgHelper;

[XmlRoot("use")]
public record SvgUse : SvgElement
{
    public string ReferenceId { get; init; } = "";

    public required SvgElement ReferencedElement { get; init; }

    public static SvgUse From(SvgElement e)
    {
        return new SvgUse { ReferenceId = $"#{e.Id}", ReferencedElement = e };
    }

    public override RectangleF GetBounds()
    {
        return new RectangleF(Position, ReferencedElement.Size);
    }

    public override XElement GetElement()
    {
        var e = base.GetElement();

        if (string.IsNullOrWhiteSpace(ReferenceId))
            throw new InvalidOperationException("Can't create use element without reference id");

        e.Add(new XAttribute(SvgModel.Xmlns_xlink + "href", ReferenceId)); // fallback for svg v1.1
        e.Add(new XAttribute("href", ReferenceId));

        return e;
    }
}