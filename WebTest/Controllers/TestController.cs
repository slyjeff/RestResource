using Microsoft.AspNetCore.Mvc;
using SlySoft.RestResource;
using System.Text.Json.Serialization;

namespace WebTest.Controllers;

[Route("[controller]")]
public sealed class TestController  : ControllerBase {
    [HttpGet]
    public IActionResult GetTests() {
        var resource = new Resource()
            .Data("description", "Tests used by the ClientTest app.")
            .Get("notFound", "/test/notFound")
            .Get("text", "/test/text")
            .Get("templatedGet", "test/templated/{value1}/{value2}", templated: true)
            .Query("query", "test/query")
                .Parameter("parameter1")
                .Parameter("parameter2")
            .EndQuery()
            .Post("post", "test/post")
                .Field("parameter1")
                .Field("parameter2")
            .EndBody()
            .Delete("delete", "test/delete")
            .Put("list", "test/list")
                .Field("list")
            .EndBody()
            .QueryWithAllParameters<BodyWithBool>("queryBool", "test/queryBool")
            .Post<BodyWithBool>("bool", "test/bool")
                .Field(x => x.Value, defaultValue: false)
            .EndBody()
            .PostWithAllFields<BodyWithInt>("int", "test/int")
            .PostWithAllFields<BodyWithEnum>("enum", "test/enum");

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

    [HttpGet("templated/{value1}/{value2}")]
    public IActionResult TemplatedGet(string value1, string value2) {
        var resource = new Resource()
            .Data("value1", value1)
            .Data("value2", value2);

        return StatusCode(200, resource);
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
        public string Parameter2 { get; set; } = string.Empty;
    }

    [HttpPost("post")]
    public IActionResult Post([FromBody] PostBody body) {
        var resource = new Resource()
            .Data("parameter1", body.Parameter1)
            .Data("parameter2", body.Parameter2);

        return StatusCode(200, resource);
    }

    [HttpDelete("delete")]
    public IActionResult Delete() {
        return StatusCode(200);
    }

    public class ListBody {
        public IList<string> List { get; set; } = new List<string>();
    }

    [HttpPut("list")]
    public IActionResult List([FromBody] ListBody body) {
        var resource = new Resource()
            .Data("list", body.List);

        return StatusCode(200, resource);
    }

    public class BodyWithBool {
        [JsonConverter(typeof(JsonConverter<bool>))]
        public bool Value { get; set; }
    }

    [HttpGet("queryBool")]
    public IActionResult BoolTest([FromQuery] bool value) {
        var resource = new Resource()
            .Data("bool", value);
        return StatusCode(200, resource);
    }

    [HttpPost("bool")]
    public IActionResult BoolTest([FromBody] BodyWithBool? body) {
        if (body == null) {
            return StatusCode(500, "Could not parse value");
        }

        var resource = new Resource()
            .Data("bool", body.Value);

        return StatusCode(200, resource);
    }

    public class BodyWithInt {
        public int Value { get; set; } = 0;
    }


    [HttpPost("int")]
    public IActionResult IntTest([FromBody] BodyWithInt? body) {
        if (body == null) {
            return StatusCode(500, "Could not parse value");
        }

        var resource = new Resource()
            .Data("int", body.Value);

        return StatusCode(200, resource);
    }

    public enum BodyEnum { Value1, Value2 }
    public class BodyWithEnum {
        [JsonConverter(typeof(JsonStringEnumConverter))] 
        public BodyEnum Value { get; set; } = BodyEnum.Value1;
    }

    [HttpPost("enum")]
    public IActionResult EnumTest([FromBody] BodyWithEnum? body) {
        if (body == null) {
            return StatusCode(500, "Could not parse value");
        }
        var resource = new Resource()
            .Data("enum", body.Value);

        return StatusCode(200, resource);
    }
}