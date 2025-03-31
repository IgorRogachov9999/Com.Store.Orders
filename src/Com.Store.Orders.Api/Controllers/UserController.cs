using Com.Store.Orders.Api.Authentication;
using Com.Store.Orders.Domain.Data.Helpers;
using Com.Store.Orders.Domain.Services.Dto;
using Com.Store.Orders.Domain.Services.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Com.Store.Orders.Api.Controllers
{
    [ApiController]
    [Route("api/v1/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJwtService _jwtService;

        public UserController(
            IUserService userService,
            IJwtService jwtService)
        {
            _userService = userService;
            _jwtService = jwtService;
        }

        [HttpPost("token")]
        public async Task<ActionResult<string>> GetTokenAsync([FromBody] UserCredentialsDto userCredentials, CancellationToken ct = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var passwordHash = PasswordHelper.HashPassword(userCredentials.Password);
            var user = await _userService.GetByEmailAndPassowrdAsync(userCredentials.Email, passwordHash, ct);
            return _jwtService.GenerateToken(user.Id.ToString(), user.Roles.Select(x => x.ToString()).ToArray());
        }
    }
}
