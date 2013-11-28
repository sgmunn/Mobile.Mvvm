using System;
using Android.App;
using Android.OS;
using Mobile.Mvvm.ViewModel;
using SampleViewModels;
using Android.Widget;
using Mobile.Mvvm;
using Mobile.Mvvm.DataBinding;
using System.Threading.Tasks;
using Android;
using Android.Util;
using System.Threading;
using Mobile.Utils.Tasks;
using Android.Support.V4.App;
using Android.Views;
using System.Collections.Concurrent;
using Android.Content;

namespace Sample.Droid.SampleActivities
{
    public class SimpleBindingFragment : Android.Support.V4.App.Fragment
    {
        private SimpleViewModel viewModel;

        private IRootViewModelContext viewModelContext;

        private TextView label1;

        private EditText field1;

        private Button button1;

        private Button button2;

        private ViewModelLoader<string> loader;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            this.viewModel = new SimpleViewModel();
        }

        public override Android.Views.View OnCreateView(Android.Views.LayoutInflater inflater, Android.Views.ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.PlainActivityLayout, null);

            this.label1 = view.FindViewById<TextView>(Resource.Id.textView1);
            this.field1 = view.FindViewById<EditText>(Resource.Id.editText1);
            this.button1 = view.FindViewById<Button>(Resource.Id.button1);
            this.button2 = view.FindViewById<Button>(Resource.Id.button2);

            this.loader = new ViewModelLoader<string>(this.GetHelloWorld, this.UpdateViewModel, new UIThreadScheduler());

            return view;
        }

        public override void OnAttach(Android.App.Activity activity)
        {
            base.OnAttach(activity);
        }

        public override void OnPause()
        {
            this.loader.Cancel();
            this.viewModelContext.Dispose();
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
            this.viewModelContext = new RootViewModelContext(this.Activity, this.viewModel);

            this.viewModelContext.Bind(label1, "Text", this.viewModel, "Property1");
            this.viewModelContext.Bind(field1, "Text", "TextChanged", this.viewModel, "Property1");

            this.viewModelContext.Bind(this.button1, "Click", "Enabled", this.viewModel.TestCommand);
            this.viewModelContext.Bind(this.button2, "Click", "Enabled", this.viewModel.TestCommand2);
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



    [Activity (Label = "PlainActivity")]            
    public class PlainActivity : FragmentActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            this.SetContentView(Resource.Layout.EmptyFrameLayout);

            this.SupportFragmentManager.BeginTransaction()
                .Replace(Resource.Id.content, new SimpleBindingFragment())
                .Commit();
        }
    }
}

