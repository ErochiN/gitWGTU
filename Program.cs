using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GitВГТУ
{
    public class Program
    {
        private static string _wwwroot = "wwwroot/";
        private static string _index = "index.html";
        private static string _authLogin = "auth/login.html";
        private static string _authRegister = "auth/register.html";

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<AppDbContext>();
            builder.Services.AddAuthorization();
            builder.Services.AddScoped<IUserService, UserService>();

            builder.Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = true;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = AuthOptions.ISSUER,

                        ValidateAudience = true,
                        ValidAudience = AuthOptions.AUDIENCE,

                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        IssuerSigningKey =
                            AuthOptions.GetSymmetricSecurityKey(),

                        ClockSkew = TimeSpan.FromSeconds(30)
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            context.Token =
                                context.Request.Cookies["auth_token"];

                            return Task.CompletedTask;
                        }
                    };
                });

            builder.Services.AddAuthorization();

            var app = builder.Build();

            app.UseAuthentication();
            app.UseAuthorization();

            var authLoginGroup = app.MapGroup($"/auth/login");
            var authRegisterGroup = app.MapGroup($"/auth/register");
            var profileGroup = app.MapGroup("/app/profile");

            app.MapGet("/api/profile", [Authorize] async (HttpContext context, AppDbContext db) =>
            {
                string? userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

                Console.WriteLine(userId);

                if (!int.TryParse(userId, out int id))
                    return Results.Unauthorized();

               var user = await db.Users.Where(u => u.Id == id).Select(u => new
                {
                    u.FirstName,
                    u.LastName,
                    u.Email,
                   u.Group
                }).FirstOrDefaultAsync();

                    if (user == null)
                        return Results.NotFound();


                return Results.Ok(user);
            });
            app.MapGet("/app/profile", [Authorize] async (HttpContext context) =>
            {
                await SendHtml(context, "app/profile.html");
            });


            // логин
            authLoginGroup.MapGet("/", async (HttpContext context) => { await SendHtml(context, _authLogin); });
            authLoginGroup.MapPost("/", async (HttpContext context, IUserService service) => 
            {
                string Email = context.Request.Form["email"];
                string Password = context.Request.Form["password"];

                if (! await service.IniUser(Email, Password))
                {
                    context.Response.Redirect("/auth/login?error=invalid_credentials");
                    return;
                }

                int id = await service.GetUserId(Email);

                string token = GenerateJwt(id.ToString(), Email);

                context.Response.Cookies.Append
                (
                    "auth_token",
                    token,
                    new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = !app.Environment.IsDevelopment(),
                        SameSite = SameSiteMode.Lax,
                        Expires = DateTimeOffset.UtcNow.AddDays(2),
                        Path = "/"
                    }
                );
                context.Response.Redirect("/app/profile");
            });
            
            // регестрация
            authRegisterGroup.MapGet("/", async (HttpContext context) => { await SendHtml(context, _authRegister); });
            authRegisterGroup.MapPost("/", async (HttpContext context, IUserService service) => 
            {
                string firstName = context.Request.Form["firstName"];
                string lastName = context.Request.Form["lastName"];
                string group = context.Request.Form["group"];
                string email = context.Request.Form["email"];
                string password = context.Request.Form["password"];

                if (await service.RegisterAsync(firstName, lastName, group, email, password))
                    context.Response.Redirect("/");
                else
                    context.Response.Redirect("/auth/register?error=exists");
            });



            app.MapGet("/", async (HttpContext context) => 
            {
                if (context.User.Identity?.IsAuthenticated != true)
                {
                    await SendHtml(context, _index);
                    return;
                }
                else
                {
                    context.Response.Redirect("/app/profile");
                    return;
                }
                
            });
            app.MapGet("/css/{fileName}", SendCss);
            app.Run();
        }

        private static async Task SendHtml(HttpContext context, string fileName)
        {
            context.Response.ContentType = "text/html; charset=utf-8";
            await context.Response.SendFileAsync($"{_wwwroot}{fileName}");
        }
        private static async Task SendCss(HttpContext context, string fileName)
        {
            context.Response.ContentType = "text/css; charset=utf-8";
            await context.Response.SendFileAsync($"{_wwwroot}css/{fileName}");
        }

        private static string GenerateJwt(string id, string email)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, id),
                new Claim(ClaimTypes.Email, email)
            };

            var credentials = new SigningCredentials(
                AuthOptions.GetSymmetricSecurityKey(),
                SecurityAlgorithms.HmacSha256
            );

            var token = new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,
                audience: AuthOptions.AUDIENCE,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
