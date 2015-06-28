// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TableViewCell.cs" company="sgmunn">
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

namespace Mobile.Mvvm.Controls
{
    using System;
    using UIKit;

    /// <summary>
    /// Provides a wrapper class for UITableViewCell that can be used in a bound list.
    /// </summary>
    /// <remarks>
    /// We use this for binding to because using a UITableViewCell won't work if we use WeakBindingExpressions. Using BindingExpressions
    /// that have a strong reference to the cell will work. The reason for WeakBindingExpressions failing is to do with how the MonoTouch runtime
    /// frees up managed peers when they are no longer referenced. Derived classes are held onto as long as the underlying table holds a reference.
    /// </remarks>
    public class TableViewCell : UITableViewCell
    {
        private string text;

        public TableViewCell(string reuseIdentifer)
            : base(UITableViewCellStyle.Default, reuseIdentifer)
        {
        }

        public TableViewCell(UITableViewCellStyle style, string reuseIdentifer)
            : base(style, reuseIdentifer)
        {
        }

        ~TableViewCell()
        {
            ////Console.WriteLine("~TableViewCell {0}", this.GetType().Name);
        }

        public string Text
        {
            get
            {
                return this.text;
            }

            set
            {
                if (value != this.text)
                {
                    this.text = value;
                    this.TextChanged(value);
                }
            }
        }

        protected virtual void TextChanged(string newValue)
        {
            this.TextLabel.Text = newValue;
        }
    }
}

