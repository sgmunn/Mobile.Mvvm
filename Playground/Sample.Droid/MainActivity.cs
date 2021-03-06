using System;
using Android.App;
using Android.Content;
using Android.Widget;
using Android.OS;
using Sample.Droid.SampleActivities;
using Mobile.Mvvm;

namespace Sample.Droid
{
    [Activity (Label = "Sample.Droid", MainLauncher = true)]
    public class MainActivity : Activity
    {
        static MainActivity()
        {
            MvvmCore.Init();
            Mobile.Utils.Diagnostics.Log.Init(Mobile.Utils.Diagnostics.ConsoleLog.Instance);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            this.SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            this.FindViewById<Button>(Resource.Id.button1).Click += delegate
            {
                var intent = new Intent(this, typeof(SimpleBindingActivity));
                this.StartActivity(intent);
            };

            this.FindViewById<Button>(Resource.Id.button2).Click += delegate
            {
                var intent = new Intent(this, typeof(SimpleListActivity));
                this.StartActivity(intent);
            };

            this.FindViewById<Button>(Resource.Id.button3).Click += delegate
            {
                var intent = new Intent(this, typeof(SimpleViewModelActivity));
                this.StartActivity(intent);
            };

            this.FindViewById<Button>(Resource.Id.button4).Click += delegate
            {
                //var intent = new Intent(this, typeof(WidgetSampleActivity));
                var intent = new Intent(this, typeof(NavigationActivity));
                this.StartActivity(intent);
            };
        }
    }
}


