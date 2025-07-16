using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourApp.Domain.Entities
{
    public class Guide : User
    {
        private readonly List<Tour> _tours = new List<Tour>();

        public IReadOnlyCollection<Tour> Tours => _tours.AsReadOnly();
        public int CancelledToursCount { get; private set; }
        public int RewardPoints { get; set; }
        public bool IsRewarded => RewardPoints >= 5;

        protected Guide() { }

        public Guide(string username, string email, string passwordHash,
                    string firstName, string lastName)
            : base(username, email, passwordHash, firstName, lastName)
        {
            CancelledToursCount = 0;
            RewardPoints = 0;
        }

        public void AddTour(Tour tour)
        {
            if (tour == null)
                throw new ArgumentNullException(nameof(tour));

            _tours.Add(tour);
        }

        public void IncrementCancelledTours()
        {
            CancelledToursCount++;
        }

        public void AddRewardPoint()
        {
            RewardPoints++;
        }

        public bool IsMalicious()
        {
            return CancelledToursCount >= 10;
        }
    }
}
