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

    public class DataTemplate : IDataTemplate
    {
        private readonly Dictionary<string, object> attributes;
        private Type viewModelType;
        private Func<object, bool> selector;
        private Func<object, object> creator1; 
        private Func<object, object, object> creator2; 
        private Action<object> initializer;
        private Action<object, object> binder;
        private Func<object, float> height;
        
        public DataTemplate()
        {
            this.attributes = new Dictionary<string, object>();
        }

        public DataTemplate(object id)
        {
            this.Id = id;
            this.attributes = new Dictionary<string, object>();
        }

        public object Id
        {
            get;
            private set;
        }

        public Type ViewType
        {
            get;
            private set;
        }

        public object this [string attribute] 
        { 
            get
            {
                object result;
                if (this.attributes.TryGetValue(attribute, out result))
                {
                    return result;
                }

                return null;
            }
        }

        public DataTemplate WhenSelecting<TViewModel>(Func<TViewModel, bool> selector)
        {
            this.viewModelType = typeof(TViewModel);
            this.selector = (v) => selector((TViewModel)v);

            return this;
        }

        public DataTemplate Creates<TView>(Func<object, TView> creator)
        {
            if (creator == null)
            {
                throw new ArgumentNullException("creator");
            }

            this.ViewType = typeof(TView);
            this.creator1 = creator as Func<object, object>;

            return this;
        }
        
        public DataTemplate Creates<TView>(Func<object, object, TView> creator)
        {
            if (creator == null)
            {
                throw new ArgumentNullException("creator");
            }

            this.ViewType = typeof(TView);
            this.creator2 = creator as Func<object, object, object>;

            return this;
        }

        //        public DataTemplate Creates<TView>()
        //        {
        //            this.ViewType = typeof(TView);
        //
        //            return this;
        //        }

        public DataTemplate WhenInitializing<TView>(Action<TView> initializer)
        {
            if (initializer == null)
            {
                throw new ArgumentNullException("initializer");
            }

            this.initializer = (x) => initializer((TView)x);

            return this;
        }

        public DataTemplate WhenBinding<TViewModel, TView>(Action<TViewModel, TView> binder)
        {
            if (binder == null)
            {
                throw new ArgumentNullException("binder");
            }

            if (this.viewModelType == null)
            {
                this.viewModelType = typeof(TViewModel);
            }

            this.binder = (vm, view) => binder((TViewModel)vm, (TView)view);

            return this;
        }

        public DataTemplate HavingHeight<TViewModel>(Func<TViewModel, float> height)
        {
            this.height = (vm) => height((TViewModel)vm);
            return this;
        }

        public DataTemplate HavingHeight(float height)
        {
            this.height = (vm) => height;
            return this;
        }

        public DataTemplate HavingAttribute(string key, object attribute)
        {
            this.attributes.Add(key, attribute);
            return this;
        }

        public TemplateMatch CanApplyToViewModel(object viewModel)
        {
            if (viewModel == null || this.viewModelType == null)
            {
                return TemplateMatch.None;
            }

            if (this.viewModelType == viewModel.GetType())
            {
                return TemplateMatch.Exact;
            }

            if (this.viewModelType.IsAssignableFrom(viewModel.GetType()))
            {
                if (this.selector != null && this.selector(viewModel))
                {
                    return TemplateMatch.Exact;
                }

                return TemplateMatch.Assignable;
            }

            return TemplateMatch.None;    
        }

        public object CreateView()
        {
            if (this.creator1 == null && this.creator2 == null)
            {
                throw new InvalidOperationException("No Creator function specified");
            }

            return this.creator1(this.Id);
        }
        
        public object CreateView(object root)
        {
            if (this.creator2 == null)
            {
                throw new InvalidOperationException("No Creator function specified");
            }

            return this.creator2(this.Id, root);
        }

        public void InitializeView(object view)
        {
            if (this.initializer != null)
            {
                this.initializer(view);
            }
        }

        public void BindViewModel(object viewModel, object view)
        {
            if (this.binder != null)
            {
                this.binder(viewModel, view);
            }
        }

        public float CalculateHeight(object viewModel)
        {
            if (this.height != null)
            {
                return this.height(viewModel);
            }

            return -1;
        }
    }
}
