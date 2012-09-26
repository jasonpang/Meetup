using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Splash.Model.Entities
{
    public enum FriendRequestStatus
    {
        Pending,
        Accepted,
    }

    public enum FollowRequestStatus
    {
        Uninitiated,
        Pending,
        Accepted,
    }

    public class Friend
    {
        public virtual int Id { get; protected set; }

        public virtual User Friender { get; set; }

        public virtual User Friendee { get; set; }

        public virtual FriendRequestStatus FriendRequestStatus { get; set; }
        public virtual FollowRequestStatus FollowRequestStatus { get; set; }

        public Friend()
        {
            FriendRequestStatus = FriendRequestStatus.Pending;
            FollowRequestStatus = FollowRequestStatus.Uninitiated;
        }
    }
}