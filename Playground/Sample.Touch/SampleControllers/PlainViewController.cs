using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Mobile.Mvvm;
using Mobile.Mvvm.ViewModel;
using SampleViewModels;

namespace Sample.Touch.SampleControllers
{
    public partial class PlainViewController : UIViewController
    {
        private IRootViewModelContext viewModelContext;

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

            var vm = new SimpleViewModel();
            this.viewModelContext = new RootViewModelContext(this, vm);

            vm.Property1 = "Hello";

            this.viewModelContext.Bindings.AddBinding(this.label1, "Text", this.viewModelContext.ViewModel, "Property1");

            this.viewModelContext.Bindings.AddEventTriggeredBinding(this.field1, "Text", "Ended", this.viewModelContext.ViewModel, "Property1");
        }
    }
}
