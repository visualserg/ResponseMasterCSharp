using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductsController : ControllerBase
{
    [HttpGet]
    public ActionResult Get()
    {
        return Ok(new ResponseModel<string>()
        {
            IsSuccess = true,
            Result = "Response"
        });
    }

    [HttpDelete]
    public ActionResult Delete()
    {
        throw new Exception("Example exception");
    }

    [HttpPost]
    public ActionResult Create([FromBody] Product product)
    {
        return Ok();
    }
}