//  --------------------------------------------------------------------------------------------------------------------
//  <copyright file="BindingScopeExtensions.cs" company="sgmunn">
//    (c) sgmunn 2012  
//
//    Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//    documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
//    the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
//    to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
//    The above copyright notice and this permission notice shall be included in all copies or substantial portions of 
//    the Software.
//
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO 
//    THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
//    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
//    CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS 
//    IN THE SOFTWARE.
//  </copyright>
//  --------------------------------------------------------------------------------------------------------------------

namespace Mobile.Mvvm
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Mobile.Mvvm.DataBinding;

    public static class BindingScopeExtensions
    {
        /// <summary>
        /// Sets a binding between target and source.
        /// </summary>
        public static IBindingExpression AddBinding(this IBindingScope scope, object target, object source, string propertyName)
        {
            return scope.AddBinding(target, propertyName, source, new Binding(propertyName));
        }
        
        /// <summary>
        /// Sets the binding between target and source.
        /// </summary>
        public static IBindingExpression AddBinding(this IBindingScope scope, object target, string targetPropertyName, object source, string sourcePropertyName)
        {
            return scope.AddBinding(target, targetPropertyName, source, new Binding(sourcePropertyName));
        }
        
        /// <summary>
        /// Sets the binding between target and source.
        /// </summary>
        public static IBindingExpression AddBinding(this IBindingScope scope, object target, object source, Binding binding)
        {
            return scope.AddBinding(target, binding.PropertyName, source, binding);
        }
        
        /// <summary>
        /// Sets the binding between target and source.
        /// </summary>
        public static IBindingExpression AddBinding(this IBindingScope scope, object target, string propertyName, IPropertyAccessor propertyAccessor, object source, Binding binding)
        {
            var expression = new WeakBindingExpression(target, propertyName, propertyAccessor, source, binding);
            expression.Bind();
            scope.AddBinding(expression);
            return expression;
        }
        
        /// <summary>
        /// Sets the binding between target and source.
        /// </summary>
        public static IBindingExpression[] AddBindings(this IBindingScope scope, object target, object source, string bindingExpression)
        {
            var expressions = BindingParser.Default.Parse(bindingExpression, target, source);
            scope.AddBinding(expressions);
            return expressions;
        }

        /// <summary>
        /// Sets the binding between target and source.
        /// </summary>
        public static IBindingExpression AddBinding(this IBindingScope scope, object target, string propertyName, object source, Binding binding)
        {
            var expression = new WeakBindingExpression(target, propertyName, source, binding);
            expression.Bind();
            scope.AddBinding(expression);
            return expression;
        }
        
        /// <summary>
        /// Replaces the source object that is bound to target.
        /// </summary>
        public static void ReplaceBindingSource(this IBindingScope scope, object source)
        {
            scope.ReplaceBindingSource(source, string.Empty);
        }
        
        /// <summary>
        /// Replaces the source object that is bound to target.
        /// </summary>
        public static void ReplaceBindingSource(this IBindingScope scope, object source, string targetPropertyName)
        {
            var allBindingExpressions = scope.GetBindingExpressions();
            var bindingExpressions = allBindingExpressions.Where(x => string.IsNullOrEmpty(targetPropertyName) || x.TargetProperty.Equals(targetPropertyName)).ToList();
            
            var newExpressions = new List<IBindingExpression>();
            
            foreach (var expression in bindingExpressions)
            {
                var newExpression = new WeakBindingExpression(expression.Target, expression.TargetProperty, source, expression.Binding);
                newExpression.Bind();
                newExpressions.Add(newExpression);
            }

            // don't remove all bindings, just the ones we replaced
            foreach (var expression in bindingExpressions)
            {
                scope.RemoveBinding(expression);
            }

            foreach (var expression in newExpressions)
            {
                scope.AddBinding(expression);
            }
        }
        
        /// <summary>
        /// Clears the bindings for the given target
        /// </summary>
        public static void ClearBindings(this IBindingScope scope, object target)
        {
            var bindingExpressions = scope.GetBindingExpressions(target, string.Empty);

            foreach (var expression in bindingExpressions)
            {
                scope.RemoveBinding(expression);
            }
        }

        /// <summary>
        /// Gets the binding expression for a target
        /// </summary>
        public static IBindingExpression[] GetBindingExpressions(this IBindingScope scope, object target, string propertyName)
        {
            var bindingExpressions = scope.GetBindingExpressions();
            
            return bindingExpressions.Where(bx => bx.Target == target && (string.IsNullOrEmpty(propertyName) || bx.TargetProperty.Equals(propertyName))).ToArray();
        } 
        
        /// <summary>
        /// Updates any source object that is bound to the target's property
        /// </summary>
        public static void UpdateBindingSource(this IBindingScope scope, object target, string propertyName)
        {
            var bx = scope.GetBindingExpressions(target, propertyName);
            foreach (var expression in bx)
            {
                expression.UpdateSource();
            }
        }

        public static IBindingExpression AddEventTriggeredBinding(this IBindingScope scope, object target, string targetPropertyName, string eventName, object source, string sourcePropertyName)
        {
            var expression = new EventTriggeredBindingExpression(target, targetPropertyName, eventName, source, new Binding(sourcePropertyName));
            expression.Bind();
            scope.AddBinding(expression);
            return expression;
        }
    }
}
