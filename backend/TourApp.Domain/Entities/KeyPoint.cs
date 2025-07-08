using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourApp.Domain.ValueObjects;

namespace TourApp.Domain.Entities
{
    public class KeyPoint
    {
        public Guid Id { get; private set; }
        public Guid TourId { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public Location Location { get; private set; }
        public string ImageUrl { get; private set; }
        public int Order { get; private set; }

        protected KeyPoint() { }

        public KeyPoint(Guid tourId, string name, string description,
                       Location location, string imageUrl, int order)
        {
            Id = Guid.NewGuid();
            TourId = tourId;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            Location = location ?? throw new ArgumentNullException(nameof(location));
            ImageUrl = imageUrl;
            Order = order;
        }
    }
}
