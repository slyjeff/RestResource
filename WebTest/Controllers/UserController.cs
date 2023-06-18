using Microsoft.AspNetCore.Mvc;
using Slysoft.RestResource;
using Slysoft.RestResource.Extensions;
using WebTest.Stores;

namespace WebTest.Controllers;

[Route("[controller]")]
public sealed class UserController  : ControllerBase {
    [HttpGet]
    public IActionResult GetUsers() {
        var users = UserStore.GetAll();

        var resource = new Resource()
            .Uri("user")
            .MapAllListDataFrom("users", users)
            .Get("getUser", "/user/{userName}", templated: true);

        return StatusCode(200, resource);
    }

    [HttpGet("{userName}")]
    public IActionResult GetUser(string userName) {
        var user = UserStore.GetByUserName(userName);
        if (user == null) {
            return StatusCode(404, $"User {userName} not found.");
        }

        var resource = new Resource()
            .Uri($"/user/{user.Username}")
            .MapAllDataFrom(user);

        return StatusCode(200, resource);
    }
}