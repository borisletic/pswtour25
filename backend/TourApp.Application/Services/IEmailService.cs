using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TourApp.Application.Services
{
    public interface IEmailService
    {
        Task SendRegistrationConfirmationAsync(string email, string firstName);
        Task SendPurchaseConfirmationAsync(string email, PurchaseEmailData data);
        Task SendTourReminderAsync(string email, TourReminderData data);
        Task SendAccountBlockedNotificationAsync(string email, string reason);
        Task SendTourCancelledNotificationAsync(string email, string tourName, decimal refundAmount);
        Task SendMonthlyReportAsync(string email, MonthlyReportData data);
        Task SendEmailAsync(string to, string subject, string htmlBody);
    }

    public class PurchaseEmailData
    {
        public string FirstName { get; set; }
        public List<string> TourNames { get; set; }
        public decimal TotalAmount { get; set; }
        public string Currency { get; set; }
    }

    public class TourReminderData
    {
        public string TourName { get; set; }
        public DateTime ScheduledDate { get; set; }
        public string MeetingPoint { get; set; }
    }
}