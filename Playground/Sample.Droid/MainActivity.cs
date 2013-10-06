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
        int count = 1;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button>(Resource.Id.myButton);
            
            button.Click += delegate
            {
                var intent = new Intent(this, typeof(SimpleListActivity));
                this.StartActivity(intent);
                //button.Text = string.Format("{0} clicks!", count++);
            };
        }
    }
}


