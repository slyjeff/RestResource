using Microsoft.AspNetCore.Mvc;

namespace WebTest.Controllers;

//[Route("[controller]/[action]")]
[Route("")]
public sealed class ApplicationController  : ControllerBase {
    [HttpGet]
    public IActionResult GetApplication() {
        return StatusCode(200, "Hello World!");
    }
}