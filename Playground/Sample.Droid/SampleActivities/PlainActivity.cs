using System;
using Android.App;
using Android.OS;
using Mobile.Mvvm.ViewModel;
using SampleViewModels;
using Android.Widget;
using Mobile.Mvvm;
using Mobile.Mvvm.DataBinding;

namespace Sample.Droid.SampleActivities
{




    [Activity (Label = "PlainActivity")]            
    public class PlainActivity : Activity
    {
        private IRootViewModelContext viewModelContext;

        private TextView label1;

        private EditText field1;

        private Button button1;
        
        private Button button2;

        private ICommandBinding binding;

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

            vm.Property1 = "Hello";

            this.viewModelContext.Bindings.AddBinding(label1, "Text", this.viewModelContext.ViewModel, "Property1");

            this.viewModelContext.Bindings.AddEventTriggeredBinding(field1, "Text", "TextChanged", this.viewModelContext.ViewModel, "Property1");

            // the idea here is to be able to bind, Click to an command and Enabled to CanExecute
            //this.button1.Click += (sender, e) => vm.TestCommand.Execute();
            //this.button1.Enabled = vm.TestCommand.GetCanExecute();
            this.binding = new WeakCommandBinding(this.button1, "Click", "Enabled", vm.TestCommand);
            this.binding.Bind();

            this.binding = new WeakCommandBinding(this.button2, "Click", "Enabled", vm.TestCommand2);
            this.binding.Bind();
        }
    }
}

