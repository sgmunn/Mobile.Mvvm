// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISection.cs" company="sgmunn">
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
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public interface ISection
    {
        IViewModel Header { get; }

        IViewModel Footer { get; }

        IList<IViewModel> Rows { get; }
    }

    public class RowViewModel : ViewModelBase
    {
        public RowViewModel()
        {
        }
    }

    public class SectionViewModel : ViewModelBase, ISection
    {
        private IViewModel header;
        private IViewModel footer;

        public SectionViewModel()
        {
            this.Rows = new ObservableCollection<IViewModel>();
        }

        public IViewModel Header 
        {
            get
            {
                return this.header;
            }

            set
            {
                if (value != this.header)
                {
                    this.header = value;
                    this.NotifyPropertyChanged("Header");
                }
            }
        }

        public IViewModel Footer
        {
            get
            {
                return this.footer;
            }

            set
            {
                if (value != this.footer)
                {
                    this.footer = value;
                    this.NotifyPropertyChanged("Footer");
                }
            }
        }

        public IList<IViewModel> Rows { get; private set; }
    }
}

