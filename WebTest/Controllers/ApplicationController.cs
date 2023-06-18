using Microsoft.AspNetCore.Mvc;
using Slysoft.RestResource;
using Slysoft.RestResource.Extensions;

namespace WebTest.Controllers;

//[Route("[controller]/[action]")]
[Route("")]
public sealed class ApplicationController  : ControllerBase {
    [HttpGet]
    public IActionResult GetApplication() {
        var resource = new Resource()
            .Uri("/")
            .Data("message", "Hello World!");

        return StatusCode(200, resource);
    }
}