using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Splash.Extensions;
using Splash.Model.Entities;

namespace Splash.Model.Dto
{
    public class UserDto
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

        public virtual IList<LocationDto> Locations { get; set; }

        public virtual IList<FriendDto> Friends { get; set; }

        public UserDto()
        {
            LastLoggedIn = default(long);
            Created = default(long);

            FirstName = String.Empty;
            LastName = String.Empty;
            Nickname = String.Empty;
            PhoneNumber = String.Empty;
            Email = String.Empty;
            Password = String.Empty;

            Locations = new List<LocationDto>();

            Friends = new List<FriendDto>();
        }

        public UserDto(User user)
        {
            this.Id = user.Id;
            this.LastLoggedIn = user.LastLoggedIn;
            this.Created = user.Created;
            this.FirstName = user.FirstName;
            this.LastName = user.LastName;
            this.Nickname = user.Nickname;
            this.PhoneNumber = user.PhoneNumber;
            this.Email = user.Email;
            this.Password = user.Password;

            this.Locations = new List<LocationDto>();
            user.Locations.ToList().ForEach(location => this.Locations.Add(new LocationDto(location)));

            this.Friends = new List<FriendDto>();
            user.Friends.ToList().ForEach(friend => this.Friends.Add(new FriendDto(friend)));
        }
    }
}