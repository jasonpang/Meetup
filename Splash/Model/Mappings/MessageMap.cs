using FluentNHibernate.Mapping;
using Splash.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Splash.Model.Mappings
{
    public class MessageMap : ClassMap<Message>
    {
        public MessageMap()
        {
            Table("messages");

            Id(x => x.Id);

            References(x => x.User, "ownerId").Cascade.None();
            Map(x => x.Timestamp);
            Map(x => x.MessageType);
            Map(x => x.Content);
            Map(x => x.IsRead);
            Map(x => x.YesActionUri);
            Map(x => x.OKActionUri);
            Map(x => x.NoActionUri);
        }
    }
}