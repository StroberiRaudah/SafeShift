using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using SafeShift.Api.Data;

namespace SafeShift.Api.Incidents;

[ApiController]
[Route("api/incidents")]                          // blank 1: the URL, e.g. /api/incidents
[Authorize(Roles = "Worker")]                  // blank 2: only this role can report
public class IncidentsController(SafeShiftDbContext db) : ControllerBase   // blank 3: same as AuthController
{
    // POST /api/incidents
    [HttpPost]                               // blank 4: Get or Post?
    public async Task<IActionResult> Create(CreateIncidentRequest request)
    {
        //the reporting
        var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        var user = await db.Users.Include(u => u.Site).FirstOrDefaultAsync(u => u.Id == userId);
        if (user is null || user.SiteId is null)
            return BadRequest(new { message = "You are not assigned to a site." });
        return Ok($"Hello {user.FullName};");

    }
}