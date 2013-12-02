//  --------------------------------------------------------------------------------------------------------------------
//  <copyright file="ViewModelBase.cs" company="sgmunn">
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

namespace Mobile.Mvvm.ViewModel
{
    using System;
    using System.ComponentModel;
    using Mobile.Utils;
    using Mobile.Utils.Disposables;
    using Mobile.Utils.Diagnostics;

    public abstract class ViewModelBase : IViewModel, INotifyPropertyChanged, IPersistentState, ILifetime
    {
        private readonly CompositeDisposable lifetimeScope;
        
        private readonly PropertyBag properties;

        private bool disposed;
        
        protected ViewModelBase()
        {
            this.lifetimeScope = new CompositeDisposable();
            this.properties = new PropertyBag(this.NotifyPropertyChanged);
        }

        ~ViewModelBase()
        {
            Log.Write("~ViewModelBase - {0}", this.GetType().ToString());
            this.Dispose(false);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public CompositeDisposable Lifetime
        {
            get
            {
                return this.lifetimeScope;
            }
        }

        public void Dispose()
        {
            Log.Write("Dispose.ViewModelBase");
            if (!this.disposed)
            {
                this.disposed = true;
                this.Dispose(true);
                GC.SuppressFinalize(this);
            }
        }

        public virtual void RestoreState(IStateBundle state)
        {
        }

        public virtual IStateBundle SaveState()
        {
            return new StateBundle();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.lifetimeScope.Dispose();
            }
        }

        protected void SetPropertyValue(string propertyName, object value)
        {
            this.properties.SetProperty(propertyName, value);
        }
        
        protected object GetPropertyValue(string propertyName, object defaultValue = null)
        {
            return this.properties.GetProperty(propertyName, defaultValue);
        }

        protected virtual void NotifyPropertyChanged(string propertyName)
        {
            var changed = this.PropertyChanged;
            if (changed != null)
            {
                changed(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public abstract class ViewModelBase<TData> : ViewModelBase, ILoadable<TData>
    {
        public bool IsLoaded { get; private set; }

        public virtual void Load(TData data)
        {
            this.IsLoaded = true;
        }
    }
}
