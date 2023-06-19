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
            .Select(x => x.ToListItemResource())
            .ToList();

        var resource = new Resource()
            .Uri("/user")
            .Data("userCount", userResources.Count)
            .Get("getUser", "/user/{id}", templated: true)
            .Post<CreateUserData>(nameof(CreateUser), "/user")
                .AllFields()
            .EndBody()
            .Embedded("users", userResources);

        return StatusCode(200, resource);
    }

    public class CreateUserData {
        public string Username { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
    }

    [HttpPost]
    public IActionResult CreateUser([FromBody] CreateUserData data) {
        var user = new User {
            Username = data.Username,
            LastName = data.LastName,
            FirstName = data.FirstName
        };

        return UserStore.Add(user)
            ? Redirect($"/user/{user.Id}")
            : StatusCode(403, "Validation failure.");
    }

    [HttpGet("{id}")]
    public IActionResult GetUser(int id) {
        var user = UserStore.Get(id);
        if (user == null) {
            return StatusCode(404, $"User with id {id} not found.");
        }

        return StatusCode(200, user.ToResource());
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteUser(int id) {
        UserStore.Delete(id);

        return Redirect("/user");
    }

    [HttpPut("{id}/role/{role}")]
    public IActionResult AddRole(int id, string role) {
        if (!Enum.TryParse<UserRole>(role, out var roleToAdd)) {
            return StatusCode(403, $"{role} is not a valid role.");
        }

        var user = UserStore.Get(id);
        if (user == null) {
            return StatusCode(404, $"User with id {id} not found.");
        }

        if (!user.Roles.Contains(roleToAdd)) {
            user.Roles.Add(roleToAdd);
        }

        return Redirect($"/user/{user.Id}");
    }

    [HttpDelete("{id}/role/{role}")]
    public IActionResult RemoveRole(int id, string role) {
        if (!Enum.TryParse<UserRole>(role, out var roleToAdd)) {
            return StatusCode(403, $"{role} is not a valid role.");
        }

        var user = UserStore.Get(id);
        if (user == null) {
            return StatusCode(404, $"User with id {id} not found.");
        }

        if (user.Roles.Contains(roleToAdd)) {
            user.Roles.Remove(roleToAdd);
        }

        return Redirect($"/user/{user.Id}");
    }
}