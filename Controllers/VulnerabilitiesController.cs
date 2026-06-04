using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VulnTrack.Data;
using VulnTrack.Dtos;
using VulnTrack.Models;
using VulnTrack.Services;

namespace VulnTrack.Controllers;

[ApiController]
[Route("api/vulnerabilities")]
[Authorize]   // every endpoint here requires a valid JWT
public class VulnerabilitiesController : ControllerBase
{
    private readonly AppDbContext _db;

    public VulnerabilitiesController(AppDbContext db)
    {
        _db = db;
    }

    // GET /api/vulnerabilities  — any authenticated user can read
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var vulns = await _db.Vulnerabilities
            .OrderByDescending(v => v.CvssScore)
            .ToListAsync();
        return Ok(vulns);
    }

    // GET /api/vulnerabilities/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var vuln = await _db.Vulnerabilities.FindAsync(id);
        return vuln is null ? NotFound() : Ok(vuln);
    }

    // POST /api/vulnerabilities  — only Analysts and Admins may create
    [HttpPost]
    [Authorize(Roles = "Analyst,Admin")]
    public async Task<IActionResult> Create(CreateVulnerabilityDto dto)
    {
        // Derive severity + SLA from the score using our service.
        var severity = CvssService.GetSeverity(dto.CvssScore);

        var vuln = new Vulnerability
        {
            Title = dto.Title,
            Description = dto.Description,
            CvssScore = dto.CvssScore,
            Severity = severity,
            AssignedTo = dto.AssignedTo,
            SlaDueDate = CvssService.GetSlaDueDate(severity)
        };

        _db.Vulnerabilities.Add(vuln);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = vuln.Id }, vuln);
    }

    // PUT /api/vulnerabilities/5  — update status / owner
    [HttpPut("{id}")]
    [Authorize(Roles = "Analyst,Admin")]
    public async Task<IActionResult> Update(int id, CreateVulnerabilityDto dto)
    {
        var vuln = await _db.Vulnerabilities.FindAsync(id);
        if (vuln is null) return NotFound();

        vuln.Title = dto.Title;
        vuln.Description = dto.Description;
        vuln.CvssScore = dto.CvssScore;
        vuln.Severity = CvssService.GetSeverity(dto.CvssScore);
        vuln.SlaDueDate = CvssService.GetSlaDueDate(vuln.Severity);
        vuln.AssignedTo = dto.AssignedTo;

        await _db.SaveChangesAsync();
        return Ok(vuln);
    }

    // DELETE /api/vulnerabilities/5  — only Admins may delete
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var vuln = await _db.Vulnerabilities.FindAsync(id);
        if (vuln is null) return NotFound();

        _db.Vulnerabilities.Remove(vuln);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
