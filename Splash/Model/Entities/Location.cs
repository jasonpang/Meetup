using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Splash.Model.Entities
{
    public class Location
    {
        public virtual int Id { get; protected set; }

        public virtual User User { get; set; }
        public virtual long Timestamp { get; set; }

        public virtual decimal Latitude { get; set; }
        public virtual decimal Longitude { get; set; }

        public Location()
        {
            Timestamp = default(long);

            Latitude = default(decimal);
            Longitude = default(decimal);
        }
    }
}