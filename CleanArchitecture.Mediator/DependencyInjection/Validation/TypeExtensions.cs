using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace CleanArchitecture.Mediator.DependencyInjection.Validation
{

    internal static class TypeExtensions
    {

        #region - - - - - - Methods - - - - - -

        public static string GetFriendlyName(this Type type)
            => type.IsGenericType
                ? $"{Regex.Replace(type.Name, "`[0-9]+$", string.Empty)}<{GetGenericArguments(type)}>"
                : type.Name;

        private static string GetGenericArguments(Type type)
            => type.GetGenericArguments().Select(t => t.GetFriendlyName()).Aggregate((agg, inc) => $"{agg}, {inc}");

        public static Type GetTypeDefinition(this Type type)
            => type.IsGenericType ? type.GetGenericTypeDefinition() : type;

        #endregion Methods

    }

}
