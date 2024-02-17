namespace OnlineShop.BLL.Services.Interfaces
{
    public interface IMailSenderService
    {
        public Task<bool> Send(MailDataDTO email);
    }
}
