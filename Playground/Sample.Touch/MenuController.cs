using System;
using UIKit;
using Mobile.Mvvm.ViewModel.Dialog;
using System.Collections.Generic;
using Mobile.Mvvm;
using Sample.Touch.SampleControllers;

namespace Sample.Touch
{
    public class MenuController : UITableViewController
    {
        private GroupedListSource source;

        public MenuController() : base (UITableViewStyle.Grouped)
        {
        }

        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            this.source = new GroupedListSource();
            this.source.TableView = this.TableView;

            // Register the TableView's data source
            this.TableView.Source = this.source;

            var groups = new List<IGroup>();

            var section1 = new GroupViewModel();
            section1.Header = new CaptionViewModel("Samples");
            groups.Add(section1);

            section1.Rows.Add(new StringElementViewModel("Simple Binding") { TapCommand = new DelegateCommand(() => {
                this.NavigationController.PushViewController(new SimpleBindingViewController(), true);
            }) });

            section1.Rows.Add(new StringElementViewModel("Simple List") { TapCommand = new DelegateCommand(() => {
                this.NavigationController.PushViewController(new SimpleListController(), true);
            }) });

            section1.Rows.Add(new StringElementViewModel("Wrapped View Model Binding To List") { TapCommand = new DelegateCommand(() => {
                this.NavigationController.PushViewController(new SimpleViewModelController(), true);
            }) });

            section1.Rows.Add(new StringElementViewModel("Widget Samples") { TapCommand = new DelegateCommand(() => {
                this.NavigationController.PushViewController(new WidgetSampleController(), true);
            }) });


            this.source.Bind(groups);
        }
    }
}

