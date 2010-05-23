using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Metsys.Little
{    
    internal static class ListHelper
    {
        public static Type GetListItemType(Type enumerableType)
        {
            if (enumerableType.IsArray)
            {
                return enumerableType.GetElementType();
            }
            return enumerableType.IsGenericType ? enumerableType.GetGenericArguments()[0] : typeof(object);
        }

        public static Type GetDictionarKeyType(Type enumerableType)
        {
            return enumerableType.IsGenericType
                ? enumerableType.GetGenericArguments()[0]
                : typeof(object);
        }

        public static Type GetDictionarValueType(Type enumerableType)
        {
            return enumerableType.IsGenericType
                ? enumerableType.GetGenericArguments()[1]
                : typeof(object);
        }
      
        public static IDictionary CreateDictionary(Type dictionaryType, Type keyType, Type valueType)
        {
            if (dictionaryType.IsInterface)
            {
                return (IDictionary)Activator.CreateInstance(typeof(Dictionary<,>).MakeGenericType(keyType, valueType));
            }

            if (dictionaryType.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, new Type[0], null) != null)
            {
                return (IDictionary)Activator.CreateInstance(dictionaryType);
            }

            return new Dictionary<object, object>();
        }
    }
}