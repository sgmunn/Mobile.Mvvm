//  --------------------------------------------------------------------------------------------------------------------
//  <copyright file="BindingScope.cs" company="sgmunn">
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

namespace Mobile.Mvvm.DataBinding
{
    using System;
    using System.Collections.Generic;

    public sealed class BindingScope : IBindingScope
    {
        private readonly List<IBindingExpression> exresssions;

        public BindingScope()
        {
            this.exresssions = new List<IBindingExpression>();
        }

        /// <summary>
        /// Adds the binding expression and binds the expression.
        /// </summary>
        public void AddBinding(IBindingExpression expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            expression.Bind();
            this.exresssions.Add(expression);
        }        
        
        /// <summary>
        /// Adds the binding expressions and binds the expressions.
        /// </summary>
        public void AddBinding(IBindingExpression[] expressions)
        {
            foreach (var exp in expressions)
            {
                exp.Bind();
                this.exresssions.Add(exp);
            }
        }        

        public void RemoveBinding(IBindingExpression expression)
        {
            expression.Dispose();
            this.exresssions.Remove(expression);
        }        

        public void ClearBindings()
        {
            foreach (var expression in exresssions)
            {
                expression.Dispose();
            }

            this.exresssions.Clear();
        }        

        public IBindingExpression[] GetBindingExpressions()
        {
            return this.exresssions.ToArray();
        }        

        public void Dispose()
        {
            this.ClearBindings();
        }       
    }
}
