namespace OnlineShop.BLL.DTO
{
    public class RestDTO <T>
    {
        public List<string> Messages { get; set; } = new();
        public T Data { get; set; }
    }
}
