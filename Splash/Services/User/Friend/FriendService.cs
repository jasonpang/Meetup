using ServiceStack.ServiceHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceStack.ServiceInterface;
using Splash.Model.Entities;

using UserModel = Splash.Model.Entities.User;
using FriendModel = Splash.Model.Entities.Friend;
using Splash.Model.Dto;

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

    [Route("/User/{FrienderId}/Friend/{FriendeeId}/FriendRequest", "PUT")]
    public class RequestFriend : IReturn<FriendModel>
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

    public class FriendService : Service
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
                            .Where(row => row.Friender.Id == request.FrienderId)
                            .And(row => row.Friendee.Id == request.FriendeeId);

                        transaction.Commit();

                        var friend = friendQuery.SingleOrDefault<FriendModel>();

                        return new FriendDto(friend);
                    }
                    else
                    {
                        // Return all Friends for this user
                        var friendQuery = session.QueryOver<FriendModel>()
                            .Where(row => row.Friender.Id == request.FrienderId);

                        transaction.Commit();

                        var friends = friendQuery.List<FriendModel>();

                        var friendsDto = new List<FriendDto>();
                        friends.ToList().ForEach(friend => friendsDto.Add(new FriendDto(friend)));
                        return friendsDto;
                    }
                }
            }
        }

        public object Put(RequestFriend request)
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

                    transaction.Commit();

                    return new FriendDto(friend);
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
                        .Where(row => row.Friender.Id == request.FrienderId)
                        .And(row => row.Friendee.Id == request.FriendeeId);

                    var converseQuery = session.QueryOver<FriendModel>()
                        .Where(row => row.Friender.Id == request.FriendeeId)
                        .And(row => row.Friendee.Id == request.FrienderId);

                    var initiatorFriend = friendQuery.SingleOrDefault();
                    var invitedFriend = converseQuery.SingleOrDefault();

                    var initiatorUser = session.Get<UserModel>(initiatorFriend.Friender.Id);
                    var invitedUser = session.Get<UserModel>(initiatorFriend.Friendee.Id);

                    initiatorUser.Friends.Remove(initiatorFriend);
                    invitedUser.Friends.Remove(invitedFriend);

                    session.Delete(initiatorFriend);
                    session.Delete(invitedFriend);

                    transaction.Commit();
                }
            }
        }
    }
}