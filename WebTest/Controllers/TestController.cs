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
            .Get("text", "/test/text")
            .Query("query", "test/query")
                .Parameter("parameter1")
                .Parameter("parameter2")
            .EndQuery()
            .Post("post", "test/post")
                .Field("parameter1")
                .Field("parameter2")
            .EndBody();

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

    [HttpGet("query")]
    public IActionResult Query([FromQuery] string parameter1, [FromQuery] string parameter2) {
        var resource = new Resource()
            .Data("parameter1", parameter1)
            .Data("parameter2", parameter2);

        return StatusCode(200, resource);
    }

    public class PostBody {
        public string Parameter1 { get; set; } = string.Empty;
        public int Parameter2 { get; set; }
    }

    [HttpPost("post")]
    public IActionResult Post([FromBody] PostBody body) {
        var resource = new Resource()
            .Data("parameter1", body.Parameter1)
            .Data("parameter2", body.Parameter2);

        return StatusCode(200, resource);
    }
}