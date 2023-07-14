using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestUtils;

namespace SlySoft.RestResource.HalXml.Tests;

[TestClass]
public sealed class DataTests {
    private const string XmlHeader = "<?xml version=\"1.0\" encoding=\"utf-16\"?>";

    [TestMethod]
    public void UriMustBeAddedAsHrefInXml() {
        //arrange
        var uri = GenerateRandom.String();
        var message = GenerateRandom.String();

        var resource = new Resource()
            .Uri(uri)
            .Data("message", message);

        //act
        var xml = resource.ToHalXml();

        //assert
        var expectedXml = $"{XmlHeader}<resource rel=\"self\" href=\"{uri}\"><message>{message}</message></resource>";
        Assert.AreEqual(expectedXml, xml);
    }
}