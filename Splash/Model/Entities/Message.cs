using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Splash.Model.Entities
{
    public enum MessageType
    {
        /// <summary>
        /// This message cannot be responded to.
        /// </summary>
        OK,
        /// <summary>
        /// This message can have a response of "Yes" or "No".
        /// </summary>
        YesNo,
    }

    public class Message
    {
        public virtual int Id { get; protected set; }

        public virtual User User { get; set; }
        public virtual long Timestamp { get; set; }

        public virtual MessageType MessageType { get; set; }
        public virtual String Content { get; set; }
        public virtual bool IsRead { get; set; }
        public virtual String YesActionUri { get; set; }
        public virtual String OKActionUri { get; set; }
        public virtual String NoActionUri { get; set; }

        public Message()
        {
            Timestamp = default(long);

            MessageType = MessageType.OK;
            Content = String.Empty;
            IsRead = default(bool);
            YesActionUri = String.Empty;
            OKActionUri = String.Empty;
            NoActionUri = String.Empty;
        }
    }
}

/*
 * Examples of Notifications
 * 
 * 1. Sebastian Liu wants to be your friend. Is this okay?  (ActionUri: POST/DELETE http://localhost/user/1/friend/2/friendrequest)
 * 2. Sebastian Liu wants to follow your location. Is this okay? (ActionUri: POST/DELETE http://localhost/user/1/friend/2/followrequest)
 * 3. Sebastian Liu has removed you as a friend. (ActionUri: None)
 * 
*/