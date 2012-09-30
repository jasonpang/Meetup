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

namespace Splash.Services.User.Friend.Follow
{
    [Route("/User/{InitatorId}/Friend/{FriendId}/FollowRequest", "PUT")]
    public class RequestFollowFriend : IReturn<FriendModel>
    {
        public int InitatorId { get; set; }
        public int FriendId { get; set; }
    }

    [Route("/User/{InvitedFriendId}/Friend/{InitiatorId}/FollowRequest", "POST")]
    public class AcceptFollowRequest : IReturn<FriendModel>
    {
        public int InitiatorId { get; set; }
        public int InvitedFriendId { get; set; }
    }

    [Route("/User/{InvitedFriendId}/Friend/{InitiatorId}/FollowRequest", "DELETE")]
    public class RejectFollowRequest : IReturn<FriendModel>
    {
        public int InvitedFriendId { get; set; }
        public int InitiatorId { get; set; }
    }

    public class FriendFollowRequestService : Service
    {
        public object Put(RequestFollowFriend request)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var friendQuery = session.QueryOver<FriendModel>()
                        .Where(row => row.Friender.Id == request.InitatorId)
                        .And(row => row.Friendee.Id == request.FriendId);

                    var friend = friendQuery.SingleOrDefault<FriendModel>();

                    friend.FollowRequestStatus = FollowRequestStatus.Pending;

                    session.Update(friend);

                    transaction.Commit();

                    return new FriendDto(friend);
                }
            }
        }

        public object Post(AcceptFollowRequest request)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var friendQuery = session.QueryOver<FriendModel>()
                        .Where(row => row.Friender.Id == request.InitiatorId)
                        .And(row => row.Friendee.Id == request.InvitedFriendId);

                    var friend = friendQuery.SingleOrDefault<FriendModel>();

                    friend.FollowRequestStatus = FollowRequestStatus.Accepted;

                    session.Update(friend);

                    transaction.Commit();

                    return new FriendDto(friend);
                }
            }
        }

        public object Delete(RejectFollowRequest request)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var friendQuery = session.QueryOver<FriendModel>()
                        .Where(row => row.Friender.Id == request.InitiatorId)
                        .And(row => row.Friendee.Id == request.InvitedFriendId);

                    var friend = friendQuery.SingleOrDefault<FriendModel>();

                    friend.FollowRequestStatus = FollowRequestStatus.Uninitiated;

                    session.Update(friend);

                    transaction.Commit();

                    return new FriendDto(friend);
                }
            }
        }
    }
}