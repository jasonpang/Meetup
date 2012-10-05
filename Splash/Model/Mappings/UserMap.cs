using FluentNHibernate.Mapping;
using Splash.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Splash.Model.Mappings
{
    public class UserMap : ClassMap<User>
    {
        public UserMap()
        {
            Table("users");

            Id(x => x.Id);

            Map(x => x.Created);
            Map(x => x.LastLoggedIn);

            Map(x => x.FirstName);
            Map(x => x.LastName);
            Map(x => x.Nickname);
            Map(x => x.PhoneNumber);
            Map(x => x.Email);
            Map(x => x.Password);

            HasMany(x => x.Locations).KeyColumn("ownerId").Inverse().Cascade.All();
            HasMany(x => x.Friends).KeyColumn("frienderId").Inverse().Cascade.All();
        }
    }
}