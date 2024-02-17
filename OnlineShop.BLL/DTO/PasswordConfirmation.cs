namespace OnlineShop.BLL.DTO
{
    public class PasswordConfirmation
    {
        [Required]
        [MinLength(8)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;
        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; } = null!;
    }
}
