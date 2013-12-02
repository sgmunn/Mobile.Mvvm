// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WeakCommandBinding.cs" company="sgmunn">
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
    using Mobile.Utils;
    using Mobile.Utils.Disposables;
    using Mobile.Utils.Reflection;
    using System.Reflection;

    public interface IBindable : IDisposable
    {
        void Bind();
    }

    public interface ICommandBinding : IBindable
    {
        void ExecuteCommand();
    }

    
    public sealed class WeakCommandBinding : WeakCommandBinding<object, EventArgs>
    {
        public WeakCommandBinding(object target, string targetEventName, string targetProperty, ICommand command)
            : base(target, targetEventName, targetProperty, command)
        {
        }

        public WeakCommandBinding(object target, string targetEventName, string targetProperty, ICommand command, IPropertyAccessor accessor)
            : base(target, targetEventName, targetProperty, command, accessor)
        {
        }
    }


    public class WeakCommandBinding<TTargetType, TEventArgs> : ICommandBinding
        where TTargetType : class where TEventArgs : EventArgs
    {
        private readonly CompositeDisposable subscriptions;

        /// <summary>
        /// The target object that is bound
        /// </summary>
        private readonly WeakReference target;

        /// <summary>
        /// The source object that is bound.
        /// </summary>
        private readonly WeakReference command;

        /// <summary>
        /// Indicates that the expression has been disposed.
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="Mobile.Mvvm.DataBinding.WeakCommandBinding"/> class.
        /// </summary>
        public WeakCommandBinding(object target, string targetEventName, string targetProperty, ICommand command)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            if (string.IsNullOrEmpty(targetEventName))
            {
                throw new ArgumentNullException("targetEventName");
            }
            
            if (string.IsNullOrEmpty(targetProperty))
            {
                throw new ArgumentNullException("targetProperty");
            }

            if (command == null)
            {
                throw new ArgumentNullException("command");
            }

            this.subscriptions = new CompositeDisposable();
            this.SafelyUpdateTarget = false;
            this.PropertyAccessor = new ReflectionPropertyAccessor(targetProperty);
            this.TargetEventName = targetEventName;
            this.target = new WeakReference(target);
            this.command = new WeakReference(command);
            this.TargetProperty = targetProperty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Mobile.Mvvm.DataBinding.WeakCommandBinding"/> class.
        /// </summary>
        public WeakCommandBinding(object target, string targetEventName, string targetProperty, ICommand command, IPropertyAccessor accessor)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            if (string.IsNullOrEmpty(targetEventName))
            {
                throw new ArgumentNullException("targetEventName");
            }

            if (string.IsNullOrEmpty(targetProperty))
            {
                throw new ArgumentNullException("targetProperty");
            }

            if (command == null)
            {
                throw new ArgumentNullException("command");
            }

            if (accessor == null)
            {
                throw new ArgumentNullException("accessor");
            }

            this.subscriptions = new CompositeDisposable();
            this.SafelyUpdateTarget = false;
            this.PropertyAccessor = accessor;
            this.TargetEventName = targetEventName;
            this.target = new WeakReference(target);
            this.command = new WeakReference(command);
            this.TargetProperty = targetProperty;
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="Mobile.Mvvm.DataBinding.WeakCommandBinding"/> is reclaimed by garbage collection.
        /// </summary>
        ~WeakCommandBinding()
        {
            Dispose(false);
        }

        /// <summary>
        /// Gets the target of the binding
        /// </summary>
        public object Target
        {
            get
            {
                return this.target.Target;
            }
        }

        /// <summary>
        /// Gets the name of the target property.
        /// </summary>
        public string TargetProperty { get; private set; }
        
        public string TargetEventName { get; private set; }

        /// <summary>
        /// Gets the source of the binding.
        /// </summary>
        public ICommand Command
        {
            get
            {
                return this.command.Target as ICommand;
            }
        }

        /// <summary>
        /// Gets or sets the property accessor for setting property values
        /// </summary>
        public IPropertyAccessor PropertyAccessor { get; set; }

        public bool SafelyUpdateTarget { get; set; }

        /// <summary>
        /// Sets up the binding and updates target
        /// </summary>
        public void Bind()
        {
            var t = this.Target;
            var s = this.Command;

            this.RegisterForCommandNotifications(s);
            this.RegisterForPropertyChangesOnTarget(t);

            this.UpdateTarget();
        }

        /// <summary>
        /// Updates the target object from the command.
        /// </summary>
        public void UpdateTarget()
        {
            var cmd = this.Command;
            var obj = this.Target;
            if (obj != null && cmd != null)
            {
                var canExecute = cmd.GetCanExecute();

                var doUpdate = true;
                if (this.SafelyUpdateTarget)
                {
                    var targetValue = (bool)this.PropertyAccessor.GetValue(obj);

                    doUpdate = targetValue != canExecute;
                }

                if (doUpdate)
                {
                    this.PropertyAccessor.SetValue(obj, canExecute);
                }
            }
        }

        /// <summary>
        /// Releases all resource used by the <see cref="Mobile.Mvvm.DataBinding.WeakCommandBinding"/> object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        /// <summary>
        /// </summary>
        public void ExecuteCommand()
        {
            var cmd = this.Command;
            if (cmd != null)
            {
                cmd.Execute();
            }
        }

        /// <summary>
        /// Disposes the resources used by this object.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.UnregisterForNotifications();
                }

                disposed = true;
            }
        }

        /// <summary>
        /// Unregisters for changes in the source and target objects.
        /// </summary>
        private void UnregisterForNotifications()
        {
            var cmd = this.Command;
            if (cmd != null)
            {
                cmd.CanExecuteChanged -= HandleCommandNotifications;
            }

            this.subscriptions.Dispose();
        }

        /// <summary>
        /// Registers for property changes on the source object.  The source object must implement INotifyPropertyChanged.
        /// </summary>
        protected virtual void RegisterForCommandNotifications(object obj)
        {
            var cmd = this.Command;
            if (cmd != null)
            {
                cmd.CanExecuteChanged -= HandleCommandNotifications;
                cmd.CanExecuteChanged += HandleCommandNotifications;
            }
        }

        /// <summary>
        /// Registers for events firing on target and call execute on the command
        /// </summary>
        private void RegisterForPropertyChangesOnTarget(object obj)
        {
            var t = (TTargetType)obj;

            // look for event name
            var addMethod = default(MethodInfo);
            var removeMethod = default(MethodInfo);
            var delegateType = default(Type);
            var isWinRT = default(bool);
            ReflectionUtils.GetEventMethods(t.GetType(), this.TargetEventName, out addMethod, out removeMethod, out delegateType, out isWinRT);

            var eventWrapper = new WeakEventWrapper<ICommandBinding, EventArgs>(this, (t1, s1, e1) => {
                // update who we are bound to
                t1.ExecuteCommand();
            });

            var d = ReflectionUtils.CreateDelegate(delegateType, eventWrapper,  eventWrapper.GetType().GetMethod("HandleEvent"));
            addMethod.Invoke(t, new object[] { d });

            var unSubscribe = new AnonymousDisposable(() => {
                removeMethod.Invoke(t, new object[] { d });
            });

            this.subscriptions.Add(eventWrapper);
            this.subscriptions.Add(unSubscribe);
        }

        /// <summary>
        /// Handles changes in the source object.
        /// </summary>
        private void HandleCommandNotifications (object sender, EventArgs e)
        {
            this.UpdateTarget();
        }
    }
}

