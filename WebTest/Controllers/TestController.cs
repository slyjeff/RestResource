using Microsoft.AspNetCore.Mvc;
using Slysoft.RestResource;
using Slysoft.RestResource.Extensions;

namespace WebTest.Controllers;

[Route("[controller]")]
public sealed class TestController  : ControllerBase {
    [HttpGet]
    public IActionResult GetTests() {
        var resource = new Resource()
            .Data("description", "Tests used by the ClientTest app.")
            .Get("notFound", "/test/notFound")
            .Get("text", "/test/text");

        return StatusCode(200, resource);
    }

    [HttpGet("notFound")]
    public IActionResult NotFoundTest() {
        return StatusCode(404, "Resource not found.");
    }

    [HttpGet("text")]
    public IActionResult Text() {
        return StatusCode(200, "Non-Resource text.");
    }
}