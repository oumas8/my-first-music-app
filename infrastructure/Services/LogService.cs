using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using Serilog.Events;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace infrastructure.Services
{
    public static class LogService
    {
        private static EmailService _emailService;
        private static ILogger Logger;
        public static void ConfigureLogger(string logDirectory, EmailService emailService)
        {
            _emailService = emailService;
            // Vérifier que le répertoire existe, sinon le créer
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            // Configurer Serilog avec RollingInterval.Day
            Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File(
                    path: Path.Combine(logDirectory, "log-.txt"),
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{End}")
                .CreateLogger();
        }

        public static void LogError(HttpContext context, Exception ex, string customMessage, string iduser)
        {
            //var bodyContent = GetRequestBodyAsync(context).GetAwaiter().GetResult();

            // Construire le message de log avec des sauts de ligne
            var logMessage = $@"
            Error occurred: {customMessage}
            User: {iduser ?? "Anonymous"}
            IP: {context.Connection.RemoteIpAddress}
            URL: {context.Request.Path}
            Referrer: {context.Request.Headers["Referer"]}
            Body: {/*bodyContent*/""}
            Exception: {ex.ToString()}
            End:--- End of Log ---
            ";

            // Enregistrer le message avec Serilog
            Logger?.Error(ex, logMessage);

            var emailSubject = $"Error Occurred: {customMessage}";
            var emailBody = $@"
                <h1>Error Occurred</h1>
                <p><strong>User:</strong> {iduser ?? "Anonymous"}</p>
                <p><strong>IP:</strong> {context.Connection.RemoteIpAddress}</p>
                <p><strong>URL:</strong> {context.Request.Path}</p>
                <p><strong>Referrer:</strong> {context.Request.Headers["Referer"]}</p>
                <p><strong>Body:</strong> {/*bodyContent*/""}</p>
                <p><strong>Exception:</strong> {ex}</p>
                <hr>
                <p>This is an automated message from the logging system.</p>
                ";
            if (_emailService != null)
            {
                _emailService.SendEmailAsync(subject:emailSubject,body:emailBody).GetAwaiter().GetResult();
            }
        }
        public static void LogRequest(HttpContext context)
        {
           // var bodyContent = GetRequestBodyAsync(context).GetAwaiter().GetResult();

            // Construire le message de log avec des sauts de ligne
            var logMessage = $@"
            Request logged:
            User: {context.User.Identity?.Name ?? "Anonymous"}
            IP: {context.Connection.RemoteIpAddress?.ToString()}
            URL: {context.Request.Path}
            Referrer: {context.Request.Headers["Referer"].ToString()}
            Body: {/*bodyContent*/""}
           

            --- End of Request Log ---
            ";

            // Enregistrer le message avec Serilog
            Log.Information(logMessage);
        }

        private static async Task<string> GetRequestBodyAsync(HttpContext context)
        {
            context.Request.EnableBuffering();
            using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0; // Reset stream position
            return string.IsNullOrEmpty(body) ? "No Body" : body;
        }
    }
}
