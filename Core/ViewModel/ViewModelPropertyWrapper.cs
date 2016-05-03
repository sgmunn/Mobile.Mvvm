// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModelPropertyWrapper.cs" company="sgmunn">
//   (c) sgmunn 2013  
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

namespace Mobile.Mvvm.ViewModel
{
    using System;
    using Mobile.Mvvm.DataBinding;
	using Mobile.Mvvm.Reflection;

    public class ViewModelPropertyWrapper<T> : ViewModelBase
    {
        private readonly string propertyName;
        private readonly IViewModel viewModel;
        private readonly IPropertyAccessor accessor;

        public ViewModelPropertyWrapper(IViewModel wrappedViewModel, string propertyName)
            : this(wrappedViewModel, propertyName, new ReflectionPropertyAccessor(propertyName))
        {
        }

        public ViewModelPropertyWrapper(IViewModel wrappedViewModel, string propertyName, IPropertyAccessor accessor)
        {
            wrappedViewModel.EnsureNotNull("wrappedViewModel");
            propertyName.EnsureNotNull("propertyName");
            accessor.EnsureNotNull("accessor");

            this.viewModel = wrappedViewModel;
            this.propertyName = propertyName;
            this.accessor = accessor;
        }

        public IViewModel ViewModel
        {
            get
            {
                return this.viewModel;
            }
        }

        public string PropertyName
        {
            get
            {
                return this.propertyName;
            }
        }

        public T Value
        {
            get
            {
                if (this.accessor.CanGetValue(this.viewModel))
                {
                    return (T)this.accessor.GetValue(this.viewModel);
                }

                return default(T);
            }

            set
            {
                if (this.accessor.CanSetValue(this.viewModel))
                {
                    this.accessor.SetValue(this.viewModel, value);
                    this.SetPropertyValue("Value", value);
                }
            }
        }

        public override string ToString()
        {
            return string.Format("{0}", this.Value);
        }
    }
}
