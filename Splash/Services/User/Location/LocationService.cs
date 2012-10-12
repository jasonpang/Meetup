using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using Splash.Authentication;
using Splash.Extensions;
using ServiceStack.FluentValidation;
using LocationModel = Splash.Model.Entities.Location;
using UserModel = Splash.Model.Entities.User;
using Splash.Model.Dto;

namespace Splash.Services.User.Location
{
    [Route("/User/Location", "GET")]
    public class GetLocations : IReturn<List<LocationModel>>
    {
        public int UserId { get; set; }
        public long? MinTimestamp { get; set; }
        public long? MaxTimestamp { get; set; }
        public int? Limit { get; set; }
    }

    public class GetLocationsValidator : AbstractValidator<GetLocations>
    {
        public GetLocationsValidator()
        {
            RuleFor(x => x.UserId).EnsureValidId();
            RuleFor(x => x.MinTimestamp).EnsureValidTimestamp(isMaxTimestamp: false);
            RuleFor(x => x.MaxTimestamp).EnsureValidTimestamp(isMaxTimestamp: true);
            RuleFor(x => x.Limit).EnsureValidLimitByNumber();
        }
    }

    [Route("/User/Location", "PUT")]
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
            RuleFor(x => x.UserId).EnsureValidId();
            RuleFor(x => x.Latitude).EnsureValidGeocoordinate(GeocoordinateType.Latitude);
            RuleFor(x => x.Longitude).EnsureValidGeocoordinate(GeocoordinateType.Latitude);
        }
    }

    [RequiresAuthentication]
    public class LocationService : Service
    {
        public IValidator<GetLocations> GetLocationsValidator { get; set; }
        public IValidator<AddLocation> AddLocationValidator { get; set; }

        public object Get(GetLocations request)
        {
            GetLocationsValidator.ValidateAndThrow(request);

            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var locationsQuery = session.QueryOver<LocationModel>()
                                       .Where(table => table.User.Id == request.UserId)
                                       .And(table => table.Timestamp >= (request.MinTimestamp.HasValue ? request.MinTimestamp.Value : 0))
                                       .And(table => table.Timestamp <= (request.MaxTimestamp.HasValue ? request.MaxTimestamp.Value : DateTime.Now.ToTimestamp()));

                    var locations = (request.Limit.HasValue)
                        ? locationsQuery.List().TakeLastN(request.Limit.Value)
                        : locationsQuery.List();

                    var locationsDto = new List<LocationDto>();

                    locations.ToList().ForEach(location => locationsDto.Add(new LocationDto(location)));

                    transaction.Commit();

                    return locationsDto;
                }
            }
        }

        public object Put(AddLocation request)
        {
            AddLocationValidator.ValidateAndThrow(request);

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

                    return new LocationDto(location);
                }
            }
        }
    }
}