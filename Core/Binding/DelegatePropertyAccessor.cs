//  --------------------------------------------------------------------------------------------------------------------
//  <copyright file="BindingAssistant.cs" company="sgmunn">
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

namespace Mobile.Mvvm.Binding
{
    using System;

    /// <summary>
    /// Provides a way to access a property given some delegates 
    /// </summary>
    /// <remarks>
    /// The primary purpose for this class is provide a means by which bindings can perform faster than using
    /// propertyInfo.GetValue / .SetValue
    /// A secondary reason allows arbitrarily complex bindings to compex properties - currently binding only support
    /// a simple property
    /// </remarks>
    public sealed class DelegatePropertyAccessor : IPropertyAccessor
    {
        public DelegatePropertyAccessor(string propertyPath, Func<object, object> getter, Action<object, object> setter)
        {
            if (string.IsNullOrEmpty(propertyPath))
            {
                throw new ArgumentNullException("propertyPath");
            }
            
            this.PropertyPath = propertyPath;
            this.Getter = getter;
            this.Setter = setter;
        }

        public string PropertyPath { get; private set; }

        public Func<object, object> Getter { get; private set; }
        
        public Action<object, object> Setter { get; private set; }

        public bool CanGetValue(object obj) 
        { 
            return this.Getter != null;
        }

        public bool CanSetValue(object obj) 
        { 
            return this.Setter != null;
        }

        public object GetValue(object obj)
        {
            if (this.CanGetValue(obj))
            {
                return this.Getter(obj);
            }

            return null;
        }

        public void SetValue(object obj, object value)
        {
            if (this.CanSetValue(obj))
            {
                this.Setter(obj, value);
            }
        }
    }
}
