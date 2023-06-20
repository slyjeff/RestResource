using Microsoft.VisualStudio.TestTools.UnitTesting;
using Slysoft.RestResource.Extensions;
using TestUtils;

namespace Slysoft.RestResource.Tests; 

[TestClass]
public class PatchTests {
    [TestMethod]
    public void PatchMustAddLink() {
        //arrange
        const string uri = "/api/user";

        //act
        var resource = new Resource()
            .Patch("UpdateUser", uri)
            .EndBody();

        //assert
        var link = resource.GetLink("updateUser");
        Assert.IsNotNull(link);
        Assert.AreEqual("updateUser", link.Name);
        Assert.AreEqual(uri, link.Href);
        Assert.IsFalse(link.Templated);
        Assert.AreEqual("PATCH", link.Verb);
    }

    [TestMethod]
    public void PatchMustAllowForTemplating() {
        //act
        var resource = new Resource()
            .Get("updateUser", "/api/user/{userType}", templated: true);

        //assert
        var link = resource.GetLink("updateUser");
        Assert.IsNotNull(link);
        Assert.IsTrue(link.Templated);
    }

    [TestMethod]
    public void PatchMustAllowForTimeout() {
        //act
        var resource = new Resource()
            .Get("updateUser", "/api/user/{userType}", timeout: 60);

        //assert
        var link = resource.GetLink("updateUser");
        Assert.IsNotNull(link);
        Assert.AreEqual(60, link.Timeout);
    }

    [TestMethod]
    public void PatchMustAllowConfigurationOfField() {
        //act
        var resource = new Resource()
            .Patch("updateUser", "/api/user")
                .Field("lastName")
                .Field("firstName")
            .EndBody()
            .Get("getUser", "/api/user{id}", templated: true); //just to prove we can do chaining after a Patch

        //assert
        var link = resource.GetLink("updateUser");
        Assert.IsNotNull(link);
        Assert.IsNotNull(link.GetInputItem("lastName"));
        Assert.IsNotNull(link.GetInputItem("firstName"));
        Assert.IsNotNull(resource.GetLink("getUser"));
    }

    [TestMethod]
    public void PatchMustAllowSettingOfDefaultValues() {
        //act
        var resource = new Resource()
            .Patch("updateUser", "/api/user")
                .Field("position", defaultValue: "admin")
            .EndBody();

        //assert
        var link = resource.GetLink("updateUser");
        Assert.IsNotNull(link);
        var queryParameter = link.GetInputItem("position");
        Assert.IsNotNull(queryParameter);
        Assert.AreEqual("admin", queryParameter.DefaultValue);
    }

    [TestMethod]
    public void PatchMustAllowSettingListOfValues() {
        //act
        var resource = new Resource()
            .Patch("updateUser", "/api/user")
                .Field("position", listOfValues: new[] { "Standard", "Admin" })
            .EndBody();

        //assert
        var link = resource.GetLink("updateUser");
        Assert.IsNotNull(link);
        var queryParameter = link.GetInputItem("position");
        Assert.IsNotNull(queryParameter);
        Assert.AreEqual("Standard", queryParameter.ListOfValues[0]);
        Assert.AreEqual("Admin", queryParameter.ListOfValues[1]);
    }

    [TestMethod]
    public void PatchMustAllowSettingValueType() {
        //act
        var resource = new Resource()
            .Patch("updateUser", "/api/user")
                .Field("yearsEmployed", type: "number")
            .EndBody();

        //assert
        var link = resource.GetLink("updateUser");
        Assert.IsNotNull(link);
        var queryParameter = link.GetInputItem("yearsEmployed");
        Assert.IsNotNull(queryParameter);
        Assert.AreEqual("number", queryParameter.Type);
    }

    [TestMethod]
    public void PatchMustAllowMappingConfigurationOfParameters() {
        //act
        var resource = new Resource()
            .Patch<User>("updateUser", "/api/user")
                .Field(x => x.LastName)
                .Field(x => x.FirstName)
            .EndBody()
            .Get("getUser", "/api/user{id}", templated: true); //just to prove we can do chaining after a query

        //assert
        var link = resource.GetLink("updateUser");
        Assert.IsNotNull(link);
        Assert.IsNotNull(link.GetInputItem("lastName"));
        Assert.IsNotNull(link.GetInputItem("firstName"));
        Assert.AreEqual("PATCH", link.Verb);
        Assert.IsNotNull(resource.GetLink("getUser"));
    }

    [TestMethod]
    public void PatchMappingMustAllowSettingOfDefaultValues() {
        //act
        var resource = new Resource()
            .Patch<User>("updateUser", "/api/user")
                .Field(x => x.Position, defaultValue: "admin")
            .EndBody();

        //assert
        var link = resource.GetLink("updateUser");
        Assert.IsNotNull(link);
        var queryParameter = link.GetInputItem("position");
        Assert.IsNotNull(queryParameter);
        Assert.AreEqual("admin", queryParameter.DefaultValue);
    }

    [TestMethod]
    public void PatchMappingMustAllowSettingListOfValues() {
        //act
        var resource = new Resource()
            .Patch<User>("updateUser", "/api/user")
                .Field(x => x.Position, listOfValues: new[] { "Standard", "Admin" })
            .EndBody();

        //assert
        var link = resource.GetLink("updateUser");
        Assert.IsNotNull(link);
        var queryParameter = link.GetInputItem("position");
        Assert.IsNotNull(queryParameter);
        Assert.AreEqual("Standard", queryParameter.ListOfValues[0]);
        Assert.AreEqual("Admin", queryParameter.ListOfValues[1]);
    }

    [TestMethod]
    public void PatchMappingMustAllowSettingValueType() {
        //act
        var resource = new Resource()
            .Patch<User>("updateUser", "/api/user")
                .Field(x => x.YearsEmployed, type: "number")
            .EndBody();

        //assert
        var link = resource.GetLink("updateUser");
        Assert.IsNotNull(link);
        var queryParameter = link.GetInputItem("yearsEmployed");
        Assert.IsNotNull(queryParameter);
        Assert.AreEqual("number", queryParameter.Type);
    }

    [TestMethod]
    public void PatchMappingMustAutomaticallyPopulateListOfValuesForBoolean() {
        //act
        var resource = new Resource()
            .Patch<User>("updateUser", "/api/user")
                .Field(x => x.IsRegistered)
            .EndBody();

        //assert
        var link = resource.GetLink("updateUser");
        Assert.IsNotNull(link);
        var queryParameter = link.GetInputItem("isRegistered");
        Assert.IsNotNull(queryParameter);
        Assert.AreEqual("True", queryParameter.ListOfValues[0]);
        Assert.AreEqual("False", queryParameter.ListOfValues[1]);
    }

    [TestMethod]
    public void PostMappingMustAutomaticallyPopulateListOfValuesForEnumerations() {
        //act
        var resource = new Resource()
            .Patch<User>("updateUser", "/api/user")
                .Field(x => x.Position)
            .EndBody();

        //assert
        var link = resource.GetLink("updateUser");
        Assert.IsNotNull(link);
        var queryParameter = link.GetInputItem("position");
        Assert.IsNotNull(queryParameter);
        Assert.AreEqual(UserPosition.Standard.ToString(), queryParameter.ListOfValues[0]);
        Assert.AreEqual(UserPosition.Admin.ToString(), queryParameter.ListOfValues[1]);
    }

    [TestMethod]
    public void PatchMappingMustSupportMapAll() {
        //act
        var resource = new Resource()
            .Patch<User>("updateUser", "/api/user")
                .AllFields()
            .EndBody();

        //assert
        var link = resource.GetLink("updateUser");
        Assert.IsNotNull(link);
        Assert.IsNotNull(link.GetInputItem("lastName"));
        Assert.IsNotNull(link.GetInputItem("firstName"));
        Assert.IsNotNull(link.GetInputItem("position"));
        Assert.IsNotNull(link.GetInputItem("yearsEmployed"));
        Assert.IsNotNull(link.GetInputItem("position"));
    }

    [TestMethod]
    public void PatchMappingMustSupportExcludingAParameter() {
        //act
        var resource = new Resource()
            .Patch<User>("updateUser", "/api/user")
                .Exclude(x => x.FirstName)
                .AllFields()
            .EndBody();

        //assert
        var link = resource.GetLink("updateUser");
        Assert.IsNotNull(link);
        Assert.IsNotNull(link);
        Assert.IsNotNull(link.GetInputItem("lastName"));
        Assert.IsNull(link.GetInputItem("firstName"));
        Assert.IsNotNull(link.GetInputItem("position"));
        Assert.IsNotNull(link.GetInputItem("yearsEmployed"));
        Assert.IsNotNull(link.GetInputItem("position"));
    }

    [TestMethod]
    public void PatchWithAllFieldsMustMapAllWithNoConfiguration() {
        //act
        var resource = new Resource()
            .PatchWithAllFields<User>("updateUser", "/api/user")
            .Get("getUser", "/api/user{id}", templated: true); //just to prove we can do chaining after a query

        //assert
        var link = resource.GetLink("updateUser");
        Assert.IsNotNull(link);
        Assert.IsNotNull(link.GetInputItem("lastName"));
        Assert.IsNotNull(link.GetInputItem("firstName"));
        Assert.IsNotNull(link.GetInputItem("position"));
        Assert.IsNotNull(link.GetInputItem("yearsEmployed"));
        Assert.IsNotNull(link.GetInputItem("position"));
        Assert.AreEqual("PATCH", link.Verb);
        Assert.IsNotNull(resource.GetLink("getUser"));
    }
}