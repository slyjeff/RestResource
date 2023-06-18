﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Slysoft.RestResource.Extensions;
using TestUtils;

namespace Slysoft.RestResource.HalXml.Tests;

[TestClass]
public class ToHalJsonGetLinkTest {
    private const string XmlHeader = "<?xml version=\"1.0\" encoding=\"utf-16\"?>";

    [TestMethod]
    public void GetMustBeConvertedToLinkInXml() {
        //arrange
        var message = GenerateRandom.String();
        var href = GenerateRandom.String();

        var resource = new Resource()
            .Data("message", message)
            .Get("getLink", href);

        //act
        var xml = resource.ToHalXml();

        //assert
        var expectedXml = $"{XmlHeader}<resource rel=\"self\"><message>{message}</message><link rel=\"getLink\" href=\"{href}\" /></resource>";
        Assert.AreEqual(expectedXml, xml);
    }

    [TestMethod]
    public void MultipleGetsMustBeConvertedToLinksInXml() {
        //arrange
        var message = GenerateRandom.String();
        var href1 = GenerateRandom.String();
        var href2 = GenerateRandom.String();

        var resource = new Resource()
            .Data("message", message)
            .Get("getLink1", href1)
            .Get("getLink2", href2);

        //act
        var xml = resource.ToHalXml();

        //assert
        var expectedXml = $"{XmlHeader}<resource rel=\"self\"><message>{message}</message><link rel=\"getLink1\" href=\"{href1}\" /><link rel=\"getLink2\" href=\"{href2}\" /></resource>";
        Assert.AreEqual(expectedXml, xml);
    }

    [TestMethod]
    public void GetMustIncludeTemplatedInXml() {
        //arrange
        var message = GenerateRandom.String();
        var href = GenerateRandom.String();

        var resource = new Resource()
            .Data("message", message)
            .Get("getLink", href, templated: true);

        //act
        var xml = resource.ToHalXml();

        //assert
        var expectedXml = $"{XmlHeader}<resource rel=\"self\"><message>{message}</message><link rel=\"getLink\" href=\"{href}\" templated=\"true\" /></resource>";
        Assert.AreEqual(expectedXml, xml);
    }

    [TestMethod]
    public void QueryMustIncludeParametersInXml() {
        //arrange
        var message = GenerateRandom.String();
        var href = GenerateRandom.String();

        var resource = new Resource()
            .Data("message", message)
            .Query<User>("getLink", href)
                .Parameter(x => x.FirstName)
                .Parameter(x => x.LastName)
            .EndQuery();

        //act
        var xml = resource.ToHalXml();

        //assert
        var expectedXml = $"{XmlHeader}<resource rel=\"self\"><message>{message}</message><link rel=\"getLink\" href=\"{href}\"><parameter name=\"firstName\" /><parameter name=\"lastName\" /></link></resource>";
        Assert.AreEqual(expectedXml, xml);
    }

    [TestMethod]
    public void QueryMustIncludeParametersPropertiesInXml() {
        //arrange
        var message = GenerateRandom.String();
        var href = GenerateRandom.String();

        var resource = new Resource()
            .Data("message", message)
            .Query<User>("getLink", href)
                .Parameter(x => x.Position, defaultValue: "Admin", listOfValues: new[] { "Standard", "Admin" })
                .Parameter(x => x.YearsEmployed, type: "number")
            .EndQuery();

        //act
        var xml = resource.ToHalXml();

        //assert
        var expectedXml = $"{XmlHeader}<resource rel=\"self\"><message>{message}</message><link rel=\"getLink\" href=\"{href}\"><parameter name=\"position\"><defaultValue>Admin</defaultValue><listOfValues><value>Standard</value><value>Admin</value></listOfValues></parameter><parameter name=\"yearsEmployed\"><type>number</type></parameter></link></resource>";
        Assert.AreEqual(expectedXml, xml);
    }
}