namespace OnlineShop.BLL.DTO
{
    public class RegisterUserDTO : PasswordConfirmation
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = null!;
        [Required]
        [MinLength(4)]
        public string Username { get; set; } = null!;
    }
}
