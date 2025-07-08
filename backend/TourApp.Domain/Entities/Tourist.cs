using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourApp.Domain.Enums;

namespace TourApp.Domain.Entities
{
    public class Tourist : User
    {
        private readonly List<Interest> _interests = new List<Interest>();
        private readonly List<Purchase> _purchases = new List<Purchase>();
        private readonly List<Problem> _reportedProblems = new List<Problem>();

        public IReadOnlyCollection<Interest> Interests => _interests.AsReadOnly();
        public IReadOnlyCollection<Purchase> Purchases => _purchases.AsReadOnly();
        public IReadOnlyCollection<Problem> ReportedProblems => _reportedProblems.AsReadOnly();
        public decimal BonusPoints { get; private set; }
        public int InvalidProblemsCount { get; private set; }

        protected Tourist() { }

        public Tourist(string username, string email, string passwordHash,
                      string firstName, string lastName, List<Interest> interests)
            : base(username, email, passwordHash, firstName, lastName)
        {
            if (interests == null || !interests.Any())
                throw new ArgumentException("Tourist must have at least one interest");

            _interests.AddRange(interests);
            BonusPoints = 0;
            InvalidProblemsCount = 0;
        }

        public void UpdateInterests(List<Interest> newInterests)
        {
            if (newInterests == null || !newInterests.Any())
                throw new ArgumentException("Tourist must have at least one interest");

            _interests.Clear();
            _interests.AddRange(newInterests);
        }

        public void AddBonusPoints(decimal points)
        {
            if (points < 0)
                throw new ArgumentException("Bonus points cannot be negative");

            BonusPoints += points;
        }

        public decimal UseBonusPoints(decimal amount)
        {
            if (amount < 0)
                throw new ArgumentException("Amount cannot be negative");

            var pointsToUse = Math.Min(amount, BonusPoints);
            BonusPoints -= pointsToUse;
            return pointsToUse;
        }

        public void IncrementInvalidProblems()
        {
            InvalidProblemsCount++;
        }

        public bool IsMalicious()
        {
            return InvalidProblemsCount >= 10;
        }
    }
}
