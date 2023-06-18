using Microsoft.AspNetCore.Mvc;
using Slysoft.RestResource;
using Slysoft.RestResource.Extensions;
using WebTest.Entities;
using WebTest.ResourceGenerators;
using WebTest.Stores;

namespace WebTest.Controllers;

[Route("[controller]")]
public sealed class UserController  : ControllerBase {
    [HttpGet]
    public IActionResult GetUsers() {
        var userResources = UserStore
            .GetAll()
            .Select(x => x.ToResource())
            .ToList();

        var resource = new Resource()
            .Uri("/user")
            .Data("userCount", userResources.Count)
            .Get("getUser", "/user/{id}", templated: true)
            .Embedded("users", userResources);

        return StatusCode(200, resource);
    }

    [HttpGet("{id}")]
    public IActionResult GetUser(int id) {
        var user = UserStore.Get(id);
        if (user == null) {
            return StatusCode(404, $"User with id {id} not found.");
        }

        return StatusCode(200, user.ToResource());
    }
}