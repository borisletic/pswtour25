using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourApp.Application.Services
{
    public interface INotificationService
    {
        Task SendTourRemindersAsync();
        Task SendMonthlyReportsAsync();
        Task ProcessMonthlyRewardsAsync();
    }
}
