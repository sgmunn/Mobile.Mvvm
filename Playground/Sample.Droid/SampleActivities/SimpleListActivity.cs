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
using Android.Util;
using System.Threading;
using System.Collections.Concurrent;

namespace Sample.Droid.SampleActivities
{
    public static class DialogDataTemplates
    {
        // if we have a context passed in, then we might as well pass in the context for the binding scope as well!
        // only downside is that the template can't be shared across view contexts




        public static IEnumerable<IDataTemplate> DefaultTemplates(Context context)
        {
            var inflator = LayoutInflater.FromContext(context);

//            yield return new DataTemplate(Android.Resource.Layout.SimpleListItem1)
//                .Creates<View>((id, args) => inflator.Inflate((int)id, (ViewGroup)args[0], false))
//                    .WhenBinding<StringViewModel, View>((c, vm, view) => {
//                        // at the moment we have to do this bit for android, unless we have a custom view, which isn't really likely
//                        // this is the only way we can do this when we use a built-in layout
//                        c.Bindings.AddBindings(view.FindViewById<TextView>(Android.Resource.Id.Text1), vm, "Text: Caption");
//                    });
//

            // pass the context to the template,
            // the template can create the view from the layout, if we gave it an id or we gave it a factory with a id, layout, TView signature


            yield return new DataTemplate<View, StringViewModel>(inflator, Android.Resource.Layout.SimpleListItem1)
                .BindChildView(Android.Resource.Id.Text1, "Text: Caption");

             //   .OnCreate((id, viewGroup) => inflator.Inflate(id, viewGroup, false))

                // multiple of these, where the id is a child view of the view that was created
                // .BindChildView(Android.Resource.Id.Text1, "Text: Caption")

//                    .Bind((c, vm, view) => {
//                        // at the moment we have to do this bit for android, unless we have a custom view, which isn't really likely
//                        // this is the only way we can do this when we use a built-in layout
//                        c.Bindings.AddBindings(view.FindViewById(Android.Resource.Id.Text1), vm, "Text: Caption");
//                    });





            // android only
            // Creates<>(id, attach)
            // WhenBinding<ViewModel>(id, string);

            // we have a context, can we get an injected 

            // Bind<ViewModel>(resourceId, expression);

            // if we use our own layout, in which case we want to set up our own layout with factory
            // we can then get the binding text based on the id


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


    public class MyFactory : Java.Lang.Object, LayoutInflater.IFactory
    {
        private readonly ConcurrentDictionary<string, string> bindings;

        public static MyFactory Default = new MyFactory();

        public MyFactory()
        {
            this.bindings = new ConcurrentDictionary<string, string>();    
        }

        public string BindingForId(int id)
        {
            var key = "@" + id.ToString();  
            if (this.bindings.ContainsKey(key))
            {
                return this.bindings[key];
            }

            return string.Empty;
        }

        public View OnCreateView(string name, Context context, Android.Util.IAttributeSet attrs)
        {
            var x = attrs.GetAttributeValue("http://schemas.android.com/apk/res/android", "id");
            //Console.WriteLine("id = {0}", x);
            
            var y = attrs.GetAttributeValue("http://schemas.mvvm.mobile.com/android", "bind");
            //Console.WriteLine("bind = {0}", y);

            if (!string.IsNullOrEmpty(x) && !string.IsNullOrEmpty(y))
            {
                this.bindings.TryAdd(x, y);
            }

            return null;
        }
    }


    [Activity (Label = "SimpleListActivity")]            
    public class SimpleListActivity : ListActivity
    {
        private GroupedListSource source;
        private IList<IGroup> groups;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            Android.Views.LayoutInflater.From(this).Factory = MyFactory.Default;


            this.groups = new ObservableCollection<IGroup>();

            var section1 = new GroupViewModel();
            section1.Header = new StringViewModel("Header 1");
            this.groups.Add(section1);

            section1.Rows.Add(new TestCommandRowViewModel(section1, "add"));
            section1.Rows.Add(new StringElementViewModel("update") { TapCommand = new DelegateCommand(() => {
                    ((StringViewModel)section1.Rows[1]).Caption = "Updated!";
                }) });
            section1.Rows.Add(new StringViewModel("item 1"));
            section1.Rows.Add(new StringViewModel("item 2"));

            this.groups.Add(new GroupViewModel());
            this.groups[1].Rows.Add(new StringViewModel("item 1"));
            this.groups[1].Rows.Add(new StringViewModel("item 2"));
            this.groups[1].Rows.Add(new StringViewModel("item 3"));
            
            this.groups[1].Header = new StringViewModel("Header 2");
            this.groups[1].Footer = new StringViewModel("Footer 2");

            this.groups.Add(new GroupViewModel());
            this.groups[2].Header = new StringViewModel("Header 3");
            for (int i = 0; i < 100; i++)
            {
                this.groups[2].Rows.Add(new StringViewModel("item " + i.ToString()));
            }

            // view did load equivalent
            this.source = new GroupedListSource(this, DialogDataTemplates.DefaultTemplates(this));
            this.source.ListView = this.ListView;


            // Register the TableView's data source
            this.ListView.Adapter = this.source;

            this.source.Bind(this.groups);


            // Create your application here
        }
    }
}

