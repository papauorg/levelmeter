using System.Drawing;
using System.Globalization;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

using FluentAssertions;

using Microsoft.VisualStudio.TestPlatform.ObjectModel.Engine.ClientProtocol;

using Newtonsoft.Json;

using Papau.Levelmeter.SvgHelper;

using UnitsNet.Units;

namespace Papau.Levelmeter.Tests;

public class SvgModelTests
{
    public class SaveToStreamMethod : SvgModelTests
    {
        readonly SvgModel _emptyModel;

        public SaveToStreamMethod()
        {
            _emptyModel = new SvgModel
            {
                Background = Color.White,
                Title = "Test scale",
                Description = "Test description",
                Padding = SizeF.Empty
            };
        }

        private async Task<XElement> GetDocumentRoot(SvgModel model)
        {
            using var s = new MemoryStream();
            await model.SaveToStream(s, LengthUnit.Millimeter, CancellationToken.None);
            s.Position = 0;

            var doc = await XDocument.LoadAsync(s, LoadOptions.None, CancellationToken.None);

            var svg = doc.Root!;
            svg.Should().NotBeNull();

            return svg;
        }

        [Fact]
        public async Task Writes_Svg_Root_Element_With_Attributes()
        {
            var svg = await GetDocumentRoot(_emptyModel);

            svg.Attributes().Should().ContainSingle(a => a.Name == "xmlns" && a.Value.EndsWith("/svg"));
            svg.Attributes().Should().ContainSingle(a => a.Name.LocalName == "xlink");


            svg.Attribute("version").Should().NotBeNull();
            svg.Attribute("viewBox").Should().NotBeNull().And.Subject.Value.Should().Be("0 0 0 0");
            svg.Attribute("height").Should().NotBeNull().And.Subject.Value.Should().Be("0mm");
            svg.Attribute("width").Should().NotBeNull().And.Subject.Value.Should().Be("0mm");
        }

        [Fact]
        public async Task Result_Contains_Title_And_Description_Tags()
        {
            var svg = await GetDocumentRoot(_emptyModel);
            svg.Element(SvgModel.Xmlns + "title").Should().NotBeNull().And.Subject.Value.Should().Be(_emptyModel.Title);
            svg.Element(SvgModel.Xmlns + "description").Should().NotBeNull().And.Subject.Value.Should().Be(_emptyModel.Description);
        }

        [Fact]
        public async Task Result_Contains_Style_Tag()
        {
            var svg = await GetDocumentRoot(_emptyModel);
            svg.Element(SvgModel.Xmlns + "style").Should().NotBeNull().And.Subject.Value.Should().Contain("background: #ffffff;");
        }

        public static IEnumerable<object[]> EnclosingRectangleSamples()
        {
            return [
                new object[] { new PointF[] { new(0, 0) }, new SvgRectangle { Size = new(5, 25) }, new RectangleF(0, 0, 5, 25) },
                new object[] { new PointF[] { new(0, -0.75f) }, new SvgRectangle { Size = new(10, 1.5f) }, new RectangleF(0, 0, 10, 1.5f) },
                new object[] { new PointF[] { new(0, -0.75f), new(0, 7.75f) }, new SvgRectangle { Size = new(10, 1.5f) }, new RectangleF(0, 0, 10, 10) },
                new object[] { new PointF[] { new(0, 0.75f), new(0, 7.75f) }, new SvgRectangle { Size = new(10, 1.5f) }, new RectangleF(0, 0, 10, 8.5f) },
            ];
        }

        [Theory]
        [MemberData(nameof(EnclosingRectangleSamples))]
        public async Task Result_Contains_Bounding_Rectangle_That_Encloses_All_Elements_Tightly(IEnumerable<PointF> usages, SvgElement definition, RectangleF boundingRectangle)
        {
            var model = new SvgModel
            {
                Padding = SizeF.Empty
            };

            foreach (var (usage, i) in usages.Select((v, i) => (v, i)))
                model.FillRectangle(usage, definition.Size, Color.Black, $"mark_{i}");

            var svg = await GetDocumentRoot(model);

            var namespaceManager = new XmlNamespaceManager(new NameTable());
            namespaceManager.AddNamespace("svg", SvgModel.Xmlns.ToString());

            var cutLine = svg.XPathSelectElement("./svg:rect[@id='cutLine']", namespaceManager)!;
            cutLine.Should().NotBeNull();
            cutLine.Attribute("x")?.Value.Should().Be(boundingRectangle.X.ToString(CultureInfo.InvariantCulture));
            cutLine.Attribute("y")?.Value.Should().Be(boundingRectangle.Y.ToString(CultureInfo.InvariantCulture));
            cutLine.Attribute("width")?.Value.Should().Be(boundingRectangle.Width.ToString(CultureInfo.InvariantCulture));
            cutLine.Attribute("height")?.Value.Should().Be(boundingRectangle.Height.ToString(CultureInfo.InvariantCulture));
        }
    }
}