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
using SampleViewModels;
using Mobile.Mvvm.ViewModel.Dialog;
using Mobile.Mvvm;
using Mobile.Mvvm.DataBinding;

namespace Sample.Droid.SampleActivities
{
    public static class DialogDataTemplates
    {
        // if we have a context passed in, then we might as well pass in the context for the binding scope as well!
        // only downside is that the template can't be shared across view contexts




        public static IEnumerable<IDataTemplate> DefaultTemplates(Context context)
        {
            var inflator = LayoutInflater.FromContext(context);

            yield return new DataTemplate(Android.Resource.Layout.SimpleListItem1)
                .Creates<View>((id, root) => inflator.Inflate((int)id, (ViewGroup)root, false))

                    .WhenBinding<StringViewModel, View>((c, vm, view) => {
                        // at the moment we have to do this bit for android, unless we have a custom view, which isn't really likely
                        var text1 = view.FindViewById<TextView>(Android.Resource.Id.Text1);
                        c.Bindings.AddBinding(BindingParser.Default.Parse("Text: Caption", text1, vm));
                    });


//                    .WhenBinding<StringViewModel, View>((c, vm, view) => {
//                        var text1 = view.FindViewById<TextView>(Android.Resource.Id.Text1);
//
//                        //c.Bindings.AddEventTriggeredBinding(text1, "Text", "TextChanged", vm, "Caption");
//
//                        //c.Bindings.AddBinding(text1, "Text", vm, "Caption");
//
//                        var targetPropertySetter = new DelegatePropertyAccessor<TextView, string>(x => x.Text, (x,v) => x.Text = v);
//
//                        // this one has the advantage of being cross platform
//                        var sourcePropertySetter = new DelegatePropertyAccessor<StringViewModel, string>(x => x.Caption, (x,v) => x.Caption = v);
//                        var binding = new Binding("Caption", sourcePropertySetter);
//
//                        c.Bindings.AddBinding(view, "Text", targetPropertySetter, vm, binding);
//
//                    });
        }
    }



    [Activity (Label = "SimpleListActivity")]            
    public class SimpleListActivity : ListActivity
    {
        private SectionSource source;
        private IList<ISection> sections;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            this.sections = new ObservableCollection<ISection>();

            var section1 = new SectionViewModel();
            section1.Header = new StringViewModel("Header 1");
            this.sections.Add(section1);

            section1.Rows.Add(new TestCommandRowViewModel(section1, "add"));
            section1.Rows.Add(new StringElementViewModel("update") { TapCommand = new DelegateCommand(() => {
                    ((StringViewModel)section1.Rows[1]).Caption = "Updated!";
                }) });
            section1.Rows.Add(new StringViewModel("item 1"));
            section1.Rows.Add(new StringViewModel("item 2"));

            this.sections.Add(new SectionViewModel());
            this.sections[1].Rows.Add(new StringViewModel("item 1"));
            this.sections[1].Rows.Add(new StringViewModel("item 2"));
            this.sections[1].Rows.Add(new StringViewModel("item 3"));
            
            this.sections[1].Header = new StringViewModel("Header 2");
            this.sections[1].Footer = new StringViewModel("Footer 2");

            this.sections.Add(new SectionViewModel());
            this.sections[2].Header = new StringViewModel("Header 3");
            for (int i = 0; i < 100; i++)
            {
                this.sections[2].Rows.Add(new StringViewModel("item " + i.ToString()));
            }

            // view did load equivalent
            this.source = new SectionSource(this, DialogDataTemplates.DefaultTemplates(this));
            this.source.ListView = this.ListView;


            // Register the TableView's data source
            this.ListView.Adapter = this.source;

            this.source.Bind(this.sections);


            // Create your application here
        }
    }
}

