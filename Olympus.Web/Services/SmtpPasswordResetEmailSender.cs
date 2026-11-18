using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;


namespace Olympus.Web.Services;

public class SmtpPasswordResetEmailSender : IPasswordResetEmailSender
{
    private readonly IConfiguration _configuration;

    public SmtpPasswordResetEmailSender(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendPasswordResetLinkAsync(
        string email,
        string resetLink)
    {
        var host = _configuration["Smtp:Host"];
        var port = int.Parse(_configuration["Smtp:Port"] ?? "587");
        var username = _configuration["Smtp:Username"];
        var password = _configuration["Smtp:Password"];

        if (string.IsNullOrWhiteSpace(host) ||
            string.IsNullOrWhiteSpace(username) ||
            string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException(
                "Konfiguracja serwera SMTP jest niekompletna.");
        }

        var message = new MimeMessage();

        message.From.Add(
            new MailboxAddress("OLYMPUS", "noreply@olympus.test"));

        message.To.Add(MailboxAddress.Parse(email));

        message.Subject = "OLYMPUS - Resetowanie hasła";

        message.Body = new TextPart("plain")
        {
            Text = $"""
                Witaj!

                Otrzymaliśmy prośbę o zresetowanie hasła
                do Twojego konta OLYMPUS.

                Aby ustawić nowe hasło, otwórz poniższy link:

                {resetLink}

                Jeśli nie prosiłeś o zmianę hasła,
                zignoruj tę wiadomość.

                Pozdrawiamy,
                Zespół OLYMPUS
                """
        };

        using var client = new MailKit.Net.Smtp.SmtpClient();

        await client.ConnectAsync(
            host,
            port,
            SecureSocketOptions.StartTls);

        await client.AuthenticateAsync(username, password);

        await client.SendAsync(message);

        await client.DisconnectAsync(true);
    }
}
