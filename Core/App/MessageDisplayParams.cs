// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageDisplayParams.cs" company="sgmunn">
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

    public class MessageDisplayParams
    {
        public MessageDisplayParams(string title, string message)
        {
            this.Title = title;
            this.Message = message;
        }

        public string Title { get; set; }

        public string Message { get; set; }

        public Action PositiveAction { get; set; }

        public Action NegativeAction { get; set; }

        public Action NeutralAction { get; set; }

        public string PositiveLabel { get; set; }

        public string NegativeLabel { get; set; }

        public string NeutralLabel { get; set; }

        public MessageDisplayParams OnPositiveSelected(Action action)
        {
            this.PositiveAction = action;
            return this;
        }

        public MessageDisplayParams OnNegativeSelected(Action action)
        {
            this.NegativeAction = action;
            return this;
        }

        public MessageDisplayParams OnNeutralSelected(Action action)
        {
            this.NeutralAction = action;
            return this;
        }

        public MessageDisplayParams WithPositiveLabel(string label)
        {
            this.PositiveLabel = label;
            return this;
        }

        public MessageDisplayParams WithNegativeLabel(string label)
        {
            this.NegativeLabel = label;
            return this;
        }

        public MessageDisplayParams WithNeutralLabel(string label)
        {
            this.NeutralLabel = label;
            return this;
        }
    }
}
