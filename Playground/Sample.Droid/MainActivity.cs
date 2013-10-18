using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Sample.Droid.SampleActivities;

namespace Sample.Droid
{
    [Activity (Label = "Sample.Droid", MainLauncher = true)]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            Android.Views.LayoutInflater.From(this).Factory = MyFactory.Default;

            // Set our view from the "main" layout resource
            this.SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            this.FindViewById<Button>(Resource.Id.button1).Click += delegate
            {
                var intent = new Intent(this, typeof(PlainActivity));
                this.StartActivity(intent);
            };

            this.FindViewById<Button>(Resource.Id.button2).Click += delegate
            {
                var intent = new Intent(this, typeof(SimpleListActivity));
                this.StartActivity(intent);
            };
        }
    }
}


