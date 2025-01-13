using System.Drawing;
using System.Globalization;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Papau.Levelmeter.SvgHelper;

[XmlRoot("g")]
public record SvgGroup : SvgElement
{
    public PointF Transform { get; init; } = PointF.Empty;

    public override XElement GetElement()
    {
        var e = base.GetElement();

        if (Transform != PointF.Empty)
            e.Add(new XAttribute("transform", $"translate({Transform.X.ToString(CultureInfo.InvariantCulture)}, {Transform.Y.ToString(CultureInfo.InvariantCulture)})"));

        return e;
    }
}