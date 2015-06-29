using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;
using Mobile.Mvvm;

namespace Sample.Touch
{
    public class Application
    {
        // This is the main entry point of the application.
        static void Main(string[] args)
        {
            MvvmCore.Init();
            Mobile.Utils.Diagnostics.Log.Init(Mobile.Utils.Diagnostics.ConsoleLog.Instance);

            // if you want to use a different Application Delegate class from "AppDelegate"
            // you can specify it here.
            UIApplication.Main(args, null, "AppDelegate");
        }
    }
}
