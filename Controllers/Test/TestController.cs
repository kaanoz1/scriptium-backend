using Microsoft.AspNetCore.Mvc;

namespace ScriptiumBackend.Controllers.Test;

[ApiController, Route("test")] 
public class TestController: ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("Hi!");
    }
}