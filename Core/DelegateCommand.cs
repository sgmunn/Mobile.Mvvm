// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DelegateCommand.cs" company="sgmunn">
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

namespace Mobile.Mvvm
{
    using System;

    public sealed class DelegateCommand : ICommand
    {
        private readonly Action command;
        private readonly Func<bool> canExecute;

        public DelegateCommand(Action command)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }

            this.command = command;
        }
        
        public DelegateCommand(Action command, Func<bool> canExecute)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }

            if (canExecute == null)
            {
                throw new ArgumentNullException("canExecute");
            }

            this.command = command;
            this.canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute()
        {
            this.command();
        }

        public bool GetCanExecute()
        {
            return (this.canExecute == null) || this.canExecute();
        }

        public void RaiseCanExecuteChanged()
        {
            var handler = this.CanExecuteChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}
