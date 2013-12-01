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
using Android.Support.V4.App;
using Mobile.Mvvm.App;
using Mobile.Utils;

namespace Sample.Droid.SampleActivities
{
    public class WidgetSampleFragment : Android.Support.V4.App.Fragment
    {
        private AlertDialogMessageDisplay messageDisplay;

        public WidgetSampleFragment()
        {
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            this.messageDisplay = new AlertDialogMessageDisplay(this.Activity);
        }

        public override Android.Views.View OnCreateView(Android.Views.LayoutInflater inflater, Android.Views.ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.WidgetSample, null);

            var button1 = view.FindViewById<Button>(Resource.Id.button1);

            button1.Click += (object sender, EventArgs e) => {
                messageDisplay.DisplayMessage("title", "this is a message", Actions.Null);
            };

            return view;
        }

        public override void OnPause()
        {
            this.messageDisplay.Pause();
            base.OnPause();
        }

        public override void OnResume()
        {
            base.OnResume();
            this.messageDisplay.Resume();
        }
    }

    [Activity(Label = "WidgetSampleActivity")]            
    public class WidgetSampleActivity : FragmentActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            this.SetContentView(Resource.Layout.EmptyFrameLayout);

            var fragment = this.SupportFragmentManager.FindFragmentByTag("content");
            if (fragment == null)
            {
                this.SupportFragmentManager.BeginTransaction()
                .Replace(Resource.Id.content, new WidgetSampleFragment(), "content")
                .Commit();
            }
        }
    }
}

