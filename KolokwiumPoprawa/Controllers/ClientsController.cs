using KolokwiumPoprawa.Models;
using KolokwiumPoprawa.Services;
using Microsoft.AspNetCore.Mvc;

namespace KolokwiumPoprawa.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientsController : ControllerBase
{
    public readonly IDbService _dbService;

    public ClientsController(IDbService dbService)
    {
        _dbService = dbService;
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetDelivery(int id)
    {
        var client = await _dbService.GetClient(id);
        if (client is null)
            return NotFound();

        return Ok(client);
    }
    
    [HttpPost]
    public async Task<IActionResult> AddDelivery([FromBody] CreateClient request)
    {
        var error = await _dbService.AddClientCarAsync(request);
        if (error is not null)
            return BadRequest(new { error });
    
        return Ok("Client added");
    }
}