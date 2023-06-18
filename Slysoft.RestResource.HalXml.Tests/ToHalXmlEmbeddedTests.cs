using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Slysoft.RestResource.Extensions;
using TestUtils;

namespace Slysoft.RestResource.HalXml.Tests; 

[TestClass]
public sealed class ToHalXmlEmbeddedTests {
    private const string XmlHeader = "<?xml version=\"1.0\" encoding=\"utf-16\"?>";

    [TestMethod]
    public void EmbeddedResourceMustBeReturnedInJson() {
        //arrange
        var parentMessage = GenerateRandom.String();
        var childMessage = GenerateRandom.String();

        var child = new Resource()
            .Data("message", childMessage);

        var parent = new Resource()
            .Data("message", parentMessage)
            .Embedded("child", child);

        //act
        var xml = parent.ToHalXml();

        //assert
        var expectedXml = $"{XmlHeader}<resource rel=\"self\"><message>{parentMessage}</message><resource rel=\"child\"><message>{childMessage}</message></resource></resource>";
        Assert.AreEqual(expectedXml, xml);

    }

    [TestMethod]
    public void MultipleEmbeddedResourcesMustBeReturnedInJson() {
        //arrange
        var parentMessage = GenerateRandom.String();
        var childMessage1 = GenerateRandom.String();
        var childMessage2 = GenerateRandom.String();

        var child1 = new Resource()
            .Data("message", childMessage1);

        var child2 = new Resource()
            .Data("message", childMessage2);

        var parent = new Resource()
            .Data("message", parentMessage)
            .Embedded("child1", child1)
            .Embedded("child2", child2);

        //act
        var xml = parent.ToHalXml();

        //assert
        var expectedXml = $"{XmlHeader}<resource rel=\"self\"><message>{parentMessage}</message><resource rel=\"child1\"><message>{childMessage1}</message></resource><resource rel=\"child2\"><message>{childMessage2}</message></resource></resource>";
        Assert.AreEqual(expectedXml, xml);
    }

    [TestMethod]
    public void EmbeddedResourceListMustBeReturnedInJson() {
        //arrange
        var parentMessage = GenerateRandom.String();
        var childMessage1 = GenerateRandom.String();
        var childMessage2 = GenerateRandom.String();

        var child1 = new Resource()
            .Data("name", "Julie")
            .Data("message", childMessage1);

        var child2 = new Resource()
            .Data("name", "Sam")
            .Data("message", childMessage2);

        var children = new List<Resource> { child1, child2 };

        var parent = new Resource()
            .Data("message", parentMessage)
            .Embedded("children", children);

        //act
        var xml = parent.ToHalXml();

        //assert
        var expectedXml = $"{XmlHeader}<resource rel=\"self\"><message>{parentMessage}</message><resource rel=\"children\"><name>Julie</name><message>{childMessage1}</message></resource><resource rel=\"children\"><name>Sam</name><message>{childMessage2}</message></resource></resource>";
        Assert.AreEqual(expectedXml, xml);
    }
}