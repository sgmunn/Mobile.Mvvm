// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FragmentNavigationService.cs" company="sgmunn">
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
using Android.Support.V4.App;

namespace Mobile.Mvvm.App
{
    using System;
    using Android.Support.V4.App;
    using Mobile.Utils;
    using System.Collections.Generic;

    /// <summary>
    /// Provides an implementation of INavigationService that uses the fragment manager to navigate
    /// </summary>
    public class FragmentNavigationService : INavigationService
    {
        private readonly FragmentManager fragmentManager;

        private readonly int contentId;

        private readonly Dictionary<Type, Type> viewModelMapping;

        private int enterAnimation;

        private int exitAnimation;

        private int popEnterAnimation;

        private int popExitAnimation;

        public FragmentNavigationService(FragmentManager fragmentManager, int contentId)
        {
            fragmentManager.EnsureNotNull("fragmentManager");

            this.fragmentManager = fragmentManager;
            this.contentId = contentId;
            this.viewModelMapping = new Dictionary<Type, Type>();
        }

        public FragmentNavigationService AnimatePush(int enter, int exit)
        {
            this.enterAnimation = enter;
            this.exitAnimation = exit;
            return this;
        }

        public FragmentNavigationService AnimatePop(int enter, int exit)
        {
            this.popEnterAnimation = enter;
            this.popExitAnimation = exit;
            return this;
        }

        public FragmentNavigationService Register<TViewModel, TFragment>()
            where TFragment : Fragment
        {
            this.viewModelMapping.Add(typeof(TViewModel), typeof(TFragment));
            return this;
        }

        public virtual void Push<TViewModel>()
        {
            this.Push<TViewModel>(null);
        }

        public virtual void Push<TViewModel>(IDictionary<string, string> args)
        {
            var fragment = this.CreateFragment<TViewModel>(args);
            this.PerformPush(fragment);
        }

        public virtual void Pop()
        {
            this.fragmentManager.PopBackStack();
        }

        public virtual void PresentModal<TViewModel>()
        {
            this.PresentModal<TViewModel>(null);
        }

        public virtual void PresentModal<TViewModel>(IDictionary<string, string> args)
        {
            var fragment = this.CreateFragment<TViewModel>(args) as DialogFragment;
            if (fragment == null)
            {
                throw new InvalidOperationException("To PresentModel the mapped view model must return a DialogFragment");
            }

            fragment.Show(this.fragmentManager, this.MakeDialogTag());
        }

        protected virtual Fragment CreateFragment<TViewModel>(IDictionary<string, string> args)
        {
            var viewModelType = typeof(TViewModel);
            if (!this.viewModelMapping.ContainsKey(viewModelType))
            {
                throw new InvalidOperationException(string.Format("mapping does not contain an entry for {0}", viewModelType));
            }

            var fragmentType = this.viewModelMapping[viewModelType];

            var fragment = System.Activator.CreateInstance(fragmentType) as Fragment;
            if (fragment == null)
            {
                throw new InvalidOperationException("Did not create a Fragment from the given view model type");
            }

            if (args != null)
            {
                fragment.Arguments = new Android.OS.Bundle();
                foreach (var arg in args)
                {
                    // IMPROVE: can we improve how we're passing args to a fragment other than devolving to strings
                    fragment.Arguments.PutString(arg.Key, arg.Value);
                }
            }

            return fragment;
        }

        protected virtual void PerformPush(Fragment fragment)
        {
            var transaction = this.fragmentManager.BeginTransaction();

            if (this.enterAnimation != 0 && this.popEnterAnimation != 0)
            {
                transaction.SetCustomAnimations(this.enterAnimation,this.exitAnimation, this.popEnterAnimation, this.popExitAnimation);
            }
            else
            if (this.enterAnimation != 0)
            {
                    transaction.SetCustomAnimations(this.enterAnimation,this.exitAnimation);
            }

            transaction.Replace(this.contentId, fragment, this.MakeContentTag());
            transaction.AddToBackStack(null);
            transaction.Commit();
        }

        protected string MakeContentTag()
        {
            return string.Format("nav_content_{0}", this.contentId);
        }

        protected string MakeDialogTag()
        {
            return string.Format("nav_dialog_{0}", this.contentId);
        }
    }
}

