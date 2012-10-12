using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;
using System;
using Splash.Extensions;
using ServiceStack.FluentValidation;
using UserModel = Splash.Model.Entities.User;
using Splash.Model.Dto;

namespace Splash.Services.User
{
    [Route("/User", "GET")]
    public class RetrieveUser : IReturn<UserModel>
    {
        public int UserId { get; set; }
    }

    public class RetrieveUserValidator : AbstractValidator<RetrieveUser>
    {
        public RetrieveUserValidator()
        {
            RuleFor(x => x.UserId).EnsureValidId();
        }
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
            RuleFor(x => x.FirstName).EnsureValidName(TypeOfName.First).When(x => x.FirstName != null);
            RuleFor(x => x.LastName).EnsureValidName(TypeOfName.Last).When(x => x.LastName != null);
            RuleFor(x => x.Nickname).EnsureValidName(TypeOfName.Nickname).When(x => x.Nickname != null);
            RuleFor(x => x.PhoneNumber).EnsureValidPhoneNumber();
            RuleFor(x => x.Email).EnsureValidEmail();
            RuleFor(x => x.Password).EnsureValidPassword();
        }
    }

    [Route("/User", "PATCH")]
    public class UpdateUser : IReturn<UserModel>
    {
        public int UserId { get; set; }
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
            RuleFor(x => x.UserId).EnsureValidId();
            RuleFor(x => x.FirstName).EnsureValidName(TypeOfName.First).When(x => x.FirstName != null);
            RuleFor(x => x.LastName).EnsureValidName(TypeOfName.Last).When(x => x.LastName != null);
            RuleFor(x => x.Nickname).EnsureValidName(TypeOfName.Nickname).When(x => x.Nickname != null);
            RuleFor(x => x.PhoneNumber).EnsureValidPhoneNumber().When(x => x.PhoneNumber != null);
            RuleFor(x => x.Email).EnsureValidEmail().When(x => x.Email != null);
            RuleFor(x => x.Password).EnsureValidPassword().When(x => x.Password != null);
        }
    }

    [Route("/User/{UserId}", "DELETE")]
    public class DeleteUser
    {
        public int UserId { get; set; }
    }

    public class DeleteUserValidator : AbstractValidator<RetrieveUser>
    {
        public DeleteUserValidator()
        {
            RuleFor(x => x.UserId).EnsureValidId();
        }
    }

    public class UserService : Service
    {
        public IValidator<RetrieveUser> RetrieveUserValidator { get; set; }
        public IValidator<UpdateUser> UpdateUserValidator { get; set; }
        public IValidator<CreateUser> CreateUserValidator { get; set; }
        public IValidator<DeleteUser> DeleteUserValidator { get; set; }

        public object Get(RetrieveUser request)
        {
            RetrieveUserValidator.ValidateAndThrow(request);

            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var user = session.Get<UserModel>(request.UserId);

                    transaction.Commit();

                    var userDto = new UserDto(user);
                    return userDto;
                }
            }
        }

        public object Patch(UpdateUser request)
        {
            UpdateUserValidator.ValidateAndThrow(request);

            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var user = session.Get<UserModel>(request.UserId);

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
            CreateUserValidator.ValidateAndThrow(request);

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
            DeleteUserValidator.ValidateAndThrow(request);

            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var user = session.Get<UserModel>(request.UserId);

                    session.Delete(user);

                    transaction.Commit();
                }
            }
        }
    }
}