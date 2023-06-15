using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestResource;
using RestResource.Extensions;
using TestUtils;

namespace HalXml.Tests; 

[TestClass]
public sealed class ToHalXmlDataTests {
    private const string XmlHeader = "<?xml version=\"1.0\" encoding=\"utf-16\"?>";

    [TestMethod]
    public void DataValuesMustBeConvertedToXml() {
        //arrange
        var stringValue = GenerateRandom.String();
        var intValue = GenerateRandom.Int();

        var resource = new Resource()
            .Data("stringValue", stringValue)
            .Data("intValue", intValue);

        //assert
        var xml = resource.ToHalXml();

        //assert
        var expectedXml = $"{XmlHeader}<resource><stringValue>{stringValue}</stringValue><intValue>{intValue}</intValue></resource>";
        Assert.AreEqual(expectedXml, xml);
    }

    [TestMethod]
    public void ObjectDataMustBeConvertedToXlm() {
        //arrange
        var testObject = new TestObject();

        var resource = new Resource()
            .Data("testObject", testObject);

        //act
        var xml = resource.ToHalXml();

        //assert
        var expectedXml = $"{XmlHeader}<resource><testObject><stringValue>{testObject.StringValue}</stringValue><intValue>{testObject.IntValue}</intValue></testObject></resource>";
        Assert.AreEqual(expectedXml, xml);
    }

    [TestMethod]
    public void ArrayDataValuesMustBeConvertedToJson() {
        //arrange
        var strings = new List<string> {
            GenerateRandom.String(),
            GenerateRandom.String(),
            GenerateRandom.String()
        };

        var resource = new Resource()
            .Data("strings", strings);

        //act
        var xml = resource.ToHalXml();

        //assert
        var expectedXml = $"{XmlHeader}<resource><strings><value>{strings[0]}</value><value>{strings[1]}</value><value>{strings[2]}</value></strings></resource>";
        Assert.AreEqual(expectedXml, xml);
    }

    [TestMethod]
    public void ArrayObjectsMustBeConvertedToJson() {
        //arrange
        var dataObjects = new List<TestObject> { new(), new(), new() };

        var resource = new Resource()
            .Data("dataObjects", dataObjects);

        //assert
        var xml = resource.ToHalXml();

        //assert
        var expectedXml = $"{XmlHeader}<resource><dataObjects><value><stringValue>{dataObjects[0].StringValue}</stringValue><intValue>{dataObjects[0].IntValue}</intValue></value><value><stringValue>{dataObjects[1].StringValue}</stringValue><intValue>{dataObjects[1].IntValue}</intValue></value><value><stringValue>{dataObjects[2].StringValue}</stringValue><intValue>{dataObjects[2].IntValue}</intValue></value></dataObjects></resource>";
        Assert.AreEqual(expectedXml, xml);
    }
}