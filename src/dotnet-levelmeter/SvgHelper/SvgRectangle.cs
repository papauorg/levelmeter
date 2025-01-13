using System.Drawing;
using System.Xml.Serialization;

namespace Papau.Levelmeter.SvgHelper;

[XmlRoot("rect")]
public record SvgRectangle : SvgElement
{
}