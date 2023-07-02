using Microsoft.AspNetCore.Mvc;
using Slysoft.RestResource;
using Slysoft.RestResource.Extensions;

namespace WebTest.Controllers;

[Route("")]
public sealed class ApplicationController  : ControllerBase {
    [HttpGet]
    public IActionResult GetApplication() {
        var resource = new Resource()
            .Uri("/")
            .Data("information", "This is a test web service for demonstrating how to use Slysoft.RestResource and related libraries.")
            .Data("hateoas", "Hypermedia as the engine of application state (HATEOAS) is essentially embedding links in resources so endpoints are browseable")
            .Data("hal", "This library is based on HAL, bot the json and xml variations, but should be flexible enough to use any format.")
            .Data("acceptHeader", "Change how this data is retrieved by changing the accept header in the request: text/html, application/json, application/xml")
            .Get("getTests", "/test");

        return StatusCode(200, resource);
    }
}