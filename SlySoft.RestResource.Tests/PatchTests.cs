using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestUtils;

namespace SlySoft.RestResource.Tests;

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
        Assert.IsNotNull(link.GetParameter("lastName"));
        Assert.IsNotNull(link.GetParameter("firstName"));
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
        var linkParameter = link.GetParameter("position");
        Assert.IsNotNull(linkParameter);
        Assert.AreEqual("admin", linkParameter.DefaultValue);
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
        var linkParameter = link.GetParameter("position");
        Assert.IsNotNull(linkParameter);
        Assert.AreEqual("Standard", linkParameter.ListOfValues[0]);
        Assert.AreEqual("Admin", linkParameter.ListOfValues[1]);
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
        var linkParameter = link.GetParameter("yearsEmployed");
        Assert.IsNotNull(linkParameter);
        Assert.AreEqual("number", linkParameter.Type);
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
        Assert.IsNotNull(link.GetParameter("lastName"));
        Assert.IsNotNull(link.GetParameter("firstName"));
        Assert.AreEqual("PATCH", link.Verb);
        Assert.IsNotNull(resource.GetLink("getUser"));
    }

    [TestMethod]
    public void PatchMappingMustAllowSettingOfDefaultValues() {
        //act
        var resource = new Resource()
            .Patch<User>("updateUser", "/api/user")
                .Field(x => x.Position, defaultValue: UserPosition.Admin)
            .EndBody();

        //assert
        var link = resource.GetLink("updateUser");
        Assert.IsNotNull(link);
        var linkParameter = link.GetParameter("position");
        Assert.IsNotNull(linkParameter);
        Assert.AreEqual("Admin", linkParameter.DefaultValue);
    }

    [TestMethod]
    public void PatchMappingMustAllowSettingListOfValues() {
        //act
        var resource = new Resource()
            .Patch<User>("updateUser", "/api/user")
                .Field(x => x.Position, listOfValues: new[] { UserPosition.Standard, UserPosition.Admin })
            .EndBody();

        //assert
        var link = resource.GetLink("updateUser");
        Assert.IsNotNull(link);
        var linkParameter = link.GetParameter("position");
        Assert.IsNotNull(linkParameter);
        Assert.AreEqual("Standard", linkParameter.ListOfValues[0]);
        Assert.AreEqual("Admin", linkParameter.ListOfValues[1]);
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
        var linkParameter = link.GetParameter("yearsEmployed");
        Assert.IsNotNull(linkParameter);
        Assert.AreEqual("number", linkParameter.Type);
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
        var linkParameter = link.GetParameter("isRegistered");
        Assert.IsNotNull(linkParameter);
        Assert.AreEqual("True", linkParameter.ListOfValues[0]);
        Assert.AreEqual("False", linkParameter.ListOfValues[1]);
    }

    [TestMethod]
    public void PatchMappingMustAutomaticallyPopulateListOfValuesForNullableBoolean() {
        //act
        var resource = new Resource()
            .Patch<User>("updateUser", "/api/user")
                .Field(x => x.IsRegisteredNullable)
            .EndBody();

        //assert
        var link = resource.GetLink("updateUser");
        Assert.IsNotNull(link);
        var linkParameter = link.GetParameter("isRegisteredNullable");
        Assert.IsNotNull(linkParameter);
        Assert.AreEqual("True", linkParameter.ListOfValues[0]);
        Assert.AreEqual("False", linkParameter.ListOfValues[1]);
    }

    [TestMethod]
    public void PatchMappingMustAutomaticallyPopulateListOfValuesForEnumerations() {
        //act
        var resource = new Resource()
            .Patch<User>("updateUser", "/api/user")
                .Field(x => x.Position)
            .EndBody();

        //assert
        var link = resource.GetLink("updateUser");
        Assert.IsNotNull(link);
        var linkParameter = link.GetParameter("position");
        Assert.IsNotNull(linkParameter);
        Assert.AreEqual(UserPosition.Standard.ToString(), linkParameter.ListOfValues[0]);
        Assert.AreEqual(UserPosition.Admin.ToString(), linkParameter.ListOfValues[1]);
    }

    [TestMethod]
    public void PatchMappingMustAutomaticallyPopulateListOfValuesForNullableEnumerations() {
        //act
        var resource = new Resource()
            .Patch<User>("updateUser", "/api/user")
                .Field(x => x.PositionNullable)
            .EndBody();

        //assert
        var link = resource.GetLink("updateUser");
        Assert.IsNotNull(link);
        var linkParameter = link.GetParameter("positionNullable");
        Assert.IsNotNull(linkParameter);
        Assert.AreEqual(UserPosition.Standard.ToString(), linkParameter.ListOfValues[0]);
        Assert.AreEqual(UserPosition.Admin.ToString(), linkParameter.ListOfValues[1]);
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
        Assert.IsNotNull(link.GetParameter("lastName"));
        Assert.IsNotNull(link.GetParameter("firstName"));
        Assert.IsNotNull(link.GetParameter("position"));
        Assert.IsNotNull(link.GetParameter("yearsEmployed"));
        Assert.IsNotNull(link.GetParameter("position"));
    }

    [TestMethod]
    public void PatchMappingMustSupportMapAllConfiguringASpecificField() {
        //act
        var resource = new Resource()
            .Patch<User>("updateUser", "/api/user")
                .AllFields()
                .Field(x => x.Position, defaultValue: UserPosition.Standard)
            .EndBody();

        //assert
        var link = resource.GetLink("updateUser");
        Assert.IsNotNull(link);
        var positionParameter = link.GetParameter("position");
        Assert.IsNotNull(positionParameter);
        Assert.AreEqual(UserPosition.Standard.ToString(), positionParameter.DefaultValue);
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
        Assert.IsNotNull(link.GetParameter("lastName"));
        Assert.IsNull(link.GetParameter("firstName"));
        Assert.IsNotNull(link.GetParameter("position"));
        Assert.IsNotNull(link.GetParameter("yearsEmployed"));
        Assert.IsNotNull(link.GetParameter("position"));
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
        Assert.IsNotNull(link.GetParameter("lastName"));
        Assert.IsNotNull(link.GetParameter("firstName"));
        Assert.IsNotNull(link.GetParameter("position"));
        Assert.IsNotNull(link.GetParameter("yearsEmployed"));
        Assert.IsNotNull(link.GetParameter("position"));
        Assert.AreEqual("PATCH", link.Verb);
        Assert.IsNotNull(resource.GetLink("getUser"));
    }
}