using Splash.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Splash.Model.Dto
{
    public class MessageDto
    {
        public virtual int Id { get; protected set; }

        public virtual int UserId { get; set; }
        public virtual long Timestamp { get; set; }

        public virtual MessageType MessageType { get; set; }
        public virtual String Content { get; set; }
        public virtual bool IsRead { get; set; }
        public virtual String YesActionUri { get; set; }
        public virtual String OKActionUri { get; set; }
        public virtual String NoActionUri { get; set; }

        public MessageDto()
        {
            UserId = default(int);
            Timestamp = default(long);

            Content = String.Empty;
            IsRead = default(bool);
            YesActionUri = String.Empty;
            OKActionUri = String.Empty;
            NoActionUri = String.Empty;
        }

        public MessageDto(Message message)
        {
            this.Id = message.Id;
            this.UserId = message.User.Id;
            this.Timestamp = message.Timestamp;
            this.MessageType = message.MessageType;
            this.Content = message.Content;
            this.IsRead = message.IsRead;
            this.YesActionUri = message.YesActionUri;
            this.OKActionUri = message.OkActionUri;
            this.NoActionUri = message.NoActionUri;
        }
    }
}