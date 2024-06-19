using Microsoft.AspNetCore.Mvc;
using WebApplication1.Context;
using WebApplication1.Models;

namespace WebApplication1.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DoctorsController : ControllerBase
{
    private readonly AppDbContext _context;

    public DoctorsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult GetDoctors()
    {
        return Ok(_context.Doctors.ToList());
    }

    [HttpPost]
    public IActionResult AddDoctor([FromBody] Doctor doctor)
    {
        _context.Doctors.Add(doctor);
        _context.SaveChanges();
        return CreatedAtAction(nameof(GetDoctor), new { id = doctor.Id }, doctor);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateDoctor(int id, [FromBody] Doctor doctor)
    {
        var existingDoctor = _context.Doctors.Find(id);
        if (existingDoctor == null)
            return NotFound();

        existingDoctor.Name = doctor.Name;
        existingDoctor.Specialty = doctor.Specialty;
        _context.SaveChanges();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteDoctor(int id)
    {
        var doctor = _context.Doctors.Find(id);
        if (doctor == null)
            return NotFound();

        _context.Doctors.Remove(doctor);
        _context.SaveChanges();

        return NoContent();
    }

    [HttpGet("{id}")]
    public IActionResult GetDoctor(int id)
    {
        var doctor = _context.Doctors.Find(id);
        if (doctor == null)
            return NotFound();

        return Ok(doctor);
    }
}