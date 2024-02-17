
namespace OnlineShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IAccountService _accountService;

        public AccountController(IConfiguration configuration,
            IAccountService accountService)
        {
            _configuration = configuration;
            _accountService = accountService;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserDTO newUser)
        {
            if (ModelState.IsValid) {
                var confirmEmailUrl = GetCallBackUrl(nameof(ConfirmEmail));
                var response = await _accountService.Register(newUser, confirmEmailUrl);
                if (response.Data)
                    return Ok(response.Messages);
                else
                    return BadRequest(response.Messages); // may be Ok and tell him to request email verification
            }
            return BadRequest(ModelState.First());
        }
        [HttpGet("confirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (ModelState.IsValid) {
                if (userId != null && code != null) {
                    var response = await _accountService.ConfirmEmail(userId, code);
                    if (response.Data)
                        return NoContent();
                    else
                        return BadRequest(response.Messages);
                }
                return BadRequest("Invalid data");
            }
            return BadRequest(ModelState.First());
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginUserDTO user)
        {
            if (ModelState.IsValid) {
                var response = await _accountService.Login(user);

                if (response.Data != null)
                    return Ok(new {
                        token = new JwtSecurityTokenHandler().WriteToken(response.Data),
                        expiration = response.Data.ValidTo
                    }); // should return JWT token here
                return BadRequest(response.Messages);
            }

            return BadRequest(ModelState.First());
        }
        [HttpPost("forgotPassword")]
        public async Task<IActionResult> ForgotPassword([DataType(DataType.EmailAddress)] string email)
        {
            if (ModelState.IsValid) {
                var confirmEmailUrl = GetCallBackUrl(nameof(ResetPassword));
                var response = await _accountService.ForgotPassword(email, confirmEmailUrl);

                if (response.Data)
                    return Ok(response.Messages);
                else
                    return BadRequest(response.Messages);
            }
            return BadRequest(ModelState.First());
        }
        [HttpPost("resetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO request)
        {
            if (ModelState.IsValid) {
                var response = await _accountService.ResetPassword(request);
                if (response.Data)
                    return NoContent();
                return BadRequest(response.Messages);
            }
            return BadRequest(ModelState.First());
        }
        [HttpPut("changePassword")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordDTO request)
        {
            if (ModelState.IsValid) {
                var response = await _accountService.ChangePassword(request);
                if (response.Data)
                    return NoContent();
                return BadRequest(response.Messages);
            }
            return BadRequest(ModelState.First());
        }
        private string GetCallBackUrl(string action) => $"{Request.Scheme}://{Request.Host}{Url.Action(action, nameof(AccountController).Replace("Controller", ""))}";
    }
}

