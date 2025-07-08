using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourApp.Domain.ValueObjects
{
    public class Location : IEquatable<Location>
    {
        public double Latitude { get; }
        public double Longitude { get; }

        public Location(double latitude, double longitude)
        {
            if (latitude < -90 || latitude > 90)
                throw new ArgumentException("Latitude must be between -90 and 90 degrees");

            if (longitude < -180 || longitude > 180)
                throw new ArgumentException("Longitude must be between -180 and 180 degrees");

            Latitude = latitude;
            Longitude = longitude;
        }

        public bool Equals(Location other)
        {
            if (other is null) return false;
            return Latitude == other.Latitude && Longitude == other.Longitude;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Location);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Latitude, Longitude);
        }

        public override string ToString()
        {
            return $"{Latitude}, {Longitude}";
        }
    }
}
