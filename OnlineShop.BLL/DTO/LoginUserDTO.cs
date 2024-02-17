namespace OnlineShop.BLL.DTO
{
    public class LoginUserDTO
    {
        [Required]
        [MinLength(4)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        [MinLength(8)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
