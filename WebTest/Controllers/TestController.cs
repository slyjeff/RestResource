using Microsoft.AspNetCore.Mvc;
using Slysoft.RestResource;
using Slysoft.RestResource.Extensions;

namespace WebTest.Controllers;

[Route("[controller]")]
public sealed class TestController  : ControllerBase {
    [HttpGet]
    public IActionResult GetApplication() {
        var resource = new Resource()
            .Data("description", "Tests used by the ClientTest app.");
            

        return StatusCode(200, resource);
    }
}