//  --------------------------------------------------------------------------------------------------------------------
//  <copyright file="ReflectionPropertyAccessor.cs" company="sgmunn">
//    (c) sgmunn 2012  
//
//    Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//    documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
//    the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
//    to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
//    The above copyright notice and this permission notice shall be included in all copies or substantial portions of 
//    the Software.
//
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO 
//    THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
//    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
//    CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS 
//    IN THE SOFTWARE.
//  </copyright>
//  --------------------------------------------------------------------------------------------------------------------

namespace Mobile.Utils.Reflection
{
    using System;
    using System.Reflection;
    using System.Linq;

    public sealed class ReflectionPropertyAccessor : IPropertyAccessor
    {
        public ReflectionPropertyAccessor(string propertyPath)
        {
            if (string.IsNullOrEmpty(propertyPath))
            {
                throw new ArgumentNullException("propertyPath");
            }
            
            this.PropertyPath = propertyPath;
        }
        
        public string PropertyPath { get; private set; }

        public Type GetPropertyType(object obj)
        {
            object targetObject;
            var propertyInfo = GetPropertyPathInfo(obj, this.PropertyPath, out targetObject);
            return propertyInfo.PropertyType;
        }

        public bool CanGetValue(object obj) 
        { 
            if (obj != null)
            {
                object targetObject;
                var propertyInfo = GetPropertyPathInfo(obj, this.PropertyPath, out targetObject);
                return (propertyInfo != null && propertyInfo.CanRead);
            }
            
            return false;
        }
        
        public bool CanSetValue(object obj) 
        { 
            if (obj != null)
            {
                object targetObject;
                var propertyInfo = GetPropertyPathInfo(obj, this.PropertyPath, out targetObject);
                return (propertyInfo != null && propertyInfo.CanWrite);
            }

            return false;
        }
        
        public object GetValue(object obj)
        {
            if (obj != null)
            {
                object targetObject;
                var propertyInfo = GetPropertyPathInfo(obj, this.PropertyPath, out targetObject);
                if (propertyInfo != null && propertyInfo.CanRead)
                {
                    return propertyInfo.GetValue(targetObject, null);
                }
            }

            return null;
        }
        
        public void SetValue(object obj, object value)
        {
            if (obj != null)
            {
                object targetObject;
                var propertyInfo = GetPropertyPathInfo(obj, this.PropertyPath, out targetObject);
                if (propertyInfo != null && propertyInfo.CanWrite)
                {
                    propertyInfo.SetValue(targetObject, value, null);
                }
            }
        }

        // TODO: indexed paths
        private static PropertyInfo GetPropertyPathInfo(object instance, string propertyPath, out object inspectedObject)
        {
            inspectedObject = instance;
            if (propertyPath.Contains("."))
            {
                var pathComponents = propertyPath.Split(new [] { "." }, StringSplitOptions.RemoveEmptyEntries);
                if (pathComponents.Length > 1)
                {
                    // recurse into each level, except the last one                    
                    foreach (var path in pathComponents.Take(pathComponents.Length - 1))
                    {
                        var info = GetProperty(inspectedObject, path, out inspectedObject);
                        if (info == null)
                        {
                            return null;
                        }

                        inspectedObject = info.GetValue(inspectedObject);
                    }

                    return GetProperty(inspectedObject, pathComponents.Last(), out inspectedObject);
                }
            }

            return GetProperty(inspectedObject, propertyPath, out inspectedObject);
        }

        private static PropertyInfo GetProperty(object instance, string path, out object inspectedObject)
        {
            inspectedObject = instance;

//            // name [ index ]
//            if (path.Contains("[") && path.Contains("]"))
//            {
//                var pathComponents = path.Split(new [] { "[", "]" }, StringSplitOptions.RemoveEmptyEntries);
//                if (pathComponents.Length == 2)
//                {
//                    // the first part is the property, the second is the index
//                    var info = inspectedObject.GetType().GetProperty(pathComponents[0]);
//                    inspectedObject = info.GetValue(inspectedObject);
//
//                    // now we need to get an indexed value
//                    int.TryParse(pathComponents[1], out index);
//
//                    return info;
//                }
//
//                return null;
//            }

            return inspectedObject.GetType().GetProperty(path);
        }

    }
}
