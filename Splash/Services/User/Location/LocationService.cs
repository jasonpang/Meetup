using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Splash.Extensions;
using ServiceStack.FluentValidation;

using LocationModel = Splash.Model.Entities.Location;
using UserModel = Splash.Model.Entities.User;

namespace Splash.Services.User.Location
{
    [Route("/User/Location/{UserId}", "GET")]
    public class GetLocations : IReturn<List<LocationModel>>
    {
        public int UserId { get; set; }
        public long? MinTimestamp { get; set; }
        public long? MaxTimestamp { get; set; }
        public int? Last { get; set; }
    }

    public class GetLocationsValidator : AbstractValidator<GetLocations>
    {
        public GetLocationsValidator()
        {
            RuleFor(x => x.MinTimestamp).GreaterThan(0).LessThan(DateTime.Now.ToTimestamp());
            RuleFor(x => x.MaxTimestamp).GreaterThan(0).LessThanOrEqualTo(DateTime.Now.ToTimestamp());
            RuleFor(x => x.Last).GreaterThan(0);
        }
    }

    [Route("/User/Location/{UserId}", "POST")]
    public class AddLocation : IReturn<LocationModel>
    {
        public int UserId { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }

    public class AddLocationValidator : AbstractValidator<AddLocation>
    {
        public AddLocationValidator()
        {
            RuleFor(x => x.Latitude).NotEmpty().GreaterThanOrEqualTo(-90).LessThanOrEqualTo(90);
            RuleFor(x => x.Longitude).NotEmpty().GreaterThanOrEqualTo(-180).LessThanOrEqualTo(180);
        }
    }

    public class LocationService : Service
    {
        public IValidator<GetLocations> GetLocationsValidator { get; set; }
        public IValidator<AddLocation> AddLocationValidator { get; set; }

        public object Get(GetLocations request)
        {
            this.GetLocationsValidator.ValidateAndThrow(request);

            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var user = session.Get<UserModel>(request.UserId);

                    var locations = user.Locations.Where(location =>
                        location.Timestamp >= (request.MinTimestamp.HasValue ? request.MinTimestamp.Value : 0) &&
                        location.Timestamp <= (request.MaxTimestamp.HasValue ? request.MaxTimestamp.Value : DateTime.Now.ToTimestamp()));

                    if (request.Last.HasValue)
                    {
                        return locations.LastN(request.Last.Value);
                    }
                    else
                    {
                        return locations;
                    }
                }
            }
        }

        public object Post(AddLocation request)
        {
            this.AddLocationValidator.ValidateAndThrow(request);

            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var location = new LocationModel();

                    location.User = session.Get<UserModel>(request.UserId);
                    location.Timestamp = DateTime.Now.ToTimestamp();
                    location.Latitude = request.Latitude;
                    location.Longitude = request.Longitude;

                    session.SaveOrUpdate(location);

                    transaction.Commit();

                    return location;
                }
            }
        }
    }
}