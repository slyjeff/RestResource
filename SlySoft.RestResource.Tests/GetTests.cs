using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestUtils;

namespace SlySoft.RestResource.Tests;

[TestClass]
public class GetTests {
    [TestMethod]
    public void GetMustAddLink() {
        //arrange
        const string uri = "/api/user";

        //act
        var resource = new Resource()
            .Get("GetUsers", uri);

        //assert
        var link = resource.GetLink("getUsers");
        Assert.IsNotNull(link);
        Assert.AreEqual("getUsers", link.Name);
        Assert.AreEqual(uri, link.Href);
        Assert.IsFalse(link.Templated);
        Assert.AreEqual("GET", link.Verb);
    }

    [TestMethod]
    public void GetMustAllowForTemplating() {
        //act
        var resource = new Resource()
            .Get("getUser", "/api/user/{id}", templated: true);

        //assert
        var link = resource.GetLink("getUser");
        Assert.IsNotNull(link);
        Assert.IsTrue(link.Templated);
    }

    [TestMethod]
    public void GetMustAllowForSettingATimeout() {
        //act
        var resource = new Resource()
            .Get("getUser", "/api/user/{id}", timeout: 60);

        //assert
        var link = resource.GetLink("getUser");
        Assert.IsNotNull(link);
        Assert.AreEqual(link.Timeout, 60);
    }

    [TestMethod]
    public void QueryMustAddLink() {
        //arrange
        const string uri = "/api/user";

        //act
        var resource = new Resource()
            .Query("search", uri)
            .EndQuery();

        //assert
        var link = resource.GetLink("search");
        Assert.IsNotNull(link);
        Assert.AreEqual(uri, link.Href);
        Assert.IsFalse(link.Templated);
        Assert.AreEqual("GET", link.Verb);
    }

    [TestMethod]
    public void QueryMustAllowForTemplating() {
        //act
        var resource = new Resource()
            .Query("searchUserByType", "/api/user/{type}", templated: true)
            .EndQuery();

        //assert
        var link = resource.GetLink("searchUserByType");
        Assert.IsNotNull(link);
        Assert.IsTrue(link.Templated);
    }

    [TestMethod]
    public void QueryMustAllowForSettingTimeout() {
        //act
        var resource = new Resource()
            .Query("searchUserByType", "/api/user/{type}", timeout: 60)
            .EndQuery();

        //assert
        var link = resource.GetLink("searchUserByType");
        Assert.IsNotNull(link);
        Assert.AreEqual(60, link.Timeout);
    }

    [TestMethod]
    public void QueryMustAllowConfigurationOfParameters() {
        //act
        var resource = new Resource()
            .Query("search", "/api/user")
                .Parameter("lastName")
                .Parameter("firstName")
            .EndQuery()
            .Get("getUser", "/api/user{id}", templated: true); //just to prove we can do chaining after a query

        //assert
        var link = resource.GetLink("search");
        Assert.IsNotNull(link);
        Assert.IsNotNull(link.GetParameter("lastName"));
        Assert.IsNotNull(link.GetParameter("firstName"));
        Assert.IsNotNull(resource.GetLink("getUser"));
    }

    [TestMethod]
    public void QueryMustAllowSettingOfDefaultValues() {
        //act
        var resource = new Resource()
            .Query("search", "/api/user")
                .Parameter("position", defaultValue: "admin")
            .EndQuery();

        //assert
        var link = resource.GetLink("search");
        Assert.IsNotNull(link);
        var queryParameter = link.GetParameter("position");
        Assert.IsNotNull(queryParameter);
        Assert.AreEqual("admin", queryParameter.DefaultValue);
    }

    [TestMethod]
    public void QueryMustAllowSettingListOfValues() {
        //act
        var resource = new Resource()
            .Query("search", "/api/user")
                .Parameter("position", listOfValues: new[] { "Standard", "Admin" })
            .EndQuery();

        //assert
        var link = resource.GetLink("search");
        Assert.IsNotNull(link);
        var queryParameter = link.GetParameter("position");
        Assert.IsNotNull(queryParameter);
        Assert.AreEqual("Standard", queryParameter.ListOfValues[0]);
        Assert.AreEqual("Admin", queryParameter.ListOfValues[1]);
    }

    private enum PositionEnum {
        // ReSharper disable once UnusedMember.Local
        Standard,
        // ReSharper disable once UnusedMember.Local
        Admin
    };

    [TestMethod]
    public void MustBeAbleSetListOfValuesFromEnumeration() {
        //act
        var resource = new Resource()
            .Query("search", "/api/user")
                .Parameter("position", listOfValues: new ListOfValues<PositionEnum>())
            .EndQuery();

        //assert
        var link = resource.GetLink("search");
        Assert.IsNotNull(link);
        var queryParameter = link.GetParameter("position");
        Assert.IsNotNull(queryParameter);
        Assert.AreEqual("Standard", queryParameter.ListOfValues[0]);
        Assert.AreEqual("Admin", queryParameter.ListOfValues[1]);
    }

    [TestMethod]
    public void QueryMustAllowSettingValueType() {
        //act
        var resource = new Resource()
            .Query("search", "/api/user")
                .Parameter("yearsEmployed", type: "number")
            .EndQuery();

        //assert
        var link = resource.GetLink("search");
        Assert.IsNotNull(link);
        var queryParameter = link.GetParameter("yearsEmployed");
        Assert.IsNotNull(queryParameter);
        Assert.AreEqual("number", queryParameter.Type);
    }

    [TestMethod]
    public void QueryMustAllowMappingConfigurationOfParameters() {
        //act
        var resource = new Resource()
            .Query<User>("search", "/api/user")
                .Parameter(x => x.LastName)
                .Parameter(x => x.FirstName)
            .EndQuery()
            .Get("getUser", "/api/user{id}", templated: true); //just to prove we can do chaining after a query

        //assert
        var link = resource.GetLink("search");
        Assert.IsNotNull(link);
        Assert.IsNotNull(link.GetParameter("lastName"));
        Assert.IsNotNull(link.GetParameter("firstName"));
        Assert.IsNotNull(resource.GetLink("getUser"));
    }

    [TestMethod]
    public void QueryMappingMustAllowSettingOfDefaultValues() {
        //act
        var resource = new Resource()
            .Query<User>("search", "/api/user")
                .Parameter(x => x.Position, defaultValue: UserPosition.Standard)
            .EndQuery();

        //assert
        var link = resource.GetLink("search");
        Assert.IsNotNull(link);
        var queryParameter = link.GetParameter("position");
        Assert.IsNotNull(queryParameter);
        Assert.AreEqual("Standard", queryParameter.DefaultValue);
    }

    [TestMethod]
    public void QueryMappingMustAllowSettingListOfValues() {
        //act
        var resource = new Resource()
            .Query<User>("search", "/api/user")
                .Parameter(x => x.Position, listOfValues: new[] { UserPosition.Standard, UserPosition.Admin })
            .EndQuery();

        //assert
        var link = resource.GetLink("search");
        Assert.IsNotNull(link);
        var queryParameter = link.GetParameter("position");
        Assert.IsNotNull(queryParameter);
        Assert.AreEqual("Standard", queryParameter.ListOfValues[0]);
        Assert.AreEqual("Admin", queryParameter.ListOfValues[1]);
    }

    [TestMethod]
    public void QueryMappingMustAllowSettingValueType() {
        //act
        var resource = new Resource()
            .Query<User>("search", "/api/user")
                .Parameter(x => x.YearsEmployed, type: "number")
            .EndQuery();

        //assert
        var link = resource.GetLink("search");
        Assert.IsNotNull(link);
        var queryParameter = link.GetParameter("yearsEmployed");
        Assert.IsNotNull(queryParameter);
        Assert.AreEqual("number", queryParameter.Type);
    }

    [TestMethod]
    public void QueryMappingMustAutomaticallyPopulateListOfValuesForBoolean() {
        //act
        var resource = new Resource()
            .Query<User>("search", "/api/user")
                .Parameter(x => x.IsRegistered)
            .EndQuery();

        //assert
        var link = resource.GetLink("search");
        Assert.IsNotNull(link);
        var queryParameter = link.GetParameter("isRegistered");
        Assert.IsNotNull(queryParameter);
        Assert.AreEqual("True", queryParameter.ListOfValues[0]);
        Assert.AreEqual("False", queryParameter.ListOfValues[1]);
    }

    [TestMethod]
    public void QueryMappingMustAutomaticallyPopulateListOfValuesForNullableBoolean() {
        //act
        var resource = new Resource()
            .Query<User>("search", "/api/user")
                .Parameter(x => x.IsRegisteredNullable)
            .EndQuery();

        //assert
        var link = resource.GetLink("search");
        Assert.IsNotNull(link);
        var queryParameter = link.GetParameter("isRegisteredNullable");
        Assert.IsNotNull(queryParameter);
        Assert.AreEqual("True", queryParameter.ListOfValues[0]);
        Assert.AreEqual("False", queryParameter.ListOfValues[1]);
    }

    [TestMethod]
    public void QueryMappingMustAutomaticallyPopulateListOfValuesForEnumerations() {
        //act
        var resource = new Resource()
            .Query<User>("search", "/api/user")
                .Parameter(x => x.Position)
            .EndQuery();

        //assert
        var link = resource.GetLink("search");
        Assert.IsNotNull(link);
        var queryParameter = link.GetParameter("position");
        Assert.IsNotNull(queryParameter);
        Assert.AreEqual(UserPosition.Standard.ToString(), queryParameter.ListOfValues[0]);
        Assert.AreEqual(UserPosition.Admin.ToString(), queryParameter.ListOfValues[1]);
    }

    [TestMethod]
    public void PatchMappingMustAutomaticallyPopulateListOfValuesForNullableEnumerations() {
        //act
        var resource = new Resource()
            .Query<User>("search", "/api/user")
                .Parameter(x => x.PositionNullable)
            .EndQuery();

        //assert
        var link = resource.GetLink("search");
        Assert.IsNotNull(link);
        var queryParameter = link.GetParameter("positionNullable");
        Assert.IsNotNull(queryParameter);
        Assert.AreEqual(UserPosition.Standard.ToString(), queryParameter.ListOfValues[0]);
        Assert.AreEqual(UserPosition.Admin.ToString(), queryParameter.ListOfValues[1]);
    }

    [TestMethod]
    public void QueryMappingMustSupportMapAll() {
        //act
        var resource = new Resource()
            .Query<User>("search", "/api/user")
                .AllParameters()
            .EndQuery();

        //assert
        var link = resource.GetLink("search");
        Assert.IsNotNull(link);
        Assert.IsNotNull(link.GetParameter("lastName"));
        Assert.IsNotNull(link.GetParameter("firstName"));
        Assert.IsNotNull(link.GetParameter("position"));
        Assert.IsNotNull(link.GetParameter("yearsEmployed"));
        Assert.IsNotNull(link.GetParameter("position"));
    }

    [TestMethod]
    public void QueryMappingMustSupportMapAllConfiguringASpecificField() {
        //act
        var resource = new Resource()
            .Query<User>("search", "/api/user")
                .AllParameters()
                .Parameter(x => x.Position, defaultValue: UserPosition.Standard)
            .EndQuery();

        //assert
        var link = resource.GetLink("search");
        Assert.IsNotNull(link);
        var positionParameter = link.GetParameter("position");
        Assert.IsNotNull(positionParameter);
        Assert.AreEqual(UserPosition.Standard.ToString(), positionParameter.DefaultValue);
    }

    [TestMethod]
    public void QueryMappingMustSupportExcludingAParameter() {
        //act
        var resource = new Resource()
            .Query<User>("search", "/api/user")
                .Exclude(x => x.FirstName)
                .AllParameters()
            .EndQuery();

        //assert
        var link = resource.GetLink("search");
        Assert.IsNotNull(link);
        Assert.IsNotNull(link);
        Assert.IsNotNull(link.GetParameter("lastName"));
        Assert.IsNull(link.GetParameter("firstName"));
        Assert.IsNotNull(link.GetParameter("position"));
        Assert.IsNotNull(link.GetParameter("yearsEmployed"));
        Assert.IsNotNull(link.GetParameter("position"));
    }

    [TestMethod]
    public void QueryWithAllParametersMustMapAllWithNoConfiguration() {
        //act
        var resource = new Resource()
            .QueryWithAllParameters<User>("search", "/api/user")
            .Get("getUser", "/api/user{id}", templated: true); //just to prove we can do chaining after a query

        //assert
        var link = resource.GetLink("search");
        Assert.IsNotNull(link);
        Assert.IsNotNull(link.GetParameter("lastName"));
        Assert.IsNotNull(link.GetParameter("firstName"));
        Assert.IsNotNull(link.GetParameter("position"));
        Assert.IsNotNull(link.GetParameter("yearsEmployed"));
        Assert.IsNotNull(link.GetParameter("position"));
        Assert.IsNotNull(resource.GetLink("getUser"));
    }
}