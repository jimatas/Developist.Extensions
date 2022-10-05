using System;
using System.Linq;

namespace Developist.Extensions.Logging.Log4Net.Utilities
{
    internal static class TypeExtensions
    {
        public static bool DerivesFromGenericParent(this Type type, Type genericTypeDefinition)
        {
            if (type == typeof(object))
            {
                return false;
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == genericTypeDefinition)
            {
                return true;
            }

            if (type.IsInterface)
            {
                return false;
            }

            return type.BaseType.DerivesFromGenericParent(genericTypeDefinition)
                || type.GetInterfaces().Any(iface => iface.DerivesFromGenericParent(genericTypeDefinition));
        }
    }
}
