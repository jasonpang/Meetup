using System;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.FluentValidation;
using ServiceStack.ServiceInterface;
using Splash.Model.Dto;
using ServiceStack.ServiceHost;
using Splash.Model.Entities;
using Splash.Extensions;
using UserModel = Splash.Model.Entities.User;

namespace Splash.Services.User.Messages
{
    [Route("/User/Message", "GET")]
    public class GetMessages : IReturn<List<MessageDto>>
    {
        public int UserId { get; set; }
        public long? MinTimestamp { get; set; }
        public long? MaxTimestamp { get; set; }
        public int? Limit { get; set; }
        public MessageType? MessageType { get; set; }
        public bool? IsRead { get; set; }
    }

    public class GetMessagesValidator : AbstractValidator<GetMessages>
    {
        public GetMessagesValidator()
        {
            RuleFor(x => x.UserId).EnsureValidId();
            RuleFor(x => x.MinTimestamp).EnsureValidTimestamp(isMaxTimestamp: false);
            RuleFor(x => x.MaxTimestamp).EnsureValidTimestamp(isMaxTimestamp: true);
            RuleFor(x => x.Limit).EnsureValidLimitByNumber();
        }
    }

    [Route("/User/Message", "PUT")]
    public class CreateMessage : IReturn<MessageDto>
    {
        public int UserId { get; set; }
        public MessageType? MessageType { get; set; }
        public String Content { get; set; }
        public String YesActionUri { get; set; }
        public String OkActionUri { get; set; }
        public String NoActionUri { get; set; }
        public bool? IsRead { get; set; }
    }

    public class CreateMessageValidator : AbstractValidator<CreateMessage>
    {
        public CreateMessageValidator()
        {
            RuleFor(x => x.UserId).EnsureValidId();
            RuleFor(x => x.MessageType).NotNull();
            RuleFor(x => x.Content).NotNull();
            RuleFor(x => x.YesActionUri).NotEmpty().When(x => x.MessageType == MessageType.YesNo);
            RuleFor(x => x.OkActionUri).NotEmpty().When(x => x.MessageType == MessageType.OK);
            RuleFor(x => x.NoActionUri).NotEmpty().When(x => x.MessageType == MessageType.YesNo);
        }
    }

    [Route("/User/Message", "PATCH")]
    public class UpdateMessage : IReturn<MessageDto>
    {
        public int? UserId { get; set; }
        public int MessageId { get; set; }
        public bool IsRead { get; set; }
    }

    public class UpdateMessageValidator : AbstractValidator<UpdateMessage>
    {
        public UpdateMessageValidator()
        {
            RuleFor(x => x.UserId).EnsureValidOptionalId();
            RuleFor(x => x.MessageId).EnsureValidId();
            RuleFor(x => x.IsRead).NotNull();
        }
    }

    public class MessageService : Service
    {
        public IValidator<GetMessages> GetMessagesValidator { get; set; }
        public IValidator<CreateMessage> CreateMessageValidator { get; set; }
        public IValidator<UpdateMessage> UpdateMessageValidator { get; set; }

        public object Get(GetMessages request)
        {
            GetMessagesValidator.ValidateAndThrow(request);

            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var messagesQuery = session.QueryOver<Message>()
                        .Where(table => table.User.Id == request.UserId);

                    if (request.IsRead.HasValue)
                    {
                        messagesQuery.And(table => table.IsRead == request.IsRead);
                    }

                    if (request.MessageType.HasValue)
                    {
                        messagesQuery.And(table => table.MessageType == request.MessageType);
                    }
                    
                    messagesQuery.And(table => table.Timestamp >= (request.MinTimestamp.HasValue ? request.MinTimestamp.Value : 0))
                        .And(table => table.Timestamp <= (request.MaxTimestamp.HasValue ? request.MaxTimestamp.Value : DateTime.Now.ToTimestamp()));
                    
                    var messages = (request.Limit.HasValue)
                        ? messagesQuery.List().TakeLastN(request.Limit.Value)
                        : messagesQuery.List();

                    var messagesDto = new List<MessageDto>();

                    messages.ToList().ForEach(message => messagesDto.Add(new MessageDto(message)));

                    transaction.Commit();

                    return messagesDto;
                }
            }
        }

        public object Put(CreateMessage request)
        {
            CreateMessageValidator.ValidateAndThrow(request);

            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var message = new Message();

                    message.User = session.Get<UserModel>(request.UserId);
                    message.Timestamp = DateTime.Now.ToTimestamp();
                    message.Content = request.Content;
                    message.IsRead = request.IsRead ?? false;
                    message.MessageType = request.MessageType.Value;
                    message.NoActionUri = request.NoActionUri.IsSet() ? request.NoActionUri : String.Empty;
                    message.OkActionUri = request.OkActionUri.IsSet() ? request.OkActionUri : String.Empty;
                    message.YesActionUri = request.YesActionUri.IsSet() ? request.YesActionUri : String.Empty;
                    
                    session.SaveOrUpdate(message);

                    transaction.Commit();

                    return new MessageDto(message);
                }
            }
        }

        public object Patch(UpdateMessage request)
        {
            UpdateMessageValidator.ValidateAndThrow(request);

            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var messagesQuery = session.QueryOver<Message>()
                        .Where(table => table.User.Id == request.UserId)
                        .And(table => table.Id == request.MessageId);

                    var message = messagesQuery.SingleOrDefault();

                    message.IsRead = request.IsRead;

                    session.SaveOrUpdate(message);

                    transaction.Commit();

                    return new MessageDto(message);
                }
            }
        }
    }
}