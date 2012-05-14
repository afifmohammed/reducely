using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reducely
{
    internal static class Extensions
    {
        /// <summary>
        /// enumerates each item on the <paramref name="items"/> collection and will apply the <paramref name="action"/> on it.
        /// </summary>
        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (var item in items)
            {
                action(item);
            }
        }

        public static string ExecutingAssmeblyPath(this AppDomain appDomain)
        {
            return string.IsNullOrEmpty(appDomain.RelativeSearchPath)
                       ? appDomain.BaseDirectory
                       : appDomain.RelativeSearchPath;
        }

        public static Type ResolveClosingType(this Type type, params Type[] genericParams)
        {
            return type.IsGenericType
                       ? type.MakeGenericType(genericParams)
                       : type;
        }

        public static Type ResolveClosingInterface(this Type genericType, Type genericInterface)
        {
            if (genericType.IsInterface || genericType.IsAbstract)
                return null;

            var typeOfObject = typeof(object);
            do
            {
                var interfaces = genericType.GetInterfaces();

                foreach (var @interface in interfaces
                                .Where(@interface => @interface.IsGenericType)
                                .Where(@interface => @interface.GetGenericTypeDefinition() == genericInterface))
                    return @interface;

                genericType = genericType.BaseType;

            } while (genericType != typeOfObject);

            return null;
        }
    }
}
