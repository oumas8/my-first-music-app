using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace infrastructure.Services
{
    
public class EmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string subject="No Subject", string body="No Body", string toEmail = "mourad.makrouf@gmail.com")
    {
        // Lire la configuration depuis appsettings.json
        var emailSettings = _configuration.GetSection("EmailSettings");
        var smtpServer = emailSettings["SmtpServer"];
        var port = int.Parse(emailSettings["Port"]);
        var senderName = emailSettings["SenderName"];
        var senderEmail = emailSettings["SenderEmail"];
        var password = emailSettings["Password"];

        var smtpClient = new SmtpClient(smtpServer)
        {
            Port = port,
            Credentials = new NetworkCredential(senderEmail, password),
            EnableSsl = true
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(senderEmail, senderName),
            Subject = subject,
            Body = body,
            IsBodyHtml = true // Utiliser HTML si nécessaire
        };
        mailMessage.To.Add(toEmail);

        // Envoi de l'email
        await smtpClient.SendMailAsync(mailMessage);
    }
}
}