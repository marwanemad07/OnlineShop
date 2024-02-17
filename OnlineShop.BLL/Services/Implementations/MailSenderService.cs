namespace OnlineShop.BLL.Services.Implementations
{
    public class MailSenderService : IMailSenderService
    {
        private readonly IConfiguration _configuration;
        public MailSenderService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> Send(MailDataDTO mail)
        {
            try {
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(_configuration.GetSection("MailSettings:UserName").Value));
                email.To.Add(MailboxAddress.Parse(mail.EmailTo));
                email.Subject = mail.Subject;
                email.Body = new TextPart(TextFormat.Html) { Text = mail.Body };

                return await SendEmail(email);
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        private async Task<bool> SendEmail(MimeMessage? email)
        {
            try {
                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(_configuration.GetSection("MailSettings:Host").Value,
                    int.Parse(_configuration.GetSection("MailSettings:Port").Value),
                    SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_configuration.GetSection("MailSettings:UserName").Value,
                    _configuration.GetSection("MailSettings:Password").Value);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
                return false;
            }
            return true;
        }

    }
}
