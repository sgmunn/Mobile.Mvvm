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

namespace Sample.Droid.SampleActivities
{
    public interface IViewModelLoader<TViewModel> where TViewModel : IViewModel
    {
        Task Load();
        void Cancel();
    }

    public class ViewModelLoader<TViewModel> : IViewModelLoader<TViewModel> where TViewModel : IViewModel
    {
        private readonly CancellationTokenSource cancellation;

        private readonly Action<TViewModel> load;

        private readonly TViewModel viewModel;

        public ViewModelLoader(TViewModel viewModel, Action<TViewModel> load)
        {
            this.viewModel = viewModel;
            this.load = load;
            this.cancellation = new CancellationTokenSource();
        }

        public Task Load()
        {
            return Task.Factory.StartNew(this.PerformLoad, this.cancellation.Token);
        }

        public void Cancel()
        {
            this.cancellation.Cancel();
        }

        protected virtual void PerformLoad()
        {
            this.load(this.viewModel);
        }
    }

    public class ViewModelLoader
    {
        private readonly CancellationTokenSource cancellation;

        private readonly Func<CancellationToken, Task> loader;

        public ViewModelLoader(Func<CancellationToken, Task> loader)
        {
            this.loader = loader;
            this.cancellation = new CancellationTokenSource();
        }

        public virtual Task Load()
        {
            return this.loader(this.cancellation.Token);
        }

        public virtual void Cancel()
        {
            this.cancellation.Cancel();
        }
    }



    [Activity (Label = "PlainActivity")]            
    public class PlainActivity : Activity
    {
        private IRootViewModelContext viewModelContext;

        private TextView label1;

        private EditText field1;

        private Button button1;
        
        private Button button2;

        private ICommandBinding binding;

        private ViewModelLoader loader;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            this.SetContentView(Resource.Layout.PlainActivityLayout);

            this.label1 = this.FindViewById<TextView>(Resource.Id.textView1);
            this.field1 = this.FindViewById<EditText>(Resource.Id.editText1);
            this.button1 = this.FindViewById<Button>(Resource.Id.button1);
            this.button2 = this.FindViewById<Button>(Resource.Id.button2);

            var vm = new SimpleViewModel();
            this.viewModelContext = new RootViewModelContext(this, vm);

            this.viewModelContext.Bindings.AddBinding(label1, "Text", this.viewModelContext.ViewModel, "Property1");

            this.viewModelContext.Bindings.AddEventTriggeredBinding(field1, "Text", "TextChanged", this.viewModelContext.ViewModel, "Property1");

            // the idea here is to be able to bind, Click to an command and Enabled to CanExecute
            //this.button1.Click += (sender, e) => vm.TestCommand.Execute();
            //this.button1.Enabled = vm.TestCommand.GetCanExecute();
            this.binding = new WeakCommandBinding(this.button1, "Click", "Enabled", vm.TestCommand);
            this.binding.Bind();

            this.binding = new WeakCommandBinding(this.button2, "Click", "Enabled", vm.TestCommand2);
            this.binding.Bind();

            this.loader = new ViewModelLoader(this.DoLoad);
        }

        protected override void OnPause()
        {
            this.loader.Cancel();
            base.OnPause();
        }

        protected override void OnResume()
        {
            base.OnResume();
            this.loader.Load();
        }

        private Task DoLoad(CancellationToken cancel)
        {
            return GetHelloWorld(cancel).ContinueWith((x) => this.UpdateViewModel(x.Result), cancel);
        }

        private void UpdateViewModel(string x)
        {
            this.RunOnUiThread(() => {
                Console.WriteLine("update UI thread {0}", System.Threading.Thread.CurrentThread.ManagedThreadId);
                ((SimpleViewModel)this.viewModelContext.ViewModel).Property1 = x;
            });
        }

        private async Task<string> GetHelloWorld(CancellationToken cancel)
        {
            Console.WriteLine("get data thread {0}", System.Threading.Thread.CurrentThread.ManagedThreadId);
            await Task.Delay(3000);

            if (cancel.IsCancellationRequested)
            {
                Console.WriteLine("cancelled");
                return "xx";
            }

            Console.WriteLine("get data thread {0}", System.Threading.Thread.CurrentThread.ManagedThreadId);
            return "hello world";
        }
    }
}

