using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourApp.Domain.Entities
{
    public class MonthlyReport
    {
        public Guid Id { get; private set; }
        public Guid GuideId { get; private set; }
        public int Month { get; private set; }
        public int Year { get; private set; }
        public List<TourSalesInfo> TourSales { get; private set; }
        public Guid? BestRatedTourId { get; private set; }
        public double BestRatedTourScore { get; private set; }
        public int BestRatedTourRatingCount { get; private set; }
        public Guid? WorstRatedTourId { get; private set; }
        public double WorstRatedTourScore { get; private set; }
        public int WorstRatedTourRatingCount { get; private set; }
        public DateTime GeneratedAt { get; private set; }

        protected MonthlyReport() { }

        public MonthlyReport(Guid guideId, int month, int year)
        {
            Id = Guid.NewGuid();
            GuideId = guideId;
            Month = month;
            Year = year;
            TourSales = new List<TourSalesInfo>();
            GeneratedAt = DateTime.UtcNow;
        }

        public void AddTourSales(Guid tourId, string tourName, int salesCount)
        {
            TourSales.Add(new TourSalesInfo
            {
                TourId = tourId,
                TourName = tourName,
                SalesCount = salesCount
            });
        }

        public void SetBestRatedTour(Guid tourId, double score, int ratingCount)
        {
            BestRatedTourId = tourId;
            BestRatedTourScore = score;
            BestRatedTourRatingCount = ratingCount;
        }

        public void SetWorstRatedTour(Guid tourId, double score, int ratingCount)
        {
            WorstRatedTourId = tourId;
            WorstRatedTourScore = score;
            WorstRatedTourRatingCount = ratingCount;
        }

        public int GetTotalSales()
        {
            return TourSales.Sum(ts => ts.SalesCount);
        }
    }

    public class TourSalesInfo
    {
        public Guid TourId { get; set; }
        public string TourName { get; set; }
        public int SalesCount { get; set; }
    }
}
