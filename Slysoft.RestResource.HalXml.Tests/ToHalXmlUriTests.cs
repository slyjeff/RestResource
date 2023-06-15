using Microsoft.VisualStudio.TestTools.UnitTesting;
using Slysoft.RestResource.Extensions;
using TestUtils;

namespace Slysoft.RestResource.HalXml.Tests; 

[TestClass]
public sealed class DataTests {
    [TestMethod]
    public void DataValuesMustBeConvertedToJson() {
        //arrange
        var uri = GenerateRandom.String();

        var resource = new Resource()
            .Uri(uri);

        //act
        var xml = resource.ToHalXml();

        //assert
        var expectedXml = $"<?xml version=\"1.0\" encoding=\"utf-16\"?><resource rel=\"self\" href=\"{uri}\" />";
        Assert.AreEqual(expectedXml, xml);
    }
}