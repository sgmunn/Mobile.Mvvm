// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventTriggeredBindingExpression.cs" company="sgmunn">
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

namespace Mobile.Mvvm.DataBinding
{
    using System;
    using System.Reflection;
    using Mobile.Utils.Disposables;
    using Mobile.Utils.Reflection;

    public sealed class EventTriggeredBindingExpression : EventTriggeredBindingExpression<object, EventArgs>
    {
        public EventTriggeredBindingExpression(object target, string targetProperty, string targetEventName, object source, Binding binding)
            : base(target, targetProperty, targetEventName, source, binding)
        {
        }
    }

    public class EventTriggeredBindingExpression<TTargetType, TEventArgs> : WeakBindingExpression
        where TTargetType : class where TEventArgs : EventArgs
    {
        private readonly CompositeDisposable subscriptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="Mobile.Mvvm.DataBinding.EventTriggeredBindingExpression"/> class.
        /// </summary>
        public EventTriggeredBindingExpression(TTargetType target, string targetProperty, string targetEventName, object source, Binding binding)
            : base(target, targetProperty, source, binding)
        {
            this.TargetEventName = targetEventName;
            this.subscriptions = new CompositeDisposable();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Mobile.Mvvm.DataBinding.EventTriggeredBindingExpression"/> class.
        /// </summary>
        public EventTriggeredBindingExpression(TTargetType target, string targetProperty, string targetEventName, IPropertyAccessor accessor, object source, Binding binding)
            : base(target, targetProperty, accessor, source, binding)
        {
            this.TargetEventName = targetEventName;
            this.subscriptions = new CompositeDisposable();
        }

        public string TargetEventName { get; private set; }

        protected override void RegisterForPropertyChangesOnTarget(object obj)
        {
            //// Do not register for INPC as well. base.RegisterForPropertyChangesOnTarget(obj);

            var t = (TTargetType)obj;

            // look for event name
            var addMethod = default(MethodInfo);
            var removeMethod = default(MethodInfo);
            var delegateType = default(Type);
            var isWinRT = default(bool);
            ReflectionUtils.GetEventMethods(t.GetType(), this.TargetEventName, out addMethod, out removeMethod, out delegateType, out isWinRT);

            var eventWrapper = new WeakEventWrapper<IBindingExpression, EventArgs>(this, (t1, s1, e1) => {
                // update who we are bound to
                t1.UpdateSource();
            });

            var d = ReflectionUtils.CreateDelegate(delegateType, eventWrapper,  eventWrapper.GetType().GetMethod("HandleEvent"));
            addMethod.Invoke(t, new object[] { d });

            var unSubscribe = new AnonymousDisposable(() => {
                removeMethod.Invoke(t, new object[] { d });
            });

            this.subscriptions.Add(eventWrapper);
            this.subscriptions.Add(unSubscribe);
        }

        protected override void UnregisterForChanges()
        {
            //// base.UnregisterForChanges();
            this.subscriptions.Dispose();
        }
    }
    
}
