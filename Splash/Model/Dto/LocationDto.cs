using Splash.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Splash.Model.Dto
{
    public class LocationDto
    {
        public virtual int Id { get; protected set; }

        public virtual int UserId { get; set; }
        public virtual long Timestamp { get; set; }

        public virtual decimal Latitude { get; set; }
        public virtual decimal Longitude { get; set; }

        public LocationDto(int userId)
        {
            UserId = userId;
            Timestamp = default(long);

            Latitude = default(decimal);
            Longitude = default(decimal);
        }

        public LocationDto(Location location)
        {
            this.Id = location.Id;
            this.UserId = location.User.Id;
            this.Timestamp = location.Timestamp;
            this.Latitude = location.Latitude;
            this.Longitude = location.Longitude;
        }

        public override string ToString()
        {
            return String.Format("{{ Timestamp={0}, Latitude={1}, Longitude={2} }}", Timestamp, Latitude, Longitude);
        }
    }
}