using Microsoft.AspNetCore.Mvc;
using MyApp.Domain;
using MyApp.Infrastructure;

namespace MyApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    private readonly IUnitOfWorkFactory _uowFactory;

    public TestController(IUnitOfWorkFactory uowFactory)
    {
        _uowFactory = uowFactory;
    }

    [HttpGet("db")]
    public IActionResult TestDbConnection()
    {
        using var uow = _uowFactory.Create();

        try
        {
            var users = uow.Session.Query<UsersCredentials>().ToList();

            return Ok(new
            {
                Count = users.Count
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}
