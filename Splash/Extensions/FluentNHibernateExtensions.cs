using FluentNHibernate;
using FluentNHibernate.Cfg;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Helpers;
using FluentNHibernate.Conventions.Instances;
using FluentNHibernate.Mapping;
using FluentNHibernate.Mapping.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Splash.Extensions
{
    public static class FluentNHibernateExtensions
    {
        public static FluentMappingsContainer AddFromNamespaceOf<T>(
            this FluentMappingsContainer fmc)
        {
            string ns = typeof(T).Namespace;
            IEnumerable<Type> types = typeof(T).Assembly.GetExportedTypes()
                .Where(t => t.Namespace == ns)
                .Where(x => IsMappingOf<IMappingProvider>(x) ||
                            IsMappingOf<IIndeterminateSubclassMappingProvider>(x) ||
                            IsMappingOf<IExternalComponentMappingProvider>(x) ||
                            IsMappingOf<IFilterDefinition>(x));

            foreach (Type t in types)
            {
                fmc.Add(t);
            }

            return fmc;
        }

        /// <summary>
        /// Private helper method cribbed from FNH source (PersistenModel.cs:151)
        /// </summary>
        private static bool IsMappingOf<T>(Type type)
        {
            return !type.IsGenericType && typeof(T).IsAssignableFrom(type);
        }

        public static IConvention[] ForLowercaseSystem(string referenceSuffix, bool toLowercase)
        {
            IList<IConvention> lcase =
                new IConvention[]
                {
                    Table.Is(x => x.EntityType.Name.ToLowercaseNamingConvention(toLowercase))
                    ,ConventionBuilder.Property.Always(x => x.Column(x.Name.ToLowercaseNamingConvention(toLowercase)))
                    ,ConventionBuilder.Id.Always( x => x.Column(x.Name.ToLowercaseNamingConvention(toLowercase)) )        
                    , LowercaseForeignKey.EndsWith(referenceSuffix, toLowercase)

                };

            return lcase.ToArray();
        }

        public static string ToLowercaseNamingConvention(this string s)
        {
            return s.ToLowercaseNamingConvention(true);
        }

        public static string ToLowercaseNamingConvention(this string s, bool toLowercase)
        {
            if (toLowercase)
            {
                var r = new Regex(@"
                (?<=[A-Z])(?=[A-Z][a-z]) |
                 (?<=[^A-Z])(?=[A-Z]) |
                 (?<=[A-Za-z])(?=[^A-Za-z])", RegexOptions.IgnorePatternWhitespace);

                StringBuilder newString = new StringBuilder(r.Replace(s, "_").ToLower());
                for (int i = 0; i < newString.Length; i++)
                {
                    if (newString[i] == '_')
                    {
                        newString[i + 1] = newString[i + 1].ToString().ToUpper().ToCharArray()[0];
                        newString[i] = '@';
                    }
                }
                newString.Replace("@", String.Empty);
                
                return newString.ToString();
            }
            else
                return s;
        }

        public static class LowercaseForeignKey
        {
            public static ForeignKeyConvention EndsWith(string suffix)
            {
                return EndsWith(suffix, true);
            }

            public static ForeignKeyConvention EndsWith(string suffix, bool toLowercase)
            {
                return new BuiltSuffixLowercaseForeignKeyConvention(suffix, toLowercase);
            }
        }

        public class BuiltSuffixLowercaseForeignKeyConvention : ForeignKeyConvention
        {
            private readonly string suffix;

            private readonly bool toLowercase;

            public BuiltSuffixLowercaseForeignKeyConvention(string suffix, bool toLowercase)
            {
                this.suffix = suffix;
                this.toLowercase = toLowercase;
            }

            public BuiltSuffixLowercaseForeignKeyConvention(string suffix) : this(suffix, true) { }

            protected override string GetKeyName(Member property, Type type)
            {
                return (property != null ?
                    property.Name.ToLowercaseNamingConvention(this.toLowercase)
                    :
                    type.Name.ToLowercaseNamingConvention(this.toLowercase)) + suffix;
            }
        }
    }
}