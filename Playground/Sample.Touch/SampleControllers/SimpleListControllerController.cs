using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Mobile.Mvvm.ViewModel;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using SampleViewModels;
using Mobile.Mvvm.ViewModel.Dialog;

namespace Sample.Touch.SampleControllers
{
    public class SimpleListControllerController : UITableViewController
    {
        private SectionSource source;
        private IList<ISection> sections;

        public SimpleListControllerController() : base (UITableViewStyle.Grouped)
        {
            this.sections = new ObservableCollection<ISection>();

            var section1 = new SectionViewModel();
            section1.Header = new StringViewModel("Header");
            this.sections.Add(section1);

            section1.Rows.Add(new TestCommandRowViewModel(section1, "Add"));
            section1.Rows.Add(new StringViewModel("item 1"));
            section1.Rows.Add(new StringViewModel("item 2"));

            this.sections.Add(new SectionViewModel());
            this.sections[1].Rows.Add(new StringViewModel("item 1"));
            this.sections[1].Rows.Add(new StringViewModel("item 2"));
            this.sections[1].Rows.Add(new StringViewModel("item 3"));

//            this.NavigationItem.RightBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Add, (s,e) => {
//                this.sections[0].Rows.Add(new RowViewModel());
//            });
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
            this.source = new SectionSource();
            this.source.TableView = this.TableView;

            
            // Register the TableView's data source
            this.TableView.Source = this.source;

            this.source.Bind(this.sections);
        }
    }
}

