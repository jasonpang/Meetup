using ServiceStack.ServiceHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceStack.ServiceInterface;
using Splash.Model.Entities;

using UserModel = Splash.Model.Entities.User;
using FriendModel = Splash.Model.Entities.Friend;

namespace Splash.Services.User.Friend
{
    [Route("/User/{UserId}/FriendRequest/{FriendUserId}", "POST")]
    public class RequestFollowFriend : IReturn<FriendModel>
    {
        public int UserId { get; set; }
        public int FriendUserId { get; set; }
    }

    [Route("/User/{UserId}/FriendRequest/{FriendUserId}", "DELETE")]
    public class RejectFriendRequest
    {
        public int UserId { get; set; }
        public int FriendUserId { get; set; }
    }

    public class FriendFollowRequestService : Service
    {
        public object Post(RequestFollowFriend request)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var friendQuery = session.QueryOver<FriendModel>()
                        .Where(table => table.Friender.Id == request.UserId)
                        .And(table => table.Friendee.Id == request.FriendUserId);

                    var friend = friendQuery.SingleOrDefault<FriendModel>();

                    friend.FrienderPermissions = FrienderPermissions.CanFollowFriendee;

                    session.Update(friend);

                    return friend;
                }
            }
        }

        public void Delete(RejectFriendRequest request)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var friendQuery = session.QueryOver<FriendModel>()
                        .Where(table => table.Friender.Id == request.UserId)
                        .And(table => table.Friendee.Id == request.FriendUserId);

                    var friend = friendQuery.SingleOrDefault<FriendModel>();

                    session.Delete(friend);
                }
            }
        }
    }
}