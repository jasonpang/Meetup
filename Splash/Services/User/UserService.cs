using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Splash.Extensions;
using ServiceStack.FluentValidation;
using ServiceStack.Common.Utils;

using UserModel = Splash.Model.Entities.User;
using Splash.Model.Dto;

namespace Splash.Services.User
{
    [Route("/User/{Id}", "GET")]
    public class RetrieveUser : IReturn<UserModel>
    {
        public int Id { get; set; }
    }

    [Route("/User", "PUT")]
    public class CreateUser : IReturn<UserModel>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Nickname { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class CreateUserValidator : AbstractValidator<CreateUser>
    {
        public CreateUserValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().Length(1, 35);
            RuleFor(x => x.LastName).NotEmpty().Length(1, 35);
            RuleFor(x => x.Nickname).NotEmpty().Length(1, 25);
            RuleFor(x => x.PhoneNumber).NotEmpty().Length(1, 12);
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty().Length(1, 35);
        }
    }

    [Route("/User/{Id}", "PATCH")]
    public class UpdateUser : IReturn<UserModel>
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Nickname { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class UpdateUserValidator : AbstractValidator<UpdateUser>
    {
        public UpdateUserValidator()
        {
            RuleFor(x => x.FirstName).SetValidator(new CreateUserValidator());
            RuleFor(x => x.LastName).SetValidator(new CreateUserValidator());
            RuleFor(x => x.Nickname).SetValidator(new CreateUserValidator());
            RuleFor(x => x.PhoneNumber).SetValidator(new CreateUserValidator());
            RuleFor(x => x.Email).SetValidator(new CreateUserValidator());
            RuleFor(x => x.Password).SetValidator(new CreateUserValidator());
        }
    }

    [Route("/User/{Id}", "DELETE")]
    public class DeleteUser
    {
        public int Id { get; set; }
    }

    public class UserService : Service
    {
        public IValidator<CreateUser> CreateUserValidator { get; set; }
        public IValidator<UpdateUser> UpdateUserValidator { get; set; }

        public object Get(RetrieveUser request)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var user = session.Get<UserModel>(request.Id);

                    transaction.Commit();

                    var userDto = new UserDto(user);
                    return userDto;
                }
            }
        }

        public object Patch(UpdateUser request)
        {
            //this.UpdateUserValidator.ValidateAndThrow(request);

            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var user = session.Get<UserModel>(request.Id);

                    if (request.FirstName.IsSet())
                        user.FirstName = request.FirstName;
                    if (request.LastName.IsSet())
                        user.LastName = request.LastName;
                    if (request.Nickname.IsSet())
                        user.Nickname = request.Nickname;
                    if (request.PhoneNumber.IsSet())
                        user.PhoneNumber = request.PhoneNumber;
                    if (request.Email.IsSet())
                        user.Email = request.Email;
                    if (request.Password.IsSet())
                        user.Password = request.Password;

                    session.SaveOrUpdate(user);

                    transaction.Commit();

                    return new UserDto(user);
                }
            }
        }

        public object Put(CreateUser request)
        {
            //this.CreateUserValidator.ValidateAndThrow(request);

            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var user = new UserModel();

                    user.Created = DateTime.Now.ToTimestamp();

                    if (request.FirstName.IsSet())
                        user.FirstName = request.FirstName;
                    if (request.LastName.IsSet())
                        user.LastName = request.LastName;
                    if (request.Nickname.IsSet())
                        user.Nickname = request.Nickname;
                    if (request.PhoneNumber.IsSet())
                        user.PhoneNumber = request.PhoneNumber;
                    if (request.Email.IsSet())
                        user.Email = request.Email;
                    if (request.Password.IsSet())
                        user.Password = request.Password;

                    session.SaveOrUpdate(user);
                    transaction.Commit();

                    return new UserDto(user);
                }
            }
        }

        public void Delete(DeleteUser request)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var user = session.Get<UserModel>(request.Id);

                    session.Delete(user);

                    transaction.Commit();
                }
            }
        }
    }
}