namespace OnlineShop.DAL.Repositories.Implementations
{
    public class AccountRepo
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountRepo(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public Task<IdentityResult> CreateUser(ApplicationUser newUser, string password)
        {
            return _userManager.CreateAsync(newUser, password);
        }

        public async Task<string> GetConfirmationToken(ApplicationUser newUser)
        {
            return await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
        }

        public async Task<string> GetResetPasswordToken(ApplicationUser user)
        {
            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }

        public async Task<IdentityResult> ConfirmEmail(ApplicationUser user, string token)
        {
            return await _userManager.ConfirmEmailAsync(user, token);
        }

        public async Task<ApplicationUser?> GetUserById(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<ApplicationUser?> GetUserByName(string name)
        {
            return await _userManager.FindByNameAsync(name);
        }

        public async Task<ApplicationUser?> GetUserByEmail(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }
        
        public async Task<bool> CheckPassword(ApplicationUser user, string password)
        {
            return await _userManager.CheckPasswordAsync(user, password);
        }
        
        public async Task<IList<string>?> GetRoles(ApplicationUser user)
        {
            return await _userManager.GetRolesAsync(user);
        }
        
        public async Task<IdentityResult> ResetPassword(ApplicationUser user, string token, string password)
        {
            return await _userManager.ResetPasswordAsync(user, token, password);
        }
        
        public async Task<IdentityResult> ChangePassword(ApplicationUser user, string oldPassword, string newPassword)
        {
            return await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
        }
    }
}
