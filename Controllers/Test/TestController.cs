using Microsoft.AspNetCore.Mvc;

namespace scriptium_backend.Controllers.Test;

[ApiController, Route("test")] 
public class TestController: ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("Hi!");
    }
}