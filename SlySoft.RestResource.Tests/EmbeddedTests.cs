using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestUtils;

namespace SlySoft.RestResource.Tests;

[TestClass]
public sealed class EmbeddedTests {
    [TestMethod]
    public void MustBeAbleToAddAnEmbeddedResource() {
        //arrange
        var message = GenerateRandom.String();
        var embeddedResource = new Resource()
            .Data("message", message);

        //act
        var resource = new Resource()
            .Embedded("subResource", embeddedResource);

        //assert
        var subResource = resource.GetEmbedded("subResource");
        Assert.IsNotNull(subResource);
        Assert.AreEqual(message, subResource.Data["message"]);
    }

    [TestMethod]
    public void MustBeAbleToAddAListOfEmbeddedResources() {
        //arrange
        var message1 = GenerateRandom.String();
        var message2 = GenerateRandom.String();

        var embeddedResource1 = new Resource()
            .Data("message", message1);
        
        var embeddedResource2 = new Resource()
            .Data("message", message2);

        //act
        var resource = new Resource()
            .Embedded("subResources", new List<Resource> { embeddedResource1, embeddedResource2 });

        //assert
        var subResources = resource.GetEmbeddedList("subResources");
        Assert.IsNotNull(subResources);
        Assert.AreEqual(message1, subResources[0].Data["message"]);
        Assert.AreEqual(message2, subResources[1].Data["message"]);
    }
}