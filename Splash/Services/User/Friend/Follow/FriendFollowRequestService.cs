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

namespace Splash.Services.User.Friend.Follow
{
    [Route("/User/Friend/FollowRequest", "PUT")]
    public class RequestFollowFriend : IReturn<FriendModel>
    {
        public int UserId { get; set; }
        public int FriendUserId { get; set; }
    }

    public class RequestFollowFriendValidator : AbstractValidator<RequestFollowFriend>
    {
        public RequestFollowFriendValidator()
        {
            RuleFor(x => x.UserId).EnsureValidId();
            RuleFor(x => x.FriendUserId).EnsureValidId();
        }
    }

    [Route("/User/Friend/FollowRequest", "POST")]
    public class AcceptFollowRequest : IReturn<FriendModel>
    {
        public int UserId { get; set; }
        public int FriendUserId { get; set; }
    }

    public class AcceptFollowRequestValidator : AbstractValidator<AcceptFollowRequest>
    {
        public AcceptFollowRequestValidator()
        {
            RuleFor(x => x.UserId).EnsureValidId();
            RuleFor(x => x.FriendUserId).EnsureValidId();
        }
    }

    [Route("/User/Friend/FollowRequest", "DELETE")]
    public class RejectFollowRequest : IReturn<FriendModel>
    {
        public int FriendUserId { get; set; }
        public int UserId { get; set; }
    }

    public class RejectFollowRequestValidator : AbstractValidator<RejectFollowRequest>
    {
        public RejectFollowRequestValidator()
        {
            RuleFor(x => x.UserId).EnsureValidId();
            RuleFor(x => x.FriendUserId).EnsureValidId();
        }
    }

    public class FriendFollowRequestService : Service
    {
        public IValidator<RequestFollowFriend> RequestFollowFriendValidator { get; set; }
        public IValidator<AcceptFollowRequest> AcceptFollowRequestValidator { get; set; }
        public IValidator<RejectFollowRequest> RejectFollowRequestValidator { get; set; }

        public object Put(RequestFollowFriend request)
        {
            RequestFollowFriendValidator.ValidateAndThrow(request);

            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var friendQuery = session.QueryOver<FriendModel>()
                        .Where(row => row.Friender.Id == request.UserId)
                        .And(row => row.Friendee.Id == request.FriendUserId);

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
            AcceptFollowRequestValidator.ValidateAndThrow(request);

            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var friendQuery = session.QueryOver<FriendModel>()
                        .Where(row => row.Friender.Id == request.FriendUserId)
                        .And(row => row.Friendee.Id == request.UserId);

                    var friend = friendQuery.SingleOrDefault<FriendModel>();

                    if (friend.FollowRequestStatus == FollowRequestStatus.Pending)
                    {
                        friend.FollowRequestStatus = FollowRequestStatus.Accepted;
                    }

                    session.Update(friend);

                    transaction.Commit();

                    return new FriendDto(friend);
                }
            }
        }

        public object Delete(RejectFollowRequest request)
        {
            RejectFollowRequestValidator.ValidateAndThrow(request);

            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var friendQuery = session.QueryOver<FriendModel>()
                        .Where(row => row.Friender.Id == request.UserId)
                        .And(row => row.Friendee.Id == request.FriendUserId);

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