// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModelLoader.cs" company="sgmunn">
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
    using System.Threading;
    using System.Threading.Tasks;
    using Mobile.Utils;

    public class ViewModelLoader<TData>
    {
        private readonly CancellationTokenSource cancellation;

        private readonly Func<CancellationToken, Task<TData>> loader;

        private readonly Action<TData> updater;

        private readonly TaskScheduler scheduler;

        private Task loadTask;

        public ViewModelLoader(Func<CancellationToken, Task<TData>> loader, Action<TData> updater)
        {
            this.loader = loader;
            this.updater = updater;
            this.cancellation = new CancellationTokenSource();
        }

        public ViewModelLoader(Func<CancellationToken, Task<TData>> loader, Action<TData> updater, TaskScheduler scheduler)
        {
            this.loader = loader;
            this.updater = updater;
            this.cancellation = new CancellationTokenSource();
            this.scheduler = scheduler;
        }

        public virtual Task Load()
        {
            if (this.loadTask != null)
            {
                return this.loadTask;
            }

            if (this.scheduler != null)
            {
                this.loadTask = this.loader(this.cancellation.Token).ContinueWith(this.UpdateViewModel, this.cancellation.Token, TaskContinuationOptions.NotOnCanceled, this.scheduler);
            }
            else
            {
                this.loadTask = this.loader(this.cancellation.Token).ContinueWith(this.UpdateViewModel, this.cancellation.Token);
            }

            return this.loadTask;
        }

        public virtual void UpdateViewModel(Task<TData> task)
        {
            this.updater(task.Result);
        }

        public virtual void Cancel()
        {
            this.cancellation.Cancel();
        }
    }

    public class ViewModelLoader<TData, TViewModel> : ViewModelLoader<TData>
        where TViewModel : ILoadable<TData>
    {
        public ViewModelLoader(TViewModel viewModel, Func<CancellationToken, Task<TData>> loader, TaskScheduler scheduler) 
            : base(loader, viewModel.Load, scheduler)
        {
            viewModel.EnsureNotNull("viewModel");

            this.ViewModel = viewModel;
        }

        public TViewModel ViewModel { get; private set; }
    }
}

