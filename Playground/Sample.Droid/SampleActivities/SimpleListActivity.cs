using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Collections.ObjectModel;
using Mobile.Mvvm.ViewModel;

namespace Sample.Droid.SampleActivities
{
    [Activity (Label = "SimpleListActivity")]            
    public class SimpleListActivity : ListActivity
    {
        private SectionSource source;
        private IList<ISection> sections;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            this.sections = new ObservableCollection<ISection>();
            this.sections.Add(new SectionViewModel());

            this.sections[0].Rows.Add(new RowViewModel());
            this.sections[0].Rows.Add(new RowViewModel());
            this.sections[0].Rows.Add(new RowViewModel());
            
            this.sections[0].Header = new StringViewModel("Header");

            this.sections.Add(new SectionViewModel());
            this.sections[1].Rows.Add(new RowViewModel());
            this.sections[1].Rows.Add(new RowViewModel());
            this.sections[1].Rows.Add(new RowViewModel());
            
            this.sections[1].Header = new StringViewModel("Header");
            this.sections[1].Footer = new StringViewModel("Footer");

            // view did load equivalent
            this.source = new SectionSource(this);
            this.source.ListView = this.ListView;


            // Register the TableView's data source
            this.ListView.Adapter = this.source;

            this.source.Bind(this.sections);


            // Create your application here
        }
    }
}

