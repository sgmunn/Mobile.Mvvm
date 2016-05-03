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
    using Mobile.Mvvm.DataBinding;

    public abstract class DataTemplateBase<TView, TViewModel> : IDataTemplate
        where TView : class where TViewModel : class
    {
        private readonly Dictionary<string, object> attributes;

        private Func<TViewModel, bool> filter;
        private Func<object, object[], TView> viewFactory; 
        private Action<TView> initializer;
        private Action<IBindingContext, TViewModel, TView> binder;

        protected DataTemplateBase() : this(null, null)
        {
        }
        
        protected DataTemplateBase(object id, string bindingExpression = null)
        {
            this.attributes = new Dictionary<string, object>();

            // TODO: ideally we would tokenise this now so that we don't have to do it when we're binding
            this.BindingExpression = bindingExpression;

            this.ViewType = typeof(TView);
            this.ViewModelType = typeof(TViewModel);
            this.Id = id;
        }

        public object Id { get; private set; }

        public Type ViewType { get; private set; }
        
        public Type ViewModelType { get; private set; }

        public string BindingExpression { get; set; }

        public object this[string attribute]
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

        public TemplateMatch CanApplyToViewModel(object viewModel)
        {
            if (viewModel == null)
            {
                return TemplateMatch.None;
            }

            if (this.ViewModelType == viewModel.GetType())
            {
                return TemplateMatch.Exact;
            }

            if (this.ViewModelType.IsAssignableFrom(viewModel.GetType()))
            {
                if (this.filter != null && this.filter((TViewModel)viewModel))
                {
                    return TemplateMatch.Exact;
                }

                return TemplateMatch.Assignable;
            }

            return TemplateMatch.None;    
        }

        public object CreateView()
        {
            return this.CreateView(new object[0]);
        }

        public virtual object CreateView(params object[] args)
        {
            if (this.viewFactory == null)
            {
                throw new InvalidOperationException("No View factory function specified");
            }

            return this.viewFactory(this.Id, args);
        }

        public virtual void InitializeView(object view)
        {
            if (this.initializer != null)
            {
                var v = view as TView;
                if (v != null)
                {
                    this.initializer(v);
                }
            }
        }

        public virtual void BindViewModel(IBindingContext context, object viewModel, object view)
        {
            // prefer binding functions over expressions
            if (this.binder != null)
            {
                var vm = viewModel as TViewModel;
                var v = view as TView;

                if (v != null && vm != null)
                {
                    this.binder(context, vm, v);
                    return;
                }
            }

            if (!string.IsNullOrEmpty(this.BindingExpression))
            {
                var exp = BindingParser.Default.Parse(this.BindingExpression, view, viewModel);
                context.Bindings.Add(exp);
            }
        }

        
        public DataTemplateBase<TView, TViewModel> Where(Func<TViewModel, bool> filter)
        {
            this.filter = filter;
            return this;
        }
        
        protected DataTemplateBase<TView, TViewModel> BaseOnCreate(Func<object, TView> viewFactory)
        {
            if (viewFactory == null)
            {
                throw new ArgumentNullException("viewFactory");
            }

            if (this.viewFactory != null)
            {
                throw new InvalidOperationException("View Factory function already specified");
            }

            this.viewFactory = (x, y) => viewFactory(x);
            return this;
        }

        protected DataTemplateBase<TView, TViewModel> BaseOnCreate(Func<object, object[], TView> viewFactory)
        {
            if (viewFactory == null)
            {
                throw new ArgumentNullException("viewFactory");
            }

            if (this.viewFactory != null)
            {
                throw new InvalidOperationException("View Factory function already specified");
            }

            this.viewFactory = viewFactory;
            return this;
        }
        
        public DataTemplateBase<TView, TViewModel> OnInit(Action<TView> initializer)
        {
            if (initializer == null)
            {
                throw new ArgumentNullException("initializer");
            }

            if (this.initializer != null)
            {
                throw new InvalidOperationException("Initializer function already specified");
            }

            this.initializer = initializer;

            return this;
        }
        
        public DataTemplateBase<TView, TViewModel> Bind(Action<IBindingContext, TViewModel, TView> binder)
        {
            if (binder == null)
            {
                throw new ArgumentNullException("binder");
            }

            if (this.binder != null)
            {
                throw new InvalidOperationException("Binder function already specified");
            }

            this.binder = binder;
            return this;
        }

        public DataTemplateBase<TView, TViewModel> SetAttribute(string key, object attribute)
        {
            this.attributes.Add(key, attribute);
            return this;
        }

    }
}
