using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestUtils;

namespace SlySoft.RestResource.Tests;

[TestClass]
public class PostTests {
    [TestMethod]
    public void PostMustAddLink() {
        //arrange
        const string uri = "/api/user";

        //act
        var resource = new Resource()
            .Post("CreateUser", uri)
            .EndBody();

        //assert
        var link = resource.GetLink("createUser");
        Assert.IsNotNull(link);
        Assert.AreEqual("createUser", link.Name);
        Assert.AreEqual(uri, link.Href);
        Assert.IsFalse(link.Templated);
        Assert.AreEqual("POST", link.Verb);
    }

    [TestMethod]
    public void PostMustAllowForTemplating() {
        //act
        var resource = new Resource()
            .Get("createUser", "/api/user/{userType}", templated: true);

        //assert
        var link = resource.GetLink("createUser");
        Assert.IsNotNull(link);
        Assert.IsTrue(link.Templated);
    }

    [TestMethod]
    public void PostMustAllowForSettingTimeout() {
        //act
        var resource = new Resource()
            .Get("createUser", "/api/user/{userType}", timeout: 60);

        //assert
        var link = resource.GetLink("createUser");
        Assert.IsNotNull(link);
        Assert.AreEqual(60, link.Timeout);
    }

    [TestMethod]
    public void PostMustAllowConfigurationOfField() {
        //act
        var resource = new Resource()
            .Post("createUser", "/api/user")
                .Field("lastName")
                .Field("firstName")
            .EndBody()
            .Get("getUser", "/api/user{id}", templated: true); //just to prove we can do chaining after a post

        //assert
        var link = resource.GetLink("createUser");
        Assert.IsNotNull(link);
        Assert.IsNotNull(link.GetParameter("lastName"));
        Assert.IsNotNull(link.GetParameter("firstName"));
        Assert.IsNotNull(resource.GetLink("getUser"));
    }

    [TestMethod]
    public void PostMustAllowSettingOfDefaultValues() {
        //act
        var resource = new Resource()
            .Post("createUser", "/api/user")
                .Field("position", defaultValue: "admin")
            .EndBody();

        //assert
        var link = resource.GetLink("createUser");
        Assert.IsNotNull(link);
        var linkParameter = link.GetParameter("position");
        Assert.IsNotNull(linkParameter);
        Assert.AreEqual("admin", linkParameter.DefaultValue);
    }

    [TestMethod]
    public void PostMustAllowSettingListOfValues() {
        //act
        var resource = new Resource()
            .Post("createUser", "/api/user")
                .Field("position", listOfValues: new[] { "Standard", "Admin" })
            .EndBody();

        //assert
        var link = resource.GetLink("createUser");
        Assert.IsNotNull(link);
        var linkParameter = link.GetParameter("position");
        Assert.IsNotNull(linkParameter);
        Assert.AreEqual("Standard", linkParameter.ListOfValues[0]);
        Assert.AreEqual("Admin", linkParameter.ListOfValues[1]);
    }

    [TestMethod]
    public void PostMustAllowSettingValueType() {
        //act
        var resource = new Resource()
            .Post("createUser", "/api/user")
                .Field("yearsEmployed", type: "number")
            .EndBody();

        //assert
        var link = resource.GetLink("createUser");
        Assert.IsNotNull(link);
        var linkParameter = link.GetParameter("yearsEmployed");
        Assert.IsNotNull(linkParameter);
        Assert.AreEqual("number", linkParameter.Type);
    }

    [TestMethod]
    public void PostMustAllowMappingConfigurationOfParameters() {
        //act
        var resource = new Resource()
            .Post<User>("createUser", "/api/user")
                .Field(x => x.LastName)
                .Field(x => x.FirstName)
            .EndBody()
            .Get("getUser", "/api/user{id}", templated: true); //just to prove we can do chaining after a query

        //assert
        var link = resource.GetLink("createUser");
        Assert.IsNotNull(link);
        Assert.IsNotNull(link.GetParameter("lastName"));
        Assert.IsNotNull(link.GetParameter("firstName"));
        Assert.AreEqual("POST", link.Verb);
        Assert.IsNotNull(resource.GetLink("getUser"));
    }

    [TestMethod]
    public void PostMappingMustAllowSettingOfDefaultValues() {
        //act
        var resource = new Resource()
            .Post<User>("createUser", "/api/user")
                .Field(x => x.Position, defaultValue: UserPosition.Admin)
            .EndBody();

        //assert
        var link = resource.GetLink("createUser");
        Assert.IsNotNull(link);
        var linkParameter = link.GetParameter("position");
        Assert.IsNotNull(linkParameter);
        Assert.AreEqual("Admin", linkParameter.DefaultValue);
    }

    [TestMethod]
    public void PostMappingMustAllowSettingListOfValues() {
        //act
        var resource = new Resource()
            .Post<User>("createUser", "/api/user")
                .Field(x => x.Position, listOfValues: new[] { UserPosition.Standard, UserPosition.Admin })
            .EndBody();

        //assert
        var link = resource.GetLink("createUser");
        Assert.IsNotNull(link);
        var linkParameter = link.GetParameter("position");
        Assert.IsNotNull(linkParameter);
        Assert.AreEqual("Standard", linkParameter.ListOfValues[0]);
        Assert.AreEqual("Admin", linkParameter.ListOfValues[1]);
    }

    [TestMethod]
    public void PostMappingMustAllowSettingValueType() {
        //act
        var resource = new Resource()
            .Post<User>("createUser", "/api/user")
                .Field(x => x.YearsEmployed, type: "number")
            .EndBody();

        //assert
        var link = resource.GetLink("createUser");
        Assert.IsNotNull(link);
        var linkParameter = link.GetParameter("yearsEmployed");
        Assert.IsNotNull(linkParameter);
        Assert.AreEqual("number", linkParameter.Type);
    }

