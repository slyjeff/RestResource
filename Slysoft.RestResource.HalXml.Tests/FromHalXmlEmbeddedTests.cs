﻿using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SlySoft.RestResource.Hal;
using TestUtils;

namespace SlySoft.RestResource.HalXml.Tests;

[TestClass]
public sealed class FromHalXmlEmbeddedTests {
    [TestMethod]
    public void MustBeAbleToReadAnEmbeddedResource() {
        //arrange
        var message = GenerateRandom.String();
        var embeddedResource = new Resource()
            .Data("message", message);

        var resource = new Resource()
            .Embedded("subResource", embeddedResource);

        var xml = resource.ToSlySoftHalXml();

        //act
        var deserializedResource = new Resource().FromSlySoftHalXml(xml);

        //assert
        var subResource = deserializedResource.GetEmbedded("subResource");
        Assert.IsNotNull(subResource);
        Assert.AreEqual(message, subResource.Data["message"]);
    }

    [TestMethod]
    public void MustBeAbleToReadAListOfEmbeddedResources() {
        //arrange
        var message1 = GenerateRandom.String();
        var message2 = GenerateRandom.String();

        var embeddedResource1 = new Resource()
            .Data("message", message1);

        var embeddedResource2 = new Resource()
            .Data("message", message2);

        var resource = new Resource()
            .Embedded("subResources", new List<Resource> { embeddedResource1, embeddedResource2 });

        var xml = resource.ToSlySoftHalXml();

        //act
        var deserializedResource = new Resource().FromSlySoftHalXml(xml);

        //assert
        var subResources = deserializedResource.GetEmbeddedList("subResources");
        Assert.IsNotNull(subResources);
        Assert.AreEqual(message1, subResources[0].Data["message"]);
        Assert.AreEqual(message2, subResources[1].Data["message"]);
    }
}