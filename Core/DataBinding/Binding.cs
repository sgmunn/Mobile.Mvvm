// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Binding.cs" company="sgmunn">
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

namespace Mobile.Mvvm.DataBinding
{
    using System;
    using System.Reflection;
    using System.Globalization;

    /// <summary>
    /// Defines a binding to a (source) property.
    /// </summary>
    public sealed class Binding : IDisposable
    {
        public Binding(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentNullException("propertyName");
            }

            this.Mode = BindingMode.TwoWay;
            this.PropertyName = propertyName;
            this.PropertyAccessor = new ReflectionPropertyAccessor(propertyName);
        }

        public Binding(string propertyName, IPropertyAccessor accessor)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentNullException("propertyName");
            }
            
            if (accessor == null)
            {
                throw new ArgumentNullException("accessor");
            }
            
            this.Mode = BindingMode.TwoWay;
            this.PropertyName = propertyName;
            this.PropertyAccessor = accessor;
        }

        public string PropertyName
        {
            get;
            private set;
        }
        
        public BindingMode Mode
        {
            get;
            set;
        }
        
        public IValueConverter Converter 
        { 
            get; 
            set; 
        }
        
        public object ConverterParameter
        {
            get;
            set;
        }

        public IPropertyAccessor PropertyAccessor { get; set; }

        public object GetSourceValue(object sourceObject, Type targetType)
        {
            if (targetType == null)
            {
                throw new ArgumentNullException("targetType");
            }

            if (sourceObject == null)
            {
                return null;
            }

            var sourceValue = this.PropertyAccessor.GetValue(sourceObject);

            if (this.Converter != null)
            {
                sourceValue = this.Converter.Convert(sourceValue, targetType, this.ConverterParameter, CultureInfo.CurrentUICulture);
            }

            return sourceValue;
        }
        
        public void UpdateSourceValue(object sourceObject, object newValue)
        {
            if (sourceObject == null)
            {
                return;
            }

            if (this.Converter != null)
            {
                var propInfo = sourceObject.GetPropertyInfo(this.PropertyName);
                newValue = this.Converter.ConvertBack(newValue, propInfo.GetType(), this.ConverterParameter, CultureInfo.CurrentUICulture);
            }

            this.PropertyAccessor.SetValue(sourceObject, newValue);
        }
        
        public void Dispose()
        {
        }
    }
}