    [TestMethod]
    public void PostMappingMustAutomaticallyPopulateListOfValuesForBoolean() {
        //act
        var resource = new Resource()
            .Post<User>("createUser", "/api/user")
                .Field(x => x.IsRegistered)
            .EndBody();

        //assert
        var link = resource.GetLink("createUser");
        Assert.IsNotNull(link);
        var linkParameter = link.GetParameter("isRegistered");
        Assert.IsNotNull(linkParameter);
        Assert.AreEqual("True", linkParameter.ListOfValues[0]);
        Assert.AreEqual("False", linkParameter.ListOfValues[1]);
    }

    [TestMethod]
    public void PostMappingMustAutomaticallyPopulateListOfValuesForNullableBoolean() {
        //act
        var resource = new Resource()
            .Post<User>("createUser", "/api/user")
                .Field(x => x.IsRegisteredNullable)
            .EndBody();

        //assert
        var link = resource.GetLink("createUser");
        Assert.IsNotNull(link);
        var linkParameter = link.GetParameter("isRegisteredNullable");
        Assert.IsNotNull(linkParameter);
        Assert.AreEqual("True", linkParameter.ListOfValues[0]);
        Assert.AreEqual("False", linkParameter.ListOfValues[1]);
    }

    [TestMethod]
    public void PostMappingMustAutomaticallyPopulateListOfValuesForEnumerations() {
        //act
        var resource = new Resource()
            .Post<User>("createUser", "/api/user")
                .Field(x => x.Position)
            .EndBody();

        //assert
        var link = resource.GetLink("createUser");
        Assert.IsNotNull(link);
        var linkParameter = link.GetParameter("position");
        Assert.IsNotNull(linkParameter);
        Assert.AreEqual(UserPosition.Standard.ToString(), linkParameter.ListOfValues[0]);
        Assert.AreEqual(UserPosition.Admin.ToString(), linkParameter.ListOfValues[1]);
    }

    [TestMethod]
    public void PostMappingMustAutomaticallyPopulateListOfValuesForNullableEnumerations() {
        //act
        var resource = new Resource()
            .Post<User>("createUser", "/api/user")
                .Field(x => x.PositionNullable)
            .EndBody();

        //assert
        var link = resource.GetLink("createUser");
        Assert.IsNotNull(link);
        var linkParameter = link.GetParameter("positionNullable");
        Assert.IsNotNull(linkParameter);
        Assert.AreEqual(UserPosition.Standard.ToString(), linkParameter.ListOfValues[0]);
        Assert.AreEqual(UserPosition.Admin.ToString(), linkParameter.ListOfValues[1]);
    }

    [TestMethod]
    public void PostMappingMustSupportMapAll() {
        //act
        var resource = new Resource()
            .Post<User>("createUser", "/api/user")
                .AllFields()
            .EndBody();

        //assert
        var link = resource.GetLink("createUser");
        Assert.IsNotNull(link);
        Assert.IsNotNull(link.GetParameter("lastName"));
        Assert.IsNotNull(link.GetParameter("firstName"));
        Assert.IsNotNull(link.GetParameter("position"));
        Assert.IsNotNull(link.GetParameter("yearsEmployed"));
        Assert.IsNotNull(link.GetParameter("position"));
    }

    [TestMethod]
    public void PostMappingMustSupportExcludingAParameter() {
        //act
        var resource = new Resource()
            .Post<User>("createUser", "/api/user")
                .Exclude(x => x.FirstName)
                .AllFields()
            .EndBody();

        //assert
        var link = resource.GetLink("createUser");
        Assert.IsNotNull(link);
        Assert.IsNotNull(link);
        Assert.IsNotNull(link.GetParameter("lastName"));
        Assert.IsNull(link.GetParameter("firstName"));
        Assert.IsNotNull(link.GetParameter("position"));
        Assert.IsNotNull(link.GetParameter("yearsEmployed"));
        Assert.IsNotNull(link.GetParameter("position"));
    }

    [TestMethod]
    public void PostMappingMustSupportMapAllConfiguringASpecificField() {
        //act
        var resource = new Resource()
            .Post<User>("createUser", "/api/user")
                .AllFields()
                .Field(x => x.Position, defaultValue: UserPosition.Standard)
            .EndBody();

        //assert
        var link = resource.GetLink("createUser");
        Assert.IsNotNull(link);
        var positionParameter = link.GetParameter("position");
        Assert.IsNotNull(positionParameter);
        Assert.AreEqual(UserPosition.Standard.ToString(), positionParameter.DefaultValue);
    }

    [TestMethod]
    public void PostWithAllFieldsMustMapAllWithNoConfiguration() {
        //act
        var resource = new Resource()
            .PostWithAllFields<User>("createUser", "/api/user")
            .Get("getUser", "/api/user{id}", templated: true); //just to prove we can do chaining after a query

        //assert
        var link = resource.GetLink("createUser");
        Assert.IsNotNull(link);
        Assert.IsNotNull(link.GetParameter("lastName"));
        Assert.IsNotNull(link.GetParameter("firstName"));
        Assert.IsNotNull(link.GetParameter("position"));
        Assert.IsNotNull(link.GetParameter("yearsEmployed"));
        Assert.IsNotNull(link.GetParameter("position"));
        Assert.AreEqual("POST", link.Verb);
        Assert.IsNotNull(resource.GetLink("getUser"));
    }
}