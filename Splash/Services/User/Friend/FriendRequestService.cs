using ServiceStack.FluentValidation;
using ServiceStack.ServiceHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceStack.ServiceInterface;
using Splash.Extensions;
using Splash.Model.Entities;

using UserModel = Splash.Model.Entities.User;
using FriendModel = Splash.Model.Entities.Friend;
using Splash.Model.Dto;

namespace Splash.Services.User.Friend
{
    [Route("/User/Friend/FriendRequest", "POST")]
    public class AcceptFriendRequest : IReturn<FriendModel>
    {
        public int UserId { get; set; }
        public int FriendUserId { get; set; }
    }

    public class AcceptFriendRequestValidator : AbstractValidator<AcceptFriendRequest>
    {
        public AcceptFriendRequestValidator()
        {
            RuleFor(x => x.UserId).EnsureValidId();
            RuleFor(x => x.FriendUserId).EnsureValidId();
        }
    }

    [Route("/User/Friend/FriendRequest", "DELETE")]
    public class RejectFriendRequest
    {
        public int UserId { get; set; }
        public int FriendUserId { get; set; }
    }

    public class RejectFriendRequestValidator : AbstractValidator<RejectFriendRequest>
    {
        public RejectFriendRequestValidator()
        {
            RuleFor(x => x.UserId).EnsureValidId();
            RuleFor(x => x.FriendUserId).EnsureValidId();
        }
    }

    public class FriendRequestService : Service
    {
        public IValidator<AcceptFriendRequest> AcceptFriendRequestValidator { get; set; }
        public IValidator<RejectFriendRequest> RejectFriendRequestValidator { get; set; }

        public object Post(AcceptFriendRequest request)
        {
            AcceptFriendRequestValidator.ValidateAndThrow(request);

            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var friendQuery = session.QueryOver<FriendModel>()
                        .Where(row => row.Friendee.Id == request.FriendUserId)
                        .And(row => row.Friender.Id == request.UserId);

                    var friend = friendQuery.SingleOrDefault<FriendModel>();

                    if (friend.FriendRequestStatus != FriendRequestStatus.Accepted)
                    {
                        friend.FriendRequestStatus = FriendRequestStatus.Accepted;
                        session.SaveOrUpdate(friend);
                    }

                    var converseFriendQuery = session.QueryOver<FriendModel>()
                        .Where(row => row.Friendee.Id == request.UserId)
                        .And(row => row.Friender.Id == request.FriendUserId);

                    var converseFriend = converseFriendQuery.SingleOrDefault<FriendModel>();

                    if (converseFriend == null)
                    {
                        // Set up the converse friendship
                        var newConverseFriend = new FriendModel()
                                                    {
                                                        Friender = session.Get<UserModel>(request.FriendUserId),
                                                        Friendee = session.Get<UserModel>(request.UserId),
                                                        FriendRequestStatus = FriendRequestStatus.Accepted,
                                                        FollowRequestStatus = FollowRequestStatus.Uninitiated,
                                                    };

                        session.SaveOrUpdate(newConverseFriend);
                    }

                    transaction.Commit();

                    return new FriendDto(friend);
                }
            }
        }

        public void Delete(RejectFriendRequest request)
        {
            RejectFriendRequestValidator.ValidateAndThrow(request);

            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var friendQuery = session.QueryOver<FriendModel>()
                        .Where(row => row.Friendee.Id == request.UserId)
                        .And(row => row.Friender.Id == request.FriendUserId);

                    var friend = friendQuery.SingleOrDefault();

                    var owningUser = session.Get<UserModel>(friend.Friender.Id);

                    if (friend.FriendRequestStatus == FriendRequestStatus.Pending)
                    {
                        owningUser.Friends.Remove(friend);
                        session.Delete(friend);
                    }

                    transaction.Commit();
                }
            }
        }
    }
}