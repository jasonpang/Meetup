using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceStack.ServiceInterface;
using Splash.Model.Dto;
using ServiceStack.ServiceHost;
using Splash.Model.Entities;
using Splash.Extensions;

using UserModel = Splash.Model.Entities.User;

namespace Splash.Services.User.Messages
{
    [Route("/User/{UserId}/Message", "GET")]
    public class GetMessages : IReturn<List<MessageDto>>
    {
        public virtual int UserId { get; set; }
        public virtual long? MinTimestamp { get; set; }
        public virtual long? MaxTimestamp { get; set; }
        public virtual int? Last { get; set; }
        public virtual MessageType? MessageType { get; set; }
        public virtual bool? IsRead { get; set; }
    }

    [Route("/User/{UserId}/Message", "PUT")]
    public class CreateMessage : IReturn<MessageDto>
    {
        public virtual int UserId { get; set; }
        public virtual MessageType MessageType { get; set; }
        public virtual String Content { get; set; }
        public virtual bool IsRead { get; set; }
        public virtual String YesActionUri { get; set; }
        public virtual String OKActionUri { get; set; }
        public virtual String NoActionUri { get; set; }
    }

    [Route("/User/{UserId}/Message/{MessageId}", "PATCH")]
    public class UpdateMessage : IReturn<MessageDto>
    {
        public virtual int UserId { get; set; }
        public virtual int MessageId { get; set; }
        public virtual bool IsRead { get; set; }
    }

    public class MessageService : Service
    {
        public object Get(GetMessages request)
        {
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
                    
                    var messages = (request.Last.HasValue)
                        ? messagesQuery.List().TakeLastN(request.Last.Value)
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
            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var message = new Message();

                    message.User = session.Get<UserModel>(request.UserId);
                    message.Timestamp = DateTime.Now.ToTimestamp();
                    message.Content = request.Content;
                    message.IsRead = request.IsRead;
                    message.MessageType = request.MessageType;
                    message.NoActionUri = request.NoActionUri.IsSet() ? request.NoActionUri : String.Empty;
                    message.OKActionUri = request.OKActionUri.IsSet() ? request.OKActionUri : String.Empty;
                    message.YesActionUri = request.YesActionUri.IsSet() ? request.YesActionUri : String.Empty;
                    
                    session.SaveOrUpdate(message);

                    transaction.Commit();

                    return new MessageDto(message);
                }
            }
        }

        public object Patch(UpdateMessage request)
        {
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