using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Slysoft.RestResource.Extensions;
using SlySoft.RestResource.HalJson;
using TestUtils;
// ReSharper disable RedundantAnonymousTypePropertyName

namespace Slysoft.RestResource.HalJson.Tests; 

[TestClass]
public class ToHalJsonGetLinkTest {
    [TestMethod]
    public void GetMustBeConvertedToLinkInJson() {
        //arrange
        var href = GenerateRandom.String();
        var resource = new Resource()
            .Get("getLink", href);

        //act
        var json = resource.ToHalJson();

        //assert
        var expected = new {
            _links = new {
                getLink = new {
                    href = href
                }
            }
        };
        var expectedJson = JsonConvert.SerializeObject(expected, Formatting.Indented);
        Assert.AreEqual(expectedJson, json);
    }

    [TestMethod]
    public void MultipleGetsMustBeConvertedToLinksInJson() {
        //arrange
        var href1 = GenerateRandom.String();
        var href2 = GenerateRandom.String();
        var resource = new Resource()
            .Get("getLink1", href1)
            .Get("getLink2", href2);

        //act
        var json = resource.ToHalJson();

        //assert
        var expected = new {
            _links = new {
                getLink1 = new {
                    href = href1
                },
                getLink2 = new {
                    href = href2
                },
            }
        };
        var expectedJson = JsonConvert.SerializeObject(expected, Formatting.Indented);
        Assert.AreEqual(expectedJson, json);
    }

    [TestMethod]
    public void GetMustIncludeTemplatedInJson() {
        //arrange
        var href = GenerateRandom.String();
        var resource = new Resource()
            .Get("getLink", href, templated: true);

        //act
        var json = resource.ToHalJson();

        //assert
        var expected = new {
            _links = new {
                getLink = new {
                    href = href,
                    templated = true
                }
            }
        };
        var expectedJson = JsonConvert.SerializeObject(expected, Formatting.Indented);
        Assert.AreEqual(expectedJson, json);
    }

    [TestMethod]
    public void QueryMustIncludeParametersInJson() {
        //arrange
        var href = GenerateRandom.String();
        var resource = new Resource()
            .QueryMap<UserSearch>("getLink", href)
                .Parameter(x => x.FirstName)
                .Parameter(x => x.LastName)
            .EndQuery();


        //act
        var json = resource.ToHalJson();

        //assert
        var expected = new {
            _links = new {
                getLink = new {
                    href = href,
                    parameters = new {
                        firstName = new {}, 
                        lastName = new {}
                    } 
                }
            }
        };
        var expectedJson = JsonConvert.SerializeObject(expected, Formatting.Indented);
        Assert.AreEqual(expectedJson, json);
    }

    [TestMethod]
    public void QueryMustIncludeParametersPropertiesInJson() {
        //arrange
        var href = GenerateRandom.String();
        var resource = new Resource()
            .QueryMap<UserSearch>("getLink", href)
                .Parameter(x => x.Position, defaultValue: "Admin", listOfValues: new []{"Standard", "Admin"})
                .Parameter(x => x.YearsEmployed, type: "number")
            .EndQuery();


        //act
        var json = resource.ToHalJson();

        //assert
        var expected = new {
            _links = new {
                getLink = new {
                    href = href,
                    parameters = new {
                        position = new { defaultValue = "Admin", listOfValues = new[]{"Standard", "Admin"}}, 
                        yearsEmployed = new { type = "number" }
                    }
                }
            }
        };
        var expectedJson = JsonConvert.SerializeObject(expected, Formatting.Indented);
        Assert.AreEqual(expectedJson, json);
    }
}