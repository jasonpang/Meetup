using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Splash.Extensions;

namespace Splash.Model.Entities
{
    [Flags]
    public enum Privileges
    {
        /// <summary>
        /// Can only read from own resources. e.g. GET /User?userId=1, where 1 is the user's id
        /// </summary>
        Standard = 1 << 0,
        /// <summary>
        /// Can read from other resources, but cannot write. e.g. GET /User?userId=2, where 2 is another user's id
        /// </summary>
        CanReadOtherUserResources = 1 << 1,
        /// <summary>
        /// Can read and write from and to other resources, includes delete. e.g. DELETE /User?userId=2, where 2 is another user's id
        /// </summary>
        CanReadWriteOtherUserResources = 1 << 2,
        /// <summary>
        /// Can read and write from the database directly. e.g. DELETE /Database
        /// </summary>
        CanInterfaceWithDatabaseDirectly = 1 << 3,
        /// <summary>
        /// No API call rate limiting.
        /// </summary>
        NoRateLimit = 1 << 4,
    }

    public class User
    {
        public virtual int Id { get; protected set; }

        public virtual long LastLoggedIn { get; set; }
        public virtual long Created { get; set; }

        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string Nickname { get; set; }
        public virtual string PhoneNumber { get; set; }
        public virtual string Email { get; set; }
        public virtual string Password { get; set; }

        public virtual IList<Location> Locations { get; set; }

        public virtual IList<Friend> Friends { get; set; }

        public virtual IList<Message> Messages { get; set; }

        public virtual Privileges Privileges { get; set; }

        public User()
        {
            LastLoggedIn = default(long);
            Created = default(long);

            FirstName = String.Empty;
            LastName = String.Empty;
            Nickname = String.Empty;
            PhoneNumber = String.Empty;
            Email = String.Empty;
            Password = String.Empty;

            Locations = new List<Location>();

            Friends = new List<Friend>();

            Messages = new List<Message>();
        }
    }
}