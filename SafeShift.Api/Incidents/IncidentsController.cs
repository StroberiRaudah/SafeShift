using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using SafeShift.Api.Data;
using SafeShift.Api.Domain;

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
        
        
        //B2 is the report already sent? then the phone retry brings the same ClientId, so we must not create a second incident. We return the existing incident's Id and ReferenceNo, and a flag alreadySaved=true.
        //var existing = make box exisitng hold what we find. what find is 
        //await is = wait for db to answer. table is Incidents from db.incidents
        //FirstOrDefaultAsync=method  find the 1st rec matches the condition its from  EF core.
        var existing = await db.Incidents.FirstOrDefaultAsync(i=> i.ClientId == request.ClientId);
        if(existing is not null){
            return Ok(new { existing.Id, existing.ReferenceNo, alreadySaved=true});
        }

        var incident = new Incident
        {
            ClientId = request.ClientId,
            Type = request.Type,
            Title = request.Title,
            Description = request.Description,
            Severity = request.Severity,
            AnyoneHurt = request.AnyoneHurt,
            AreaMadeSafe = request.AreaMadeSafe,
            IsAnonymous = request.IsAnonymous,
            ReportedAt = request.ReportedAt,
            SyncedAt = DateTimeOffset.UtcNow,
            ReportedById = user.Id,
            SiteId = user.SiteId.Value
        };
        var count = await  db.Incidents.CountAsync();
        incident.ReferenceNo = $"SS-{DateTime.UtcNow:yyyy}-{count+1:D4}";

        db.Incidents.Add(incident);
        await db.SaveChangesAsync();

        return Ok(new { incident.Id, incident.ReferenceNo, alreadySaved=false});
           }
}
