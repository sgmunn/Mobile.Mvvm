using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Mobile.Mvvm;
using Mobile.Mvvm.ViewModel;
using SampleViewModels;
using System.Threading.Tasks;
using System.Threading;
using Mobile.Utils.Tasks;

namespace Sample.Touch.SampleControllers
{
    public partial class PlainViewController : UIViewController
    {
        private SimpleViewModel viewModel;

        private IRootViewModelContext viewModelContext;

        private ViewModelLoader<string> loader;

        public PlainViewController() : base ("PlainViewController", null)
        {
        }

        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();
            
            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            this.viewModel = new SimpleViewModel();

            // we can either put this here or in will appear, depending on what we need to do with loading for the VM.
            this.loader = new ViewModelLoader<string>(this.GetHelloWorld, this.UpdateViewModel, new UIThreadScheduler());

            // 
            this.NavigationItem.RightBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Play, (s, e) =>
            {
                    this.NavigationController.PushViewController(new SimpleListController(), true);
            });
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            this.BindViewModel();
            this.loader.Load();
        }

        public override void ViewDidDisappear(bool animated)
        {
            this.loader.Cancel();
            this.viewModelContext.Dispose();

            base.ViewDidDisappear(animated);
        }

        private void BindViewModel()
        {
            this.viewModelContext = new RootViewModelContext(this, this.viewModel);

            this.viewModelContext.Bind(label1, "Text", this.viewModel, "Property1");
            this.viewModelContext.Bind(field1, "Text", "Ended", this.viewModel, "Property1");

            //this.viewModelContext.Bind(this.button1, "Click", "Enabled", this.viewModel.TestCommand);
            //this.viewModelContext.Bind(this.button2, "Click", "Enabled", this.viewModel.TestCommand2);
        }

        private void UpdateViewModel(string x)
        {
            //this.RunOnUiThread(() => {
            Console.WriteLine("update UI thread {0}", System.Threading.Thread.CurrentThread.ManagedThreadId);
            ((SimpleViewModel)this.viewModelContext.ViewModel).Property1 = x;
            //});
        }

        private async Task<string> GetHelloWorld(CancellationToken cancel)
        {
            Console.WriteLine("get data thread {0}", System.Threading.Thread.CurrentThread.ManagedThreadId);
            await Task.Delay(3000).ConfigureAwait(false);

            if (cancel.IsCancellationRequested)
            {
                Console.WriteLine("cancelled");
                return "xx";
            }

            Console.WriteLine(">> get data thread {0}", System.Threading.Thread.CurrentThread.ManagedThreadId);
            return "hello world";
        }

    }
}
