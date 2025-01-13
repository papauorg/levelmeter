using System.Drawing;
using System.Globalization;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Papau.Levelmeter.SvgHelper;

[XmlRoot("path")]
public record SvgPath : SvgElement
{
    public string Data { get; init; } = string.Empty;

    public override XElement GetElement()
    {
        var e = base.GetElement();

        if (!string.IsNullOrWhiteSpace(Data))
            e.Add(new XAttribute("d", Data));

        // Size attributes on path are only for calculating the scale size and
        // should not be applied to the svg element.
        e.Attribute("width")?.Remove();
        e.Attribute("height")?.Remove();

        // x and y coordinates do not apply to paths, instead they need transformation
        e.Attribute("x")?.Remove();
        e.Attribute("y")?.Remove();
        e.Add(new XAttribute("transform", $"translate({Position.X.ToString(CultureInfo.InvariantCulture)}, {Position.Y.ToString(CultureInfo.InvariantCulture)})"));

        return e;
    }
}