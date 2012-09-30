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

            Map(x => x.Created).Not.Nullable();
            Map(x => x.LastLoggedIn).Not.Nullable();

            Map(x => x.FirstName).Not.Nullable();
            Map(x => x.LastName).Not.Nullable();
            Map(x => x.Nickname).Not.Nullable();
            Map(x => x.PhoneNumber).Not.Nullable();
            Map(x => x.Email).Not.Nullable();
            Map(x => x.Password).Not.Nullable();

            HasMany(x => x.Locations).KeyColumn("ownerId").Inverse().Cascade.All();
            HasMany(x => x.Friends).KeyColumn("frienderId").Inverse().Cascade.All();
        }
    }
}