using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceStack.FluentValidation;
using ServiceStack.FluentValidation.Internal;

namespace Splash.Extensions
{
    public enum TypeOfName
    {
        First,
        Last,
        Nickname
    }

    public enum GeocoordinateType
    {
        Latitude,
        Longitude,
    }

    public static class ValidationExtensions
    {
        public static IRuleBuilderOptions<T, Int32> EnsureValidId<T>(this IRuleBuilder<T, Int32> rule)
        {
            return rule.NotNull().GreaterThan(0).WithMessage("You must enter a user UserId, and one that is greater than 0.");
        }

        public static IRuleBuilderOptions<T, Int32?> EnsureValidOptionalId<T>(this IRuleBuilder<T, Int32?> rule)
        {
            return rule.GreaterThan(0).WithMessage("This user UserId is optional, but if supplied, it must be greater than 0.");
        }

        public static IRuleBuilderOptions<T, String> EnsureValidName<T>(this IRuleBuilder<T, String> rule, TypeOfName type)
        {
            switch (type)
            {
                case TypeOfName.First:
                    return rule.Length(1, 35).Must(x => x.ToList().TrueForAll(Char.IsLetter));
                case TypeOfName.Last:
                    return rule.Length(1, 35).Must(x => x.ToList().TrueForAll(Char.IsLetter));
                case TypeOfName.Nickname:
                    return rule.Length(1, 25).Must(x => x.ToList().TrueForAll(Char.IsLetterOrDigit));
                default:
                    throw new ArgumentOutOfRangeException("type");
            }
        }

        public static IRuleBuilderOptions<T, String> EnsureValidEmail<T>(this IRuleBuilder<T, String> rule)
        {
            return rule.NotEmpty().EmailAddress().WithMessage("{PropertyName} is not a valid e-mail address.");
        }

        public static IRuleBuilderOptions<T, String> EnsureValidPhoneNumber<T>(this IRuleBuilder<T, String> rule)
        {
            return rule.NotEmpty().Length(1, 12).Must(x => x.ToList().TrueForAll(Char.IsDigit));
        }

        public static IRuleBuilderOptions<T, String> EnsureValidPassword<T>(this IRuleBuilder<T, String> rule)
        {
            return rule.NotEmpty().Length(6, 35).WithMessage("As a good practice, your password must be at least 6 characters.");
        }

        public static IRuleBuilderOptions<T, Int64?> EnsureValidTimestamp<T>(this IRuleBuilder<T, Int64?> rule, bool isMaxTimestamp)
        {
            return (isMaxTimestamp ? rule.GreaterThanOrEqualTo(0).LessThanOrEqualTo(DateTime.Now.ToTimestamp()).WithMessage("Your timestamp must be greater than 0 and less than the current timestamp of " + DateTime.Now.ToTimestamp()) : rule.GreaterThanOrEqualTo(0).WithMessage("Your timestamp cannot be negative"));
        }

        public static IRuleBuilderOptions<T, Int32?> EnsureValidLimitByNumber<T>(this IRuleBuilder<T, Int32?> rule)
        {
            return rule.GreaterThan(0).WithMessage("The limiting number of items to display must be greater than 0.");
        }

        public static IRuleBuilderOptions<T, Decimal> EnsureValidGeocoordinate<T>(this IRuleBuilder<T, Decimal> rule, GeocoordinateType type)
        {
            switch (type)
            {
                case GeocoordinateType.Latitude:
                    return rule.GreaterThanOrEqualTo(-90).LessThanOrEqualTo(90).WithMessage("Latitude values range from -90 to 90. You entered {PropertyName}.");
                case GeocoordinateType.Longitude:
                    return rule.GreaterThanOrEqualTo(-180).LessThanOrEqualTo(180).WithMessage("Longitude values range from -180 to 180. You entered {PropertyName}.");
                default:
                    throw new ArgumentOutOfRangeException("type");
            }
        }
    }
}