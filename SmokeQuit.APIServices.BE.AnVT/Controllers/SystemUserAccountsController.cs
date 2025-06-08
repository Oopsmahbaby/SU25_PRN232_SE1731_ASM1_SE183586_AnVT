using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SmokeQuit.Repositories.AnVT.Models;
using SmokeQuit.Services.AnVT;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SmokeQuit.APIServices.BE.AnVT.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class SystemUserAccountsController : ControllerBase
	{
		private readonly IConfiguration _config;
		private readonly SystemUserAccountService _userAccountsService;

		public SystemUserAccountsController(IConfiguration config, SystemUserAccountService userAccountsService)
		{
			_config = config;
			_userAccountsService = userAccountsService;
		}

		[HttpPost("Login")]
		public IActionResult Login([FromBody] LoginRequest request)
		{
			var user = _userAccountsService.GetUserAccountAsync(request.UserName, request.Password);

			if (user == null || user.Result == null)
				return Unauthorized();

			var token = GenerateJSONWebToken(user.Result);

			return Ok(token);
		}

		private string GenerateJSONWebToken(SystemUserAccount systemUserAccount)
		{
			var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
			var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(_config["Jwt:Issuer"]
					, _config["Jwt:Audience"]
					, new Claim[]
					{
				new(ClaimTypes.Name, systemUserAccount.UserName),
                //new(ClaimTypes.Email, systemUserAccount.Email),
                new(ClaimTypes.Role, systemUserAccount.RoleId.ToString()),
					},
					expires: DateTime.Now.AddMinutes(120),
					signingCredentials: credentials
				);

			var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

			return tokenString;
		}

		[HttpGet]
		public async Task<IActionResult> GetAllUserAccounts()
		{
			var users = await _userAccountsService.GetAllUserAccounts();

			if (users == null || !users.Any())
				return NotFound("No user accounts found.");

			return Ok(users);
		}

		public sealed record LoginRequest(string UserName, string Password);
	}
}
