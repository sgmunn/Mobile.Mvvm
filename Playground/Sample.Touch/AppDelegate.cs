// --------------------------------------------------------------------------------------------------------------------
// <copyright file=".cs" company="sgmunn">
//   (c) sgmunn 2013  
//
//   Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//   documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
//   the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
//   to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
//   The above copyright notice and this permission notice shall be included in all copies or substantial portions of 
//   the Software.
//
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO 
//   THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
//   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
//   CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS 
//   IN THE SOFTWARE.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Mobile.Mvvm.Disposables;
using Mobile.Mvvm.DataBinding;
using System.Reflection;
using Mobile.Mvvm;

namespace Sample.Touch
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register ("AppDelegate")]
    public partial class AppDelegate : UIApplicationDelegate
    {
        // class-level declarations
        UIWindow window;
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            // create a new window instance based on the screen size
            window = new UIWindow(UIScreen.MainScreen.Bounds);
            
            // If you have defined a root view controller, set it here:
            // window.RootViewController = myViewController;
            
            // make the window visible
            window.MakeKeyAndVisible();

            this.Test();
            
            return true;
        }

        private void Test()
        {
            var x = new TestMe();
            var z = BindingScopeExtensions.AddEventBinding<TestMe, EventArgs>(null, x, "SomethingChanged");
            x.Fire();


//            var addMethod = default(MethodInfo);
//            var removeMethod = default(MethodInfo);
//            var delegateType = default(Type);
//            var isWinRT = default(bool);
//            ReflectionUtils.GetEventMethods<TestMe, EventArgs>(x, "SomethingChanged", out addMethod, out removeMethod, out delegateType, out isWinRT);
//
//            if (addMethod != null)
//            {
//
//            }



           // var parser = new MvxPropertyExpressionParser();
          //  parser.Parse()
        }
    }

    public class TestMe
    {
        public string Field1 { get; set; }

        public event EventHandler SomethingChanged;

        public void Fire()
        {
            if (this.SomethingChanged != null)
                this.SomethingChanged(this, new EventArgs());
        }
    }
}

