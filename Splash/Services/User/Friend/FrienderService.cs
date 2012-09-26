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
    /// <summary>
    /// By supplying only a {FrienderId}, all friends of the user will be retrieved.
    /// By supplying both a {FrienderId} and a {FriendeeId), only the specific Friendee will be retrieved.
    /// </summary>
    [Route("/User/{FrienderId}/Friend", "GET")]
    public class GetFriends : IReturn<List<FriendModel>>
    {
        public int FrienderId { get; set; }
        public int? FriendeeId { get; set; }
    }

    [Route("/User/{FrienderId}/Friend/{FriendeeId}", "POST")]
    public class AddFriend : IReturn<FriendModel>
    {
        public int FrienderId { get; set; }
        public int FriendeeId { get; set; }
    }

    [Route("/User/{FrienderId}/Friend/{FriendeeId}", "DELETE")]
    public class RemoveFriend
    {
        public int FrienderId { get; set; }
        public int FriendeeId { get; set; }
    }

    public class FrienderService : Service
    {
        public object Get(GetFriends request)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    if (request.FriendeeId.HasValue)
                    {
                        // Only return one Friend
                        var friendQuery = session.QueryOver<FriendModel>()
                            .Where(table => table.Friender.Id == request.FrienderId)
                            .And(table => table.Friendee.Id == request.FriendeeId);

                        return friendQuery.SingleOrDefault<FriendModel>();
                    }
                    else
                    {
                        // Return all Friends for this user
                        var friendQuery = session.QueryOver<FriendModel>()
                            .Where(table => table.Friender.Id == request.FrienderId);

                        return friendQuery.List<FriendModel>();
                    }
                }
            }
        }

        public object Post(AddFriend request)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var friend = new FriendModel()
                    {
                        Friendee = session.Get<UserModel>(request.FriendeeId),
                        Friender = session.Get<UserModel>(request.FrienderId),
                        FriendRequestStatus = FriendRequestStatus.Pending,
                        FollowRequestStatus = FollowRequestStatus.Uninitiated,
                    };

                    session.SaveOrUpdate(friend);

                    return friend;
                }
            }
        }

        public void Delete(RemoveFriend request)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var friendQuery = session.QueryOver<FriendModel>()
                        .Where(table => table.Friender.Id == request.FrienderId)
                        .And(table => table.Friendee.Id == request.FriendeeId);

                    session.Delete(friendQuery.SingleOrDefault());
                }
            }
        }
    }
}