using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestResource;
using RestResource.Extensions;
using TestUtils;

namespace HalXml.Tests; 

[TestClass]
public class DataTests {
    [TestMethod]
    public void DataValuesMustBeConvertedToJson() {
        //arrange
        var uri = GenerateRandom.String();

        var resource = new Resource()
            .Uri(uri);

        //act
        var json = resource.ToHalXml();

        //assert
        var expectedJson = $"<?xml version=\"1.0\" encoding=\"utf-16\"?><Resource rel=\"self\" href=\"{uri}\" />";
        Assert.AreEqual(expectedJson, json);
    }
}