namespace OnlineShop.BLL.Services.Interfaces
{
    public interface IAccountService
    {
        public Task<RestDTO<bool>> Register(RegisterUserDTO user, string confirmEmailUrl);
        public Task<RestDTO<bool>> ConfirmEmail(string userId, string code);
        public Task<RestDTO<JwtSecurityToken?>> Login(LoginUserDTO user);
        public Task<RestDTO<bool>> ForgotPassword(string email, string confirmEmailUrl);
        public Task<RestDTO<bool>> ResetPassword(ResetPasswordDTO request);
        public Task<RestDTO<bool>> ChangePassword(ChangePasswordDTO request);
    }
}
