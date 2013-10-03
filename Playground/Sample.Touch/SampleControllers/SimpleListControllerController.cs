using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Mobile.Mvvm.ViewModel;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace Sample.Touch.SampleControllers
{
    public class SimpleListControllerController : UITableViewController
    {
        private SectionSource source;
        private IList<ISection> sections;

        public SimpleListControllerController() : base (UITableViewStyle.Grouped)
        {
            this.sections = new ObservableCollection<ISection>();
            this.sections.Add(new SectionViewModel());
            this.sections[0].Rows.Add(new RowViewModel());
            this.sections[0].Rows.Add(new RowViewModel());
            this.sections[0].Rows.Add(new RowViewModel());
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

