// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NavigationService.cs" company="sgmunn">
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

namespace Mobile.Mvvm.App
{
    using System;
    using UIKit;
    using Mobile.Utils;
    using System.Collections.Generic;

    /// <summary>
    /// Provides an implementation of INavigationService that uses the NavigationController to navigate
    /// </summary>
    public class NavigationService : INavigationService
    {
        private readonly UINavigationController navController;

        private readonly Dictionary<Type, Type> viewModelMapping;

        public NavigationService(UINavigationController navController)
        {
            navController.EnsureNotNull("navController");
            this.navController = navController;
            this.viewModelMapping = new Dictionary<Type, Type>();
        }

        public NavigationService Register<TViewModel, TController>()
            where TController : UIViewController
        {
            this.viewModelMapping.Add(typeof(TViewModel), typeof(TController));
            return this;
        }

        public virtual void Push<TViewModel>()
        {
            this.Push<TViewModel>(null);
        }

        public virtual void Push<TViewModel>(IDictionary<string, string> args)
        {
            var controller = this.CreateController<TViewModel>(args);
            this.navController.PushViewController(controller, true);
        }

        public virtual void Pop()
        {
            this.navController.PopViewController(true);
        }

        public virtual void PresentModal<TViewModel>()
        {
            this.PresentModal<TViewModel>(null);
        }

        public virtual void PresentModal<TViewModel>(IDictionary<string, string> args)
        {
            var controller = this.CreateController<TViewModel>(args);
            this.navController.PresentViewController(controller, true, null);
        }

        protected virtual UIViewController CreateController<TViewModel>(IDictionary<string, string> args)
        {
            var viewModelType = typeof(TViewModel);
            if (!this.viewModelMapping.ContainsKey(viewModelType))
            {
                throw new InvalidOperationException(string.Format("mapping does not contain an entry for {0}", viewModelType));
            }

            var controllerType = this.viewModelMapping[viewModelType];

            var controller = System.Activator.CreateInstance(controllerType) as UIViewController;
            if (controller == null)
            {
                throw new InvalidOperationException("Did not create a UIViewController from the given view model type");
            }

            var navTarget = controller as INavigationTarget;
            if (navTarget != null)
            {
                navTarget.WillNavigate(args);
            }

            return controller;
        }
    }
}

