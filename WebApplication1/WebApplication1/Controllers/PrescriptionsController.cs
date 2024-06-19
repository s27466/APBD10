using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Context;

namespace WebApplication1.Controllers;


[ApiController]
[Route("/api/[controller]")]
public class PrescriptionsController : ControllerBase
{
    private readonly AppDbContext _context;

    public PrescriptionsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetPrescriptionsAsync()
    {
        var prescriptions = await _context.Prescriptions
            .Include(p => p.Doctor) 
            .ToListAsync();

        return Ok(prescriptions);
    }
}
