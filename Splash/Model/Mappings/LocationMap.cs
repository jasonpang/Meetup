using FluentNHibernate.Mapping;
using Splash.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Splash.Model.Mappings
{
    public class LocationMap : ClassMap<Location>
    {
        public LocationMap()
        {
            Table("locations");

            Id(x => x.Id);

            References(x => x.User, "ownerId").Cascade.None();
            Map(x => x.Timestamp).Not.Nullable();

            Map(x => x.Latitude).Not.Nullable();
            Map(x => x.Longitude).Not.Nullable();
        }
    }
}