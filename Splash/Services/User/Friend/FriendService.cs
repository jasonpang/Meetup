using ServiceStack.FluentValidation;
using ServiceStack.ServiceHost;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.ServiceInterface;
using Splash.Extensions;
using Splash.Model.Entities;
using UserModel = Splash.Model.Entities.User;
using FriendModel = Splash.Model.Entities.Friend;
using Splash.Model.Dto;

namespace Splash.Services.User.Friend
{
    /// <summary>
    /// By supplying only a {UserId}, all friends of the user will be retrieved.
    /// By supplying both a {UserId} and a {FriendUserId), only the specific Friendee will be retrieved.
    /// </summary>
    [Route("/User/Friend", "GET")]
    public class GetFriends : IReturn<List<FriendModel>>
    {
        public int UserId { get; set; }
        public int? FriendUserId { get; set; }
    }

    public class GetFriendsValidator : AbstractValidator<GetFriends>
    {
        public GetFriendsValidator()
        {
            RuleFor(x => x.UserId).EnsureValidId();
            RuleFor(x => x.FriendUserId).EnsureValidOptionalId();
        }
    }

    [Route("/User/Friend", "DELETE")]
    public class RemoveFriend
    {
        public int UserId { get; set; }
        public int FriendUserId { get; set; }
    }

    public class RemoveFriendValidator : AbstractValidator<RemoveFriend>
    {
        public RemoveFriendValidator()
        {
            RuleFor(x => x.UserId).EnsureValidId();
            RuleFor(x => x.FriendUserId).EnsureValidId();
        }
    }

    [Route("/User/Friend/FriendRequest", "PUT")]
    public class RequestFriend : IReturn<FriendModel>
    {
        public int UserId { get; set; }
        public int FriendUserId { get; set; }
    }

    public class RequestFriendValidator : AbstractValidator<RequestFriend>
    {
        public RequestFriendValidator()
        {
            RuleFor(x => x.UserId).EnsureValidId();
            RuleFor(x => x.FriendUserId).EnsureValidId();
        }
    }

    public class FriendService : Service
    {
        public IValidator<GetFriends> GetFriendsValidator { get; set; }
        public IValidator<RemoveFriend> RemoveFriendValidator { get; set; }
        public IValidator<RequestFriend> RequestFriendValidator { get; set; }

        public object Get(GetFriends request)
        {
            GetFriendsValidator.ValidateAndThrow(request);

            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    if (request.FriendUserId.HasValue)
                    {
                        // Only return one Friend
                        var friendQuery = session.QueryOver<FriendModel>()
                            .Where(row => row.Friender.Id == request.UserId)
                            .And(row => row.Friendee.Id == request.FriendUserId);

                        transaction.Commit();

                        var friend = friendQuery.SingleOrDefault<FriendModel>();

                        return new FriendDto(friend);
                    }
                    else
                    {
                        // Return all Friends for this user
                        var friendQuery = session.QueryOver<FriendModel>()
                            .Where(row => row.Friender.Id == request.UserId);

                        transaction.Commit();

                        var friends = friendQuery.List<FriendModel>();

                        var friendsDto = new List<FriendDto>();
                        friends.ToList().ForEach(friend => friendsDto.Add(new FriendDto(friend)));
                        return friendsDto;
                    }
                }
            }
        }

        public void Delete(RemoveFriend request)
        {
            RemoveFriendValidator.ValidateAndThrow(request);

            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var friendQuery = session.QueryOver<FriendModel>()
                        .Where(row => row.Friender.Id == request.UserId)
                        .And(row => row.Friendee.Id == request.FriendUserId);

                    var converseQuery = session.QueryOver<FriendModel>()
                        .Where(row => row.Friender.Id == request.FriendUserId)
                        .And(row => row.Friendee.Id == request.UserId);

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

        public object Put(RequestFriend request)
        {
            RequestFriendValidator.ValidateAndThrow(request);

            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var friend = new FriendModel()
                    {
                        Friendee = session.Get<UserModel>(request.FriendUserId),
                        Friender = session.Get<UserModel>(request.UserId),
                        FriendRequestStatus = FriendRequestStatus.Pending,
                        FollowRequestStatus = FollowRequestStatus.Uninitiated,
                    };

                    session.SaveOrUpdate(friend);

                    transaction.Commit();

                    return new FriendDto(friend);
                }
            }
        }
    }
}