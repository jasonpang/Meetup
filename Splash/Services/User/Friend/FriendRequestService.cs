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
    [Route("/User/{FriendeeId}/Friend/{FrienderId}/FriendRequest", "POST")]
    public class AcceptFriendRequest : IReturn<FriendModel>
    {
        public int FriendeeId { get; set; }
        public int FrienderId { get; set; }
    }

    [Route("/User/{InvitedFriendId}/Friend/{InitiatorId}/FriendRequest", "DELETE")]
    public class RejectFriendRequest
    {
        public int InvitedFriendId { get; set; }
        public int InitiatorId { get; set; }
    }

    public class FriendRequestService : Service
    {
        public object Post(AcceptFriendRequest request)
        {   
            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var friendQuery = session.QueryOver<FriendModel>()
                        .Where(row => row.Friendee.Id == request.FriendeeId)
                        .And(row => row.Friender.Id == request.FrienderId);

                    var friend = friendQuery.SingleOrDefault<FriendModel>();

                    friend.FriendRequestStatus = FriendRequestStatus.Accepted;

                    session.Update(friend);

                    // Set up the converse friendship
                    var converseFriend = new FriendModel()
                    {
                        Friender = session.Get<UserModel>(request.FriendeeId),
                        Friendee = session.Get<UserModel>(request.FrienderId),
                        FriendRequestStatus = FriendRequestStatus.Accepted,
                        FollowRequestStatus = FollowRequestStatus.Uninitiated,
                    };

                    session.SaveOrUpdate(converseFriend);

                    transaction.Commit();

                    return new FriendDto(friend);
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
                        .Where(row => row.Friendee.Id == request.InvitedFriendId)
                        .And(row => row.Friender.Id == request.InitiatorId);

                    var friend = friendQuery.SingleOrDefault();

                    var owningUser = session.Get<UserModel>(friend.Friender.Id);
                    owningUser.Friends.Remove(friend);

                    session.Delete(friend);

                    transaction.Commit();
                }
            }
        }
    }
}