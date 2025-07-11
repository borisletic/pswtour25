using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;


namespace TourApp.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _senderEmail;
        private readonly string _senderName;
        private readonly string _username;
        private readonly string _password;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
            _smtpServer = _configuration["EmailSettings:SmtpServer"];
            _smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);
            _senderEmail = _configuration["EmailSettings:SenderEmail"];
            _senderName = _configuration["EmailSettings:SenderName"];
            _username = _configuration["EmailSettings:Username"];
            _password = _configuration["EmailSettings:Password"];
        }

        public async Task SendEmailAsync(string to, string subject, string htmlBody)
        {
            try
            {
                using var client = new SmtpClient(_smtpServer, _smtpPort)
                {
                    EnableSsl = true,
                    Credentials = new NetworkCredential(_username, _password)
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_senderEmail, _senderName),
                    Subject = subject,
                    Body = htmlBody,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(to);

                await client.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Failed to send email: {ex.Message}");
                throw;
            }
        }

        public async Task SendRegistrationConfirmationAsync(string email, string firstName)
        {
            var subject = "Welcome to Tour App!";
            var body = $@"
                <html>
                <body>
                    <h2>Welcome {firstName}!</h2>
                    <p>Thank you for registering with Tour App.</p>
                    <p>You can now browse and purchase amazing tours from our expert guides.</p>
                    <p>Start exploring at <a href='https://tourapp.com'>tourapp.com</a></p>
                    <br>
                    <p>Best regards,<br>Tour App Team</p>
                </body>
                </html>";

            await SendEmailAsync(email, subject, body);
        }

        public async Task SendPurchaseConfirmationAsync(string email, PurchaseEmailData data)
        {
            var toursList = string.Join("</li><li>", data.TourNames);
            var subject = "Purchase Confirmation - Tour App";
            var body = $@"
                <html>
                <body>
                    <h2>Thank you for your purchase, {data.FirstName}!</h2>
                    <p>Your purchase has been confirmed.</p>
                    <h3>Tours purchased:</h3>
                    <ul>
                        <li>{toursList}</li>
                    </ul>
                    <p><strong>Total Amount: {data.TotalAmount:F2} {data.Currency}</strong></p>
                    <p>You will receive a reminder 48 hours before each tour.</p>
                    <br>
                    <p>Best regards,<br>Tour App Team</p>
                </body>
                </html>";

            await SendEmailAsync(email, subject, body);
        }

        public async Task SendTourReminderAsync(string email, TourReminderData data)
        {
            var subject = $"Reminder: {data.TourName} - Coming Soon!";
            var body = $@"
                <html>
                <body>
                    <h2>Tour Reminder</h2>
                    <p>This is a friendly reminder that your tour is coming up!</p>
                    <h3>Tour Details:</h3>
                    <ul>
                        <li><strong>Tour:</strong> {data.TourName}</li>
                        <li><strong>Date:</strong> {data.ScheduledDate:dddd, MMMM dd, yyyy}</li>
                        <li><strong>Time:</strong> {data.ScheduledDate:HH:mm}</li>
                        <li><strong>Meeting Point:</strong> {data.MeetingPoint}</li>
                    </ul>
                    <p>Please arrive 15 minutes before the scheduled time.</p>
                    <p>We look forward to seeing you!</p>
                    <br>
                    <p>Best regards,<br>Tour App Team</p>
                </body>
                </html>";

            await SendEmailAsync(email, subject, body);
        }

        public async Task SendAccountBlockedNotificationAsync(string email, string reason)
        {
            var subject = "Account Status - Tour App";
            var body = $@"
                <html>
                <body>
                    <h2>Account Blocked</h2>
                    <p>Your account has been blocked by an administrator.</p>
                    <p><strong>Reason:</strong> {reason}</p>
                    <p>If you believe this is an error, please contact support at support@touristtours.com</p>
                    <br>
                    <p>Best regards,<br>Tour App Team</p>
                </body>
                </html>";

            await SendEmailAsync(email, subject, body);
        }

        public async Task SendTourCancelledNotificationAsync(string email, string tourName, decimal refundAmount)
        {
            var subject = $"Tour Cancelled - {tourName}";
            var body = $@"
                <html>
                <body>
                    <h2>Tour Cancellation Notice</h2>
                    <p>We regret to inform you that the following tour has been cancelled:</p>
                    <p><strong>{tourName}</strong></p>
                    <p>As compensation, we have added <strong>{refundAmount:F2} EUR</strong> bonus points to your account.</p>
                    <p>These bonus points can be used for your next purchase.</p>
                    <p>We apologize for any inconvenience.</p>
                    <br>
                    <p>Best regards,<br>Tour App Team</p>
                </body>
                </html>";

            await SendEmailAsync(email, subject, body);
        }

        public async Task SendMonthlyReportAsync(string email, MonthlyReportData data)
        {
            var toursList = string.Join("", data.TourSales.Select(ts =>
                $"<tr><td>{ts.TourName}</td><td>{ts.SalesCount}</td></tr>"));

            var subject = $"Monthly Report - {data.Month}/{data.Year}";
            var body = $@"
                <html>
                <body>
                    <h2>Monthly Report for {data.Month}/{data.Year}</h2>
                    <p>Hello {data.GuideName},</p>
                    <p>Here is your monthly performance report:</p>
                    
                    <h3>Tour Sales</h3>
                    <table border='1' cellpadding='5'>
                        <tr>
                            <th>Tour Name</th>
                            <th>Number of Sales</th>
                        </tr>
                        {toursList}
                    </table>
                    
                    <h3>Ratings Summary</h3>
                    <ul>
                        <li><strong>Best Rated Tour:</strong> {data.BestRatedTourName} 
                            ({data.BestRatedScore:F1}/5.0 from {data.BestRatedCount} reviews)</li>
                        <li><strong>Worst Rated Tour:</strong> {data.WorstRatedTourName} 
                            ({data.WorstRatedScore:F1}/5.0 from {data.WorstRatedCount} reviews)</li>
                    </ul>
                    
                    <p><strong>Total Tours Sold:</strong> {data.TotalSales}</p>
                    
                    <br>
                    <p>Keep up the great work!</p>
                    <p>Best regards,<br>Tour App Team</p>
                </body>
                </html>";

            await SendEmailAsync(email, subject, body);
        }

        
    }

    public class MonthlyReportData
    {
        public string GuideName { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public List<TourSalesDto> TourSales { get; set; }
        public string BestRatedTourName { get; set; }
        public double BestRatedScore { get; set; }
        public int BestRatedCount { get; set; }
        public string WorstRatedTourName { get; set; }
        public double WorstRatedScore { get; set; }
        public int WorstRatedCount { get; set; }
        public int TotalSales { get; set; }
    }

    public class TourSalesDto
    {
        public string TourName { get; set; }
        public int SalesCount { get; set; }
    }

}
