namespace OnlineShop.BLL.Services.Implementations
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepo _accountRepo;
        private readonly IMailSenderService _mailSender;
        private readonly IConfiguration _configuration;

        public AccountService(IAccountRepo accountRepo,
            IMailSenderService mailSender,
            IConfiguration configuration)
        {
            _accountRepo = accountRepo;
            _mailSender = mailSender;
            _configuration = configuration;
        }

        public async Task<RestDTO<bool>> Register(RegisterUserDTO user, string confirmEmailUrl)
        {
            var response = new RestDTO<bool>();

            var newUser = new ApplicationUser() {
                Email = user.Email,
                UserName = user.Username
            };
            var isCreated = await _accountRepo.CreateUser(newUser, user.Password);

            if (isCreated.Succeeded) {
                var code = await _accountRepo.GetConfirmationToken(newUser);
                var mailData = GetMailData(newUser, confirmEmailUrl, code,
                    "Please confirm your email here", "Confirm Email");

                response.Data = await _mailSender.Send(mailData);
                if (response.Data)
                    response.Messages.Add("User created successfuly");
                else
                    response.Messages.Add("Email Created but cannot to send confirm email");
                return response;
            }

            response.Data = false;
            response.Messages.Add(isCreated.Errors.First().Description);
            return response;
        }

        public async Task<RestDTO<bool>> ConfirmEmail(string userId, string code)
        {
            var response = new RestDTO<bool>();
            var user = await _accountRepo.GetUserById(userId);
            if (user == null || user.EmailConfirmed) {
                response.Data = false;
                response.Messages.Add("Email isn't confirmed or doesn't exist");
                return response;
            }

            var result = await _accountRepo.ConfirmEmail(user, code);
            response.Data = result.Succeeded;
            if (result.Succeeded)
                response.Messages.Add("Email is confirmed successfuly");
            else
                response.Messages.Add(result.Errors.First().Description);
            return response;
        }

        public async Task<RestDTO<JwtSecurityToken?>> Login(LoginUserDTO user)
        {
            var response = new RestDTO<JwtSecurityToken?>();
            var existUser = await _accountRepo.GetUserByEmail(user.Email);
            if (IsNotConfirmedUser(existUser)) {
                response.Messages.Add("Email is not confirmed");
                return response;
            }
            
            var isAuthenticated = await _accountRepo.CheckPassword(existUser!, user.Password);
            if (!isAuthenticated) {
                response.Messages.Add("Email is not authenticated");
                return response;
            }

            // Create JWT token
            response.Data = await CreateToken(existUser!);
            response.Messages.Add("Logged in successfuly");
            return response;
        }

        public async Task<RestDTO<bool>> ForgotPassword(string email, string confirmEmailUrl)
        {
            var response = new RestDTO<bool>();
            var user = await _accountRepo.GetUserByEmail(email);
            if (IsNotConfirmedUser(user)) {
                response.Data = false;
                response.Messages.Add("Email is not confirmed");
                return response;
            }

            var code = await _accountRepo.GetResetPasswordToken(user!);
            var mailData = GetMailData(user!, confirmEmailUrl, code,
                "Please reset you password here", "Reset Password");
            response.Data = await _mailSender.Send(mailData);

            if (response.Data)
                response.Messages.Add("Check reset password email");
            else
                response.Messages.Add("Cannot send email");
            return response;
        }

        public async Task<RestDTO<bool>> ResetPassword(ResetPasswordDTO request)
        {
            var response = new RestDTO<bool>();
            var user = await _accountRepo.GetUserById(request.UserId);
            if (user == null)
                return DoesntExistUserResponse(response);

            request.Token = HttpUtility.UrlDecode(request.Token);
            var result = await _accountRepo.ResetPassword(user, request.Token, request.Password);

            response.Data = result.Succeeded;
            if(response.Data)
                response.Messages.Add("Password reset successfuly");
            else
                response.Messages.Add(result.Errors.First().Description);
            return response;
        }

        public async Task<RestDTO<bool>> ChangePassword(ChangePasswordDTO request)
        {
            var response = new RestDTO<bool>();
            var user = await _accountRepo.GetUserById(request.UserId);
            if (user == null)
                return DoesntExistUserResponse(response);

            var result = await _accountRepo.ChangePassword(user!, request.OldPassword, request.Password);
            
            response.Data = result.Succeeded;
            if(response.Data)
                response.Messages.Add("Password changed successfuly");
            else
                response.Messages.Add(result.Errors.First().Description);
            return response;
        }

        private RestDTO<bool> DoesntExistUserResponse(RestDTO<bool> response)
        {
            response.Data = false;
            response.Messages.Add("User doesn't exist");
            return response;
        }

        private MailDataDTO GetMailData(ApplicationUser user, string url, string code,
            string message, string subject)
        {
            var callBackUrl = $"{url}?userId={user.Id}&code={HttpUtility.UrlEncode(code)}";

            var mailData = new MailDataDTO() {
                EmailTo = user.Email!,
                Subject = subject,
                Body = $"{message} <a href=\"{callBackUrl}\">Clikc here</a>"
            };
            return mailData;
        }

        private async Task<JwtSecurityToken> CreateToken(ApplicationUser user)
        {
            var claims = new List<Claim> {
                new (ClaimTypes.NameIdentifier, user.Id),
                new (ClaimTypes.Name, user.UserName!),
                new (ClaimTypes.Email, user.Email!),
                new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var roles = await _accountRepo.GetRoles(user);
            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: signingCredentials
                );

            return token;
        }

        private bool IsNotConfirmedUser(ApplicationUser? user)
        {
            return user == null || !user.EmailConfirmed;
        }
    }
}
