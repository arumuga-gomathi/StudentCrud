using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using StudentCrud.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StudentCrud.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AccountController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public IActionResult Login() => View();

        public IActionResult Register() => View();

        [HttpPost]
        public IActionResult Register(Student model)
        {
            if (ModelState.IsValid)
            {
                // ✅ Hash password before saving
                model.Password = BCrypt.Net.BCrypt.HashPassword(model.Password);
                _context.Students.Add(model);
                _context.SaveChanges();
                return RedirectToAction("Login");
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var student = _context.Students.FirstOrDefault(x => x.Email == model.Email);
                if (student != null && BCrypt.Net.BCrypt.Verify(model.Password, student.Password))
                {
                    var token = GenerateJwtToken(student.Email);

                    // ✅ Store JWT in Cookie
                    Response.Cookies.Append("jwt", token, new CookieOptions
                    {
                        HttpOnly = true,          // ✅ safer against XSS
                        Secure = true,            // ✅ works with HTTPS/IIS
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTime.UtcNow.AddMinutes(30)  // match ExpiryInMinutes
                    });

                    return RedirectToAction("Index", "Students");
                }
                ModelState.AddModelError("", "Invalid credentials");
            }
            return View(model);
        }

        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt");
            return RedirectToAction("Login");
        }

        private string GenerateJwtToken(string email)
        {
            // ✅ Read JWT Config
            var jwtSection = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Email, email)
            };

            var token = new JwtSecurityToken(
                issuer: jwtSection["Issuer"],
                audience: jwtSection["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSection["ExpiryInMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
