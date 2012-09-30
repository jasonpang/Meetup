using Splash.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Splash.Model.Dto
{
    public class FriendDto
    {
        public virtual int Id { get; protected set; }

        public virtual int FrienderId { get; set; }

        public virtual int FriendeeId { get; set; }

        public virtual FriendRequestStatus FriendRequestStatus { get; set; }
        public virtual FollowRequestStatus FollowRequestStatus { get; set; }

        public FriendDto(int frienderId, int friendeeId)
        {
            FrienderId = frienderId;
            FriendeeId = friendeeId;
            FriendRequestStatus = FriendRequestStatus.Pending;
            FollowRequestStatus = FollowRequestStatus.Uninitiated;
        }

        public FriendDto(Friend friend)
        {
            this.Id = friend.Id;
            this.FriendeeId = friend.Friendee.Id;
            this.FrienderId = friend.Friender.Id;
            this.FriendRequestStatus = friend.FriendRequestStatus;
            this.FollowRequestStatus = friend.FollowRequestStatus;
        }
    }
}