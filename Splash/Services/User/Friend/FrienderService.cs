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
    [Route("/User/{UserId}/Friend", "GET")]
    public class GetFriends : IReturn<List<FriendModel>>
    {
        public int UserId { get; set; }
        public int? FriendeeAndFrienderUserId { get; set; }
    }

    [Route("/User/{UserId}/Friend/{FriendeeUserId}", "POST")]
    public class AddFriend : IReturn<FriendModel>
    {
        public int UserId { get; set; }
        public int FriendeeUserId { get; set; }
    }

    [Route("/User/{UserId}/Friend/{FriendUserId}", "DELETE")]
    public class RemoveFriend
    {
        public int UserId { get; set; }
        public int FriendeeUserId { get; set; }
    }

    public class FrienderService : Service
    {
        public object Get(GetFriends request)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    if (request.FriendeeAndFrienderUserId.HasValue)
                    {
                        var friendQuery = session.QueryOver<FriendModel>()
                            .Where(table => table.Friender.Id == request.UserId)
                            .And(table => table.Friendee.Id == request.FriendeeUserId);

                        return friendQuery.SingleOrDefault<FriendModel>();
                    }
                    else
                    {
                        var friendQuery = session.QueryOver<FriendModel>()
                            .Where(table => table.Friender.Id == request.UserId);

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
                        Friendee = session.Get<UserModel>(request.FriendeeUserId),
                        Friender = session.Get<UserModel>(request.UserId),
                        //FrienderPermissions = FrienderPermissions.None,
                        //IsFriendRequestPending = true,
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
                        .Where(table => table.Friender.Id == request.UserId)
                        .And(table => table.Friendee.Id == request.FriendeeUserId);

                    session.Delete(friendQuery.SingleOrDefault());
                }
            }
        }
    }
}