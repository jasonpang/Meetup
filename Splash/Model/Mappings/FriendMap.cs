using FluentNHibernate.Mapping;
using Splash.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Splash.Model.Mappings
{
    public class FriendMap : ClassMap<Friend>
    {
        public FriendMap()
        {           
            Table("friends");

            Id(x => x.Id);
            References(x => x.Friender, "userId").Cascade.None();
            References(x => x.Friendee, "friendId").Cascade.None();
            Map(x => x.FriendRequestStatus);
            Map(x => x.FollowRequestStatus);
        }
    }
}