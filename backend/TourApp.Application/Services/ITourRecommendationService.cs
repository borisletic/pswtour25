using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourApp.Domain.Entities;

namespace TourApp.Application.Services
{
    public interface ITourRecommendationService
    {
        Task SendRecommendationsForNewTour(Tour tour);
    }
}
