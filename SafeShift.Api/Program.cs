using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using SafeShift.Api.Auth;
using SafeShift.Api.Data;
using SafeShift.Api.Domain;

var builder = WebApplication.CreateBuilder(args);

// ---- 1. Register services (things the app can use) ----

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Database: Azure SQL / SQL Server. UseNetTopologySuite enables the geography (GPS) type.
builder.Services.AddDbContext<SafeShiftDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("SafeShift"),
        sql => sql.UseNetTopologySuite()));

// ASP.NET Core Identity: stores users and safely hashes their passwords.
builder.Services.AddIdentityCore<AppUser>()
    .AddEntityFrameworkStores<SafeShiftDbContext>();

// JWT settings. The app refuses to start if the signing key is missing or too short.
builder.Services.AddOptions<JwtOptions>()
    .Bind(builder.Configuration.GetSection(JwtOptions.SectionName))
    .Validate(o => o.Key.Length >= 32, "Jwt:Key must be at least 32 characters.")
    .ValidateOnStart();
builder.Services.AddScoped<TokenService>();

// Every request with "Authorization: Bearer <token>" gets its token checked here.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
builder.Services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
    .Configure<IOptions<JwtOptions>>((bearer, jwtOptions) =>
    {
        var jwt = jwtOptions.Value;
        bearer.MapInboundClaims = false;   // keep claim names as written: "sub", "role"
        bearer.TokenValidationParameters = new()
        {
            ValidIssuer = jwt.Issuer,
            ValidAudience = jwt.Audience,
            IssuerSigningKey = TokenService.SigningKey(jwt.Key),
            NameClaimType = JwtRegisteredClaimNames.Name,
            RoleClaimType = "role",        // makes [Authorize(Roles = "Supervisor")] work
        };
    });
builder.Services.AddAuthorization();

// CORS: allow the React app (a different port/domain) to call this API.
var allowedOrigins = builder.Configuration.GetSection("Cors:Origins").Get<string[]>() ?? [];
builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
    p.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

// ---- 2. The request pipeline (runs in this order for every request) ----

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();   // API description at /openapi/v1.json
}

app.UseCors();
app.UseAuthentication();   // who are you? (reads the token)
app.UseAuthorization();    // are you allowed? (checks [Authorize])

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));
app.MapControllers();

if (app.Configuration.GetValue<bool>("SeedDemoData"))
{
    await DbSeeder.SeedAsync(app.Services);
}

app.Run();
