// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CaptionedValueViewModel_T.cs" company="sgmunn">
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
using System.Threading.Tasks;

namespace Mobile.Mvvm.ViewModel.Dialog
{
    using System;

    public interface ICaptionedElementValue<T> 
    {
        string Caption { get; set; }
        T Value { get; set; }
    }


    

    public class CaptionedValueViewModel<T> : CaptionViewModel, ICaptionedElementValue<T>, ITapCommand
    {
        public CaptionedValueViewModel(string caption) : base(caption)
        {
        }

        public CaptionedValueViewModel(string caption, T value) : base(caption)
        {
            this.Value = value;
        }

        public T Value
        {
            get
            {
                return (T)this.GetPropertyValue("Value");
            }

            set
            {
                this.SetPropertyValue("Value", value);
            }
        }

        // TODO: INPC if command is bound
        public ICommand TapCommand { get; set; }

        public override string ToString()
        {
            return this.Caption;
        }
    }
}
