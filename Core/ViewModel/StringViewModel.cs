// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringViewModel.cs" company="sgmunn">
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

namespace Mobile.Mvvm.ViewModel
{
    using System;

    // value vm, of T
    // individual classes of T
    // caption and value (ala elements
    // need a way to add in with RowVM and ICommand
    // perhaps drop RowVM and we need a property of type ICommand that is on a VM (instead of a VM implementing ICommand)
    // this will impact both sectionSources.

    // ICommand and IxxCommand - perhaps even ITapCommand / ILongTapCommand -- adds TapCommand and LongTapCommand


    public interface ITapCommand
    {
        ICommand TapCommand { get; }
    }
    
    public interface ILongTapCommand
    {
        ICommand LongTapCommand { get; }
    }

    public static class ViewModelExtensions
    {
        public static void ExecuteTapCommand(this IViewModel viewModel)
        {
            var cmd = viewModel as ITapCommand;
            if (cmd != null)
            {
                cmd.TapCommand.Execute();
            }
        }

        public static void ExecuteLongTapCommand(this IViewModel viewModel)
        {
            var cmd = viewModel as ILongTapCommand;
            if (cmd != null)
            {
                cmd.LongTapCommand.Execute();
            }
        }
    }


    public class StringViewModel : ViewModelBase
    {
        public StringViewModel(string caption)
        {
            this.Caption = caption;
        }

        public string Caption
        {
            get
            {
                return (string)this.GetPropertyValue("Caption");
            }

            set
            {
                this.SetPropertyValue("Caption", value);
            }
        }

        public override string ToString()
        {
            return this.Caption;
        }
    }
}

