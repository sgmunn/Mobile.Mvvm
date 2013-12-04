// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BindingContextExtensions.cs" company="sgmunn">
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

namespace Mobile.Mvvm
{
    using System;
    using Mobile.Mvvm.DataBinding;
    using Mobile.Mvvm.ViewModel;

    public static class BindingContextExtensions
    {
        public static void Bind(this IBindingContext bindingContext, object target, object viewModel, string expression)
        {
            bindingContext.Bindings.AddBindings(target, viewModel, expression);
        }

        public static void Bind(this IBindingContext bindingContext, object target, string targetProperty, object viewModel, string viewModelProperty)
        {
            bindingContext.Bindings.AddBinding(target, targetProperty, viewModel, viewModelProperty);
        }

        public static void Bind(this IBindingContext bindingContext, object target, string targetProperty, string targetEventName, object viewModel, string viewModelProperty)
        {
            bindingContext.Bindings.AddEventTriggeredBinding(target, targetProperty, targetEventName, viewModel, viewModelProperty);
        }

        public static void Bind(this IBindingContext bindingContext, object target, string commandEvent, string enabledProperty, ICommand command)
        {
            var binding = new WeakCommandBinding(target, commandEvent, enabledProperty, command);
            bindingContext.Bindings.Add(binding);
        }
    }
    
}
