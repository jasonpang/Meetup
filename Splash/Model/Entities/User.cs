using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Splash.Extensions;

namespace Splash.Model.Entities
{
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
        }
    }
}