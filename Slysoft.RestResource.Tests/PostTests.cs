﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Slysoft.RestResource.Extensions;
using TestUtils;

namespace Slysoft.RestResource.Tests; 

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
        Assert.IsNotNull(link.GetInputItem("lastName"));
        Assert.IsNotNull(link.GetInputItem("firstName"));
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
        var queryParameter = link.GetInputItem("position");
        Assert.IsNotNull(queryParameter);
        Assert.AreEqual("admin", queryParameter.DefaultValue);
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
        var queryParameter = link.GetInputItem("position");
        Assert.IsNotNull(queryParameter);
        Assert.AreEqual("Standard", queryParameter.ListOfValues[0]);
        Assert.AreEqual("Admin", queryParameter.ListOfValues[1]);
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
        var queryParameter = link.GetInputItem("yearsEmployed");
        Assert.IsNotNull(queryParameter);
        Assert.AreEqual("number", queryParameter.Type);
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
        Assert.IsNotNull(link.GetInputItem("lastName"));
        Assert.IsNotNull(link.GetInputItem("firstName"));
        Assert.AreEqual("POST", link.Verb);
        Assert.IsNotNull(resource.GetLink("getUser"));
    }

    [TestMethod]
    public void PostMappingMustAllowSettingOfDefaultValues() {
        //act
        var resource = new Resource()
            .Post<User>("createUser", "/api/user")
                .Field(x => x.Position, defaultValue: "admin")
            .EndBody();

        //assert
        var link = resource.GetLink("createUser");
        Assert.IsNotNull(link);
        var queryParameter = link.GetInputItem("position");
        Assert.IsNotNull(queryParameter);
        Assert.AreEqual("admin", queryParameter.DefaultValue);
    }

    [TestMethod]
    public void PostMappingMustAllowSettingListOfValues() {
        //act
        var resource = new Resource()
            .Post<User>("createUser", "/api/user")
                .Field(x => x.Position, listOfValues: new[] { "Standard", "Admin" })
            .EndBody();

        //assert
        var link = resource.GetLink("createUser");
        Assert.IsNotNull(link);
        var queryParameter = link.GetInputItem("position");
        Assert.IsNotNull(queryParameter);
        Assert.AreEqual("Standard", queryParameter.ListOfValues[0]);
        Assert.AreEqual("Admin", queryParameter.ListOfValues[1]);
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
        var queryParameter = link.GetInputItem("yearsEmployed");
        Assert.IsNotNull(queryParameter);
        Assert.AreEqual("number", queryParameter.Type);
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
        var queryParameter = link.GetInputItem("isRegistered");
        Assert.IsNotNull(queryParameter);
        Assert.AreEqual("True", queryParameter.ListOfValues[0]);
        Assert.AreEqual("False", queryParameter.ListOfValues[1]);
    }

    [TestMethod]
    public void PostMappingMustSupportMapAll() {
        //act
        var resource = new Resource()
            .Post<User>("createUser", "/api/user")
                .MapAll()
            .EndBody();

        //assert
        var link = resource.GetLink("createUser");
        Assert.IsNotNull(link);
        Assert.IsNotNull(link.GetInputItem("lastName"));
        Assert.IsNotNull(link.GetInputItem("firstName"));
        Assert.IsNotNull(link.GetInputItem("position"));
        Assert.IsNotNull(link.GetInputItem("yearsEmployed"));
        Assert.IsNotNull(link.GetInputItem("position"));
    }

    [TestMethod]
    public void PostMappingMustSupportExcludingAParameter() {
        //act
        var resource = new Resource()
            .Post<User>("createUser", "/api/user")
                .Exclude(x => x.FirstName)
                .MapAll()
            .EndBody();

        //assert
        var link = resource.GetLink("createUser");
        Assert.IsNotNull(link);
        Assert.IsNotNull(link);
        Assert.IsNotNull(link.GetInputItem("lastName"));
        Assert.IsNull(link.GetInputItem("firstName"));
        Assert.IsNotNull(link.GetInputItem("position"));
        Assert.IsNotNull(link.GetInputItem("yearsEmployed"));
        Assert.IsNotNull(link.GetInputItem("position"));
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
        Assert.IsNotNull(link.GetInputItem("lastName"));
        Assert.IsNotNull(link.GetInputItem("firstName"));
        Assert.IsNotNull(link.GetInputItem("position"));
        Assert.IsNotNull(link.GetInputItem("yearsEmployed"));
        Assert.IsNotNull(link.GetInputItem("position"));
        Assert.AreEqual("POST", link.Verb);
        Assert.IsNotNull(resource.GetLink("getUser"));
    }
}