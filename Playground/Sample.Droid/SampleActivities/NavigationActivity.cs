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

namespace Sample.Droid.SampleActivities
{

    public class Page1Fragment : Android.Support.V4.App.DialogFragment
    {
        private FragmentNavigationService navService;

        public Page1Fragment()
        {
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            //this.FragmentManager;
            this.navService = new FragmentNavigationService(this.FragmentManager, Resource.Id.content)
                .AnimatePush(Resource.Animation.slide_in_right, Resource.Animation.slide_out_left)
                .AnimatePop(Resource.Animation.slide_in_left, Resource.Animation.slide_out_right)
                .Register<string, Page2Fragment>();
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {

            var b = new AlertDialog.Builder(this.Activity)
                    //.setIcon(R.drawable.alert_dialog_icon)
                .SetTitle("new title");
//                    .setPositiveButton(R.string.alert_dialog_ok,
//                        new DialogInterface.OnClickListener() {
//                            public void onClick(DialogInterface dialog, int whichButton) {
//                                ((FragmentAlertDialog)getActivity()).doPositiveClick();
//                            }
//                        }
//                    )
//                    .setNegativeButton(R.string.alert_dialog_cancel,
//                        new DialogInterface.OnClickListener() {
//                            public void onClick(DialogInterface dialog, int whichButton) {
//                                ((FragmentAlertDialog)getActivity()).doNegativeClick();
//                            }
//                        }
//                    )


            b.SetView(this.GetTheView());

            var dd = b.Create();

            //dd.RequestWindowFeature((int)Android.Views.WindowFeatures.);
            return dd;

            var d = base.OnCreateDialog(savedInstanceState);
            return d;
        }

        public override Android.Views.View OnCreateView(Android.Views.LayoutInflater inflater, Android.Views.ViewGroup container, Bundle savedInstanceState)
        {
            var view = base.OnCreateView(inflater, container, savedInstanceState);
            if (!this.ShowsDialog)
            {
                view = inflater.Inflate(Resource.Layout.WidgetSample, null);

                var button1 = view.FindViewById<Button>(Resource.Id.button1);
                button1.Text = "1 Push";

                button1.Click += (object sender, EventArgs e) => {
                    this.navService.Push<string>();
                    //                messageDisplay.DisplayMessage("title", "this is a message", Actions.Null);
                };
            }

            return view;
        }

        private View GetTheView()
        {
            var inflater = this.Activity.LayoutInflater;

            var view = inflater.Inflate(Resource.Layout.WidgetSample, null);

            var button1 = view.FindViewById<Button>(Resource.Id.button1);
            button1.Text = "1 Push";

            button1.Click += (object sender, EventArgs e) => {
                this.navService.Push<string>();
                //                messageDisplay.DisplayMessage("title", "this is a message", Actions.Null);
            };

            return view;
        }

        public override void OnPause()
        {
            base.OnPause();
        }

        public override void OnResume()
        {
            base.OnResume();
        }
    }

    public class Page2Fragment : Android.Support.V4.App.DialogFragment
    {
        private FragmentNavigationService navService;

        public Page2Fragment()
        {
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            //this.FragmentManager;
            this.navService = new FragmentNavigationService(this.FragmentManager, Resource.Id.content)
                .AnimatePush(Resource.Animation.slide_in_right, Resource.Animation.slide_out_left)
                .AnimatePop(Resource.Animation.slide_in_left, Resource.Animation.slide_out_right)
                .Register<string, Page1Fragment>();
        }

        public override Android.Views.View OnCreateView(Android.Views.LayoutInflater inflater, Android.Views.ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.WidgetSample, null);

            var button1 = view.FindViewById<Button>(Resource.Id.button1);
            button1.Text = "2 Push";

            button1.Click += (object sender, EventArgs e) => {
                this.navService.PresentModal<string>();
                //                messageDisplay.DisplayMessage("title", "this is a message", Actions.Null);
            };

            return view;
        }

        public override void OnPause()
        {
            base.OnPause();
        }

        public override void OnResume()
        {
            base.OnResume();
        }
    }


    [Activity(Label = "NavigationActivity")]            
    public class NavigationActivity : FragmentActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            this.SetContentView(Resource.Layout.EmptyFrameLayout);

            this.SupportFragmentManager.BackStackChanged += (object sender, EventArgs e) => {
                this.Title = string.Format("count {0}", this.SupportFragmentManager.BackStackEntryCount);
            };

            var fragment = this.SupportFragmentManager.FindFragmentByTag("content");
            if (fragment == null)
            {
                fragment = new Page1Fragment();
                this.SupportFragmentManager.BeginTransaction()
                    .Replace(Resource.Id.content, fragment, "content")
                    .Commit();
            }

        }
    }
}

