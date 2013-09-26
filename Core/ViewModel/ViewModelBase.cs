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
    using System.Runtime.CompilerServices;
    using Mobile.Mvvm.Disposables;
    using Mobile.Mvvm.Diagnostics;

    public abstract class ViewModelBase : IViewModel, ILoadable, IPersistentState, ILifetime//, ICommand
    {
        private readonly CompositeDisposable lifetimeScope;

        private bool disposed;
        
        protected ViewModelBase()
        {
            this.lifetimeScope = new CompositeDisposable();
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

//        public DelegateCommand Command
//        {
//            get;
//            set;
//        }
        
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

        public virtual void Load(IStateBundle parameters)
        {
        }

        public virtual void Load()
        {
            throw new NotImplementedException();
        }

        public virtual void RestoreState(IStateBundle state)
        {
        }

        public virtual void SaveState(IStateBundle state)
        {
        }

//        public virtual void Execute()
//        {
//            if (this.Command != null)
//            {
//                this.Command.Execute();
//            }
//        }
//        
//        public virtual bool GetCanExecute()
//        {
//            if (this.Command != null)
//            {
//                return this.Command.GetCanExecute();
//            }
//
//            return true;
//        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.lifetimeScope.Dispose();
            }
        }

        // TODO: CallerMemberName
        protected void NotifyPropertyChanged(string propertyName)
        {
            var changed = this.PropertyChanged;
            if (changed != null)
            {
                changed(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
