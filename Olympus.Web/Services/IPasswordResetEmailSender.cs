namespace Olympus.Web.Services;

public interface IPasswordResetEmailSender
{
    Task SendPasswordResetLinkAsync(
        string email,
        string resetLink);
}