// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModelContext.cs" company="sgmunn">
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
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using Mobile.Mvvm.Disposables;
    using Mobile.Mvvm.Diagnostics;
    using Mobile.Mvvm.DataBinding;

    /// <summary>
    /// Defines a context for a view model, bindings and injected properties.
    /// </summary>
    public abstract class ViewModelContext : IViewModelContext
    {
        private bool disposed;

        protected ViewModelContext(IViewModel viewModel)
        {
            this.ViewModel = viewModel;
            this.Bindings = new BindingScope();
            this.InjectedProperties = new InjectionScope(this.CreateInjectedPropertyStore);
        }
        
        ~ViewModelContext()
        {
            this.Dispose(false);
        }

        public IViewModel ViewModel { get; private set; }

        public IBindingScope Bindings { get; private set; }

        public IInjectionScope InjectedProperties { get; private set; }
        
        public void Dispose()
        {
            if (!this.disposed)
            {
                this.disposed = true;
                this.Dispose(true);
                GC.SuppressFinalize(this);
            }
        }

        protected virtual IInjectedPropertyStore CreateInjectedPropertyStore(object owner)
        {
            return new InjectedPropertyStore(owner);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Bindings.Dispose();
                this.InjectedProperties.Dispose();
            }
        }
    }
}
