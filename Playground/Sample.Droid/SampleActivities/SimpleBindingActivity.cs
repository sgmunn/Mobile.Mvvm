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
using Mobile.Utils;

namespace Sample.Droid.SampleActivities
{
    public class SimpleBindingFragment : Android.Support.V4.App.Fragment
    {
        private SimpleViewModel viewModel;

        private ViewModelContext bindingContext;

        private TextView label1;

        private EditText field1;

        private Button button1;

        private Button button2;

        private ViewModelLoader<string> loader;

        public SimpleBindingFragment()
        {
            // this.RetainInstance = true;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            this.viewModel = new SimpleViewModel();

            // we can either put this here or in will appear (onResume), depending on what we need to do with loading for the VM.
            this.loader = new ViewModelLoader<string>(this.GetHelloWorld, this.UpdateViewModel, new UIThreadScheduler());
        }

        public override Android.Views.View OnCreateView(Android.Views.LayoutInflater inflater, Android.Views.ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.PlainActivityLayout, null);

            this.label1 = view.FindViewById<TextView>(Resource.Id.textView1);
            this.field1 = view.FindViewById<EditText>(Resource.Id.editText1);
            this.button1 = view.FindViewById<Button>(Resource.Id.button1);
            this.button2 = view.FindViewById<Button>(Resource.Id.button2);

            return view;
        }

        public override void OnPause()
        {
            this.loader.Cancel();
            this.bindingContext.Dispose();
            base.OnPause();
        }

        public override void OnResume()
        {
            base.OnResume();
            this.BindViewModel();
            this.loader.Load();
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            this.viewModel.SaveState().Save(outState);

            Console.WriteLine("save instance state");
        }

        public override void OnViewStateRestored(Bundle savedInstanceState)
        {
            base.OnViewStateRestored(savedInstanceState);
            this.viewModel.RestoreState(savedInstanceState.ToStateBundle());
            Console.WriteLine("restore instance state");
        }

        private void BindViewModel()
        {
            this.bindingContext = new ViewModelContext(this.Activity, this.viewModel);

            this.bindingContext.Bind(label1, "Text", this.viewModel, "Property1");
            this.bindingContext.Bind(field1, "Text", "TextChanged", this.viewModel, "Property1");

            // TODO: when we have property path bindings working, we can bind to Errors[Property1] for example
            this.bindingContext.Bind(field1, "Error", this.viewModel, "Property1Error");

            this.bindingContext.Bind(this.button1, "Click", "Enabled", this.viewModel.TestCommand);
            this.bindingContext.Bind(this.button2, "Click", "Enabled", this.viewModel.TestCommand2);
        }

        private void UpdateViewModel(string x)
        {
            Console.WriteLine("update UI thread {0}", System.Threading.Thread.CurrentThread.ManagedThreadId);
            ((SimpleViewModel)this.bindingContext.ViewModel).Property1 = x;
        }

        private async Task<string> GetHelloWorld(CancellationToken cancel)
        {
            Console.WriteLine("1st get data thread {0}", System.Threading.Thread.CurrentThread.ManagedThreadId);
            await Task.Delay(3000).ConfigureAwait(false);

            Console.WriteLine("2nd get data thread {0}", System.Threading.Thread.CurrentThread.ManagedThreadId);

            if (cancel.IsCancellationRequested)
            {
                Console.WriteLine("cancelled");
                return "xx";
            }

            Console.WriteLine(">> get data thread {0}", System.Threading.Thread.CurrentThread.ManagedThreadId);
            return "hello world";
        }
    }

    [Activity (Label = "Simple Binding")]            
    public class SimpleBindingActivity : FragmentActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            this.SetContentView(Resource.Layout.EmptyFrameLayout);

            var fragment = this.SupportFragmentManager.FindFragmentByTag("content");
            if (fragment == null)
            {
                fragment = new SimpleBindingFragment();
                this.SupportFragmentManager.BeginTransaction()
                    .Replace(Resource.Id.content, fragment, "content")
                    .Commit();
            }
        }
    }
}

