// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WeakBindingExpression.cs" company="sgmunn">
//   (c) sgmunn 2012  
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
    using System.ComponentModel;

    /// <summary>
    /// An instance of a Binding between two objects that uses weak references between target and source
    /// </summary>
    public class WeakBindingExpression : IBindingExpression
    {
        /// <summary>
        /// The target object that is bound
        /// </summary>
        private WeakReference target;
        
        /// <summary>
        /// The source object that is bound.
        /// </summary>
        private WeakReference source;
  
        /// <summary>
        /// Indicates that the expression has been disposed.
        /// </summary>
        private bool disposed = false;
        
        /// <summary>
        /// The property type of the target's property.
        /// </summary>
        private Type targetPropertyType;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Mobile.Mvvm.DataBinding.WeakBindingExpression"/> class.
        /// </summary>
        public WeakBindingExpression(object target, string targetProperty, object source, Binding binding)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            if (string.IsNullOrEmpty(targetProperty))
            {
                throw new ArgumentNullException("targetProperty");
            }

            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (binding == null)
            {
                throw new ArgumentNullException("binding");
            }

            this.SafelyUpdateTarget = true;
            this.PropertyAccessor = new ReflectionPropertyAccessor(targetProperty);
            this.Initialize(target, targetProperty, source, binding);
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Mobile.Mvvm.DataBinding.WeakBindingExpression"/> class.
        /// </summary>
        public WeakBindingExpression(object target, string targetProperty, IPropertyAccessor accessor, object source, Binding binding)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }
            
            if (string.IsNullOrEmpty(targetProperty))
            {
                throw new ArgumentNullException("targetProperty");
            }
            
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            
            if (accessor == null)
            {
                throw new ArgumentNullException("accessor");
            }

            if (binding == null)
            {
                throw new ArgumentNullException("binding");
            }
            
            this.SafelyUpdateTarget = true;
            this.PropertyAccessor = accessor;
            this.Initialize(target, targetProperty, source, binding);
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="Mobile.Mvvm.DataBinding.WeakBindingExpression"/> is reclaimed by garbage collection.
        /// </summary>
        ~WeakBindingExpression()
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

        /// <summary>
        /// Gets the source of the binding.
        /// </summary>
        public object Source
        {
            get
            {
                return this.source.Target;
            }
        }
        
        /// <summary>
        /// Gets the binding for Source.
        /// </summary>
        public Binding Binding { get; private set; }

        /// <summary>
        /// Gets or sets the property accessor for getting and setting property values
        /// </summary>
        public IPropertyAccessor PropertyAccessor { get; set; }

        public bool SafelyUpdateTarget { get; set; }

        /// <summary>
        /// Sets up the binding and updates target
        /// </summary>
        public IBindingExpression Bind()
        {
            var t = this.Target;
            var s = this.Source;

            this.RegisterForPropertyChangesOnSource(s);
            this.RegisterForPropertyChangesOnTarget(t);

            this.UpdateTarget(s);
            return this;
        }
        
        /// <summary>
        /// Updates the target object from the source object.
        /// </summary>
        public void UpdateTarget(object sourceObject)
        {
            if (this.Binding.Mode == BindingMode.OneWayToSource)
            {
                return;
            }
            
            var obj = this.Target;
            if (obj != null)
            {
                var sourceValue = this.Binding.GetSourceValue(sourceObject, this.targetPropertyType);

                var doUpdate = true;
                if (this.SafelyUpdateTarget)
                {
                    var targetValue = this.PropertyAccessor.GetValue(obj);
                    if (targetValue == null && sourceValue == null)
                    {
                        doUpdate = false;
                    }

                    if (targetValue != null && sourceValue != null)
                    {
                        doUpdate = !targetValue.Equals(sourceValue);
                    }
                }

                if (doUpdate)
                {
                    this.PropertyAccessor.SetValue(obj, sourceValue);
                }
            }
        }
        
        /// <summary>
        /// Updates the source object from the Target object.
        /// </summary>
        public void UpdateSource(object targetObject)
        {
            if (this.Binding.Mode == BindingMode.OneWay)
            {
                return;
            }
            
            System.Diagnostics.Debug.WriteLine("update source");
            this.Binding.UpdateSourceValue(this.Source, this.PropertyAccessor.GetValue(targetObject));
        }
        
        /// <summary>
        /// Updates the source object from the Target object.
        /// </summary>
        public void UpdateSource()
        {
            if (this.Binding.Mode == BindingMode.OneWay)
            {
                return;
            }
            
            var obj = this.Target;
            if (obj != null)
            {
                this.Binding.UpdateSourceValue(this.Source, this.PropertyAccessor.GetValue(obj));
            }            
        }

        /// <summary>
        /// Releases all resource used by the <see cref="Mobile.Mvvm.DataBinding.BindingExpression"/> object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
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
                    this.Binding.Dispose();
                    this.UnregisterForChanges();
                }
                
                disposed = true;
            }
        }
        
        /// <summary>
        /// Unregisters for changes in the source and target objects.
        /// </summary>
        protected virtual void UnregisterForChanges()
        {
            var s = this.Source;
            var inpc = s as INotifyPropertyChanged;
            if (inpc != null)
            {
                inpc.PropertyChanged -= HandleSourcePropertyChanged;
            }

            var t = this.Target;
            inpc = t as INotifyPropertyChanged;
            if (inpc != null)
            {
                inpc.PropertyChanged -= HandleTargetPropertyChanged;
            }
        }
        
        /// <summary>
        /// Initialize the binding instance.
        /// </summary>
        private void Initialize(object target, string targetProperty, object source, Binding binding)
        {
            this.target = new WeakReference(target);
            this.source = new WeakReference(source);
            this.TargetProperty = targetProperty;
            this.Binding = binding;

            this.targetPropertyType = target.GetPropertyInfo(this.TargetProperty).PropertyType;
        }
  
        /// <summary>
        /// Registers for property changes on the source object.  The source object must implement INotifyPropertyChanged.
        /// </summary>
        protected virtual void RegisterForPropertyChangesOnSource(object obj)
        {
            var inpc = obj as INotifyPropertyChanged;
            
            if (inpc != null)
            {
                inpc.PropertyChanged -= HandleSourcePropertyChanged;
                inpc.PropertyChanged += HandleSourcePropertyChanged;
            }
        }
        
        /// <summary>
        /// Registers for property changes on the target object.  The target object must implement INotifyPropertyChanged.
        /// </summary>
        protected virtual void RegisterForPropertyChangesOnTarget(object obj)
        {
            var inpc = obj as INotifyPropertyChanged;
            if (inpc != null)
            {
                inpc.PropertyChanged -= HandleTargetPropertyChanged;
                inpc.PropertyChanged += HandleTargetPropertyChanged;
            }
        }
  
        /// <summary>
        /// Handles changes in the source object.
        /// </summary>
        private void HandleSourcePropertyChanged (object sender, PropertyChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.PropertyName) || e.PropertyName.Equals(this.Binding.PropertyName))
            {
                this.UpdateTarget(sender);
            }
        }

        /// <summary>
        /// Handles changes in the target object.
        /// </summary>
        private void HandleTargetPropertyChanged (object sender, PropertyChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.PropertyName) || e.PropertyName.Equals(this.TargetProperty))
            {
                this.UpdateSource(sender);
            }
        }
    }
}

