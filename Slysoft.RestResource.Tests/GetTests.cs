﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Slysoft.RestResource.Extensions;

namespace Slysoft.RestResource.Tests; 

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
        Assert.IsNotNull(link.GetInputSpec("lastName"));
        Assert.IsNotNull(link.GetInputSpec("firstName"));
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
        var queryParameter = link.GetInputSpec("position");
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
        var queryParameter = link.GetInputSpec("position");
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
        var queryParameter = link.GetInputSpec("yearsEmployed");
        Assert.IsNotNull(queryParameter);
        Assert.AreEqual("number", queryParameter.Type);
    }
}