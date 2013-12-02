// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReflectionExtensions.cs" company="sgmunn">
//   (c) sgmunn 2012  
//
//   Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//   documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
//   the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
//   to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
//   The above copyright notice and this permission notice shall be included in all copies or substantial portions of 
//   the Software.
// 
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO 
//   THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
//   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
//   CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS 
//   IN THE SOFTWARE.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
using Mobile.Utils.Diagnostics;

namespace Mobile.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Extension methods for reflection
    /// </summary>
    public static class ReflectionExtensions
    {
//        /// <summary>
//        /// Gets all Types that have the given attribute defined
//        /// </summary>        
//        public static IQueryable<Type> GetTypesWith<TAttribute>(Assembly[] assemblies, bool inherit) where TAttribute: System.Attribute
//        {
//            // assemblies = AppDomain.CurrentDomain.GetAssemblies()
//            return from a in assemblies.AsQueryable()
//                from t in a.GetTypes()
//                    where t.IsDefined(typeof(TAttribute),inherit)
//                    select t;
//        }
//        
//        /// <summary>
//        /// Gets the attributes for a given Type as a Queryable
//        /// </summary>
//        public static IQueryable<Attribute> GetAttributes(Type objectType)
//        {
//            return System.Attribute.GetCustomAttributes(objectType).AsQueryable();
//        }
//
//        public static IEnumerable<T> GetAttributes<T>(this MemberInfo member, bool inherit)
//        {
//            return Attribute.GetCustomAttributes(member, inherit).OfType<T>();
//        }
//        

        public static PropertyInfo GetPropertyInfo(this object instance, string propertyName)
        {
            return instance.GetType().GetProperty(propertyName);
        }

        /// <summary>
        /// Provides a GetMethod compatible version that recurses up to base types 
        /// </summary>
        public static MethodInfo GetMethod(this Type type, string name)
        {
            var typeInfo = type.GetTypeInfo();
            var info = typeInfo.GetDeclaredMethod(name);

            if (info == null)
            {
                if (typeInfo.BaseType == null)
                {
                    return null;
                }

                return GetMethod(typeInfo.BaseType, name);
            }

            return info;
        }

        /// <summary>
        /// Provides a GetProperty compatible version that recurses up to base types 
        /// </summary>
        public static PropertyInfo GetProperty(this Type type, string name)
        {
            var typeInfo = type.GetTypeInfo();
            var info = typeInfo.GetDeclaredProperty(name);

            if (info == null)
            {
                if (typeInfo.BaseType == null)
                {
                    return null;
                }

                return GetProperty(typeInfo.BaseType, name);
            }

            return info;
        }

        // TODO: test isStatic
        public static EventInfo GetEvent(this Type type, string name, bool isStatic)
        {
            var typeInfo = type.GetTypeInfo();
            var info = typeInfo.GetDeclaredEvent(name);

            if (info == null)
            {
                type = typeInfo.BaseType;
                if (type == null)
                {
                    return null;
                }

                return GetEvent(type, name, isStatic);
            }

            return info;
        }

        public static bool IsAssignableFrom(this Type type, Type otherType)
        {
            var typeInfo = type.GetTypeInfo();
            return typeInfo.IsAssignableFrom(otherType.GetTypeInfo());
        }
    }
}