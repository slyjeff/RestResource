using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Slysoft.RestResource.Extensions;
using SlySoft.RestResource.HalJson;
using Slysoft.RestResource.HalJson.Tests.Utils;
using TestUtils;

namespace Slysoft.RestResource.HalJson.Tests; 

[TestClass]
public sealed class ToHalJsonEmbeddedTests {
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
        var json = parent.ToHalJson();

        //assert
        var expected = new {
            message = parentMessage,
            _embedded = new {
                child = "{child}"
            }
        };

        var expectedJson = JsonConvert
            .SerializeObject(expected, Formatting.Indented)
            .WithPlaceholder("child", child.ToHalJson());

        Assert.AreEqual(expectedJson, json);
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
        var json = parent.ToHalJson();

        //assert
        var expected = new {
            message = parentMessage,
            _embedded = new {
                child1 = "{child1}",
                child2 = "{child2}"
            }
        };

        var expectedJson = JsonConvert
            .SerializeObject(expected, Formatting.Indented)
            .WithPlaceholder("child1", child1.ToHalJson())
            .WithPlaceholder("child2", child2.ToHalJson());

        Assert.AreEqual(expectedJson, json);
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
        var json = parent.ToHalJson();

        //assert
        var expected = new {
            message = parentMessage,
            _embedded = new {
                children = new[] { "{child1}", "{child2}" }
            }
        };

        var expectedJson = JsonConvert
            .SerializeObject(expected, Formatting.Indented)
            .WithPlaceholder("child1", child1.ToHalJson())
            .WithPlaceholder("child2", child2.ToHalJson());

        Assert.AreEqual(expectedJson, json);
    }
}