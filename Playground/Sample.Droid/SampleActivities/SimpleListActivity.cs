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
using Android.Support.V4.App;
using Mobile.Utils.Tasks;
using System.Threading.Tasks;

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


            yield return new DataTemplate<View, StringWrapperElementViewModel>(inflator, Android.Resource.Layout.SimpleListItem2)
                    .BindChildView(Android.Resource.Id.Text2, "Text: Value")
                    .BindChildView(Android.Resource.Id.Text1, "Text: Caption")
                    ;

            yield return new DataTemplate<View, CaptionViewModel>(inflator, Android.Resource.Layout.SimpleListItem1)
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

    public class SimpleListFragment : Android.Support.V4.App.ListFragment
    {
        ////private SimpleViewModel viewModel;

        // this is a binding context
        private GroupedListSource source;

        private ViewModelLoader<string> loader;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            ////this.viewModel = new SimpleViewModel();
        }

        public override Android.Views.View OnCreateView(Android.Views.LayoutInflater inflater, Android.Views.ViewGroup container, Bundle savedInstanceState)
        {
            this.source = new GroupedListSource(this.Activity, DialogDataTemplates.DefaultTemplates(this.Activity));

            // we can either put this here or in will appear, depending on what we need to do with loading for the VM.
            this.loader = new ViewModelLoader<string>(this.GetHelloWorld, this.UpdateViewModel, new UIThreadScheduler());

            return base.OnCreateView(inflater, container, savedInstanceState);
        }

        public override void OnPause()
        {
            this.loader.Cancel();
            this.source.Dispose();
            base.OnPause();
        }

        public override void OnResume()
        {
            base.OnResume();
            this.BindViewModel();
            this.loader.Load();
        }

        private void BindViewModel()
        {
            this.source.ListView = this.ListView;
        }

        private void UpdateViewModel(string x)
        {
            //this.RunOnUiThread(() => {
            Console.WriteLine("update UI thread {0}", System.Threading.Thread.CurrentThread.ManagedThreadId);

            // do this here so that we get the indeterminate progress
            this.ListAdapter = this.source;
            var data = SimpleListViewModel.GetViewModel();
            this.source.Bind(data);

            // ((SimpleViewModel)this.viewModelContext.ViewModel).Property1 = x;
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

    [Activity (Label = "SimpleListActivity")]            
    public class SimpleListActivity : FragmentActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            this.SetContentView(Resource.Layout.EmptyFrameLayout);

            this.SupportFragmentManager.BeginTransaction()
                .Replace(Resource.Id.content, new SimpleListFragment())
                .Commit();
        }
    }
}

