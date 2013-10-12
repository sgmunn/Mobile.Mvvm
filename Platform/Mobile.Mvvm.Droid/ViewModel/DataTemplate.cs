// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataTemplate.cs" company="sgmunn">
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
    using System.Collections.Generic;
    using Android.Views;

    public class DataTemplate<TView, TViewModel> : DataTemplateBase<TView, TViewModel>
        where TView : View where TViewModel : class
    {
        private readonly List<Tuple<int, string>> childBindings;

        private LayoutInflater inflator;

        public DataTemplate(LayoutInflater inflator, int id, string bindingExpression = null) 
            : base(id, bindingExpression)
        {
            this.inflator = inflator;
            this.childBindings = new List<Tuple<int, string>>();
        }
        
        public DataTemplate(int id, string bindingExpression = null) 
            : base(id, bindingExpression)
        {
            this.childBindings = new List<Tuple<int, string>>();
        }


        public DataTemplate<TView, TViewModel> OnCreate(Func<int, ViewGroup, TView> viewFactory)
        {
            base.BaseSelect((x, y) => viewFactory((int)x, (ViewGroup)y[0]));
            return this;
        }
        
        public DataTemplate<TView, TViewModel> BindChildView(int id, string bindingExpression)
        {
            this.childBindings.Add(new Tuple<int, string>(id, bindingExpression));
            return this;
        }

        public override object CreateView(params object[] args)
        {
            // should we check for a view factory?

            if (this.inflator != null)
            {
                // TODO: we need to control this better
                return this.inflator.Inflate((int)this.Id, (ViewGroup)args[0], false);
            }

            return base.CreateView(args);
        }

        
        public override void BindViewModel(IBindingContext context, object viewModel, object view)
        {
            base.BindViewModel(context, viewModel, view);

            var v = view as TView;

            if (v != null)
            {
                foreach (var childBinding in this.childBindings)
                {
                    context.Bindings.AddBindings(v.FindViewById(childBinding.Item1), viewModel, childBinding.Item2);
                }
            }
        }
    }
}

