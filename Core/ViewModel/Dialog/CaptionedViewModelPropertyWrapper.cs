// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CaptionedViewModelPropertyWrapper.cs" company="sgmunn">
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

namespace Mobile.Mvvm.ViewModel.Dialog
{
    using System;
	using Mobile.Mvvm.Reflection;

    public class StringWrapperElementViewModel : CaptionedViewModelPropertyWrapper<string>
    {
        public StringWrapperElementViewModel(IViewModel wrappedViewModel, string propertyName) 
            : base(wrappedViewModel, propertyName)
        {
        }

        public StringWrapperElementViewModel(IViewModel wrappedViewModel, string propertyName, IPropertyAccessor accessor) 
            : base(wrappedViewModel, propertyName, accessor)
        {
        }
    }

    public class BooleanWrapperElementViewModel : CaptionedViewModelPropertyWrapper<bool>
    {
        public BooleanWrapperElementViewModel(IViewModel wrappedViewModel, string propertyName) 
            : base(wrappedViewModel, propertyName)
        {
        }

        public BooleanWrapperElementViewModel(IViewModel wrappedViewModel, string propertyName, IPropertyAccessor accessor) 
            : base(wrappedViewModel, propertyName, accessor)
        {
        }
    }

    public class CaptionedViewModelPropertyWrapper<T> : ViewModelPropertyWrapper<T>, ICaptionedElementValue<T>, ITapCommand
    {
        public CaptionedViewModelPropertyWrapper(IViewModel wrappedViewModel, string propertyName)
            : base(wrappedViewModel, propertyName, new ReflectionPropertyAccessor(propertyName))
        {
            this.Caption = propertyName;
        }

        public CaptionedViewModelPropertyWrapper(IViewModel wrappedViewModel, string propertyName, IPropertyAccessor accessor)
            : base(wrappedViewModel, propertyName, accessor)
        {
            this.Caption = propertyName;
        }

        public string Caption
        {
            get
            {
                return this.GetPropertyValue<string>("Caption");
            }

            set
            {
                this.SetPropertyValue("Caption", value);
            }
        }

        // TODO: INPC if command is bound
        public ICommand TapCommand { get; set; }
    }
}
