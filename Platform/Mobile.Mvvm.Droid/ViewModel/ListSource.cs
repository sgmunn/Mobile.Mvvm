// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ListSource.cs" company="sgmunn">
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
    using System.Linq;
    using Android.Content;
    using Android.Widget;
    using Android.Views;

    public class ListSource<T> : BaseAdapter<ISection>
        where T : ISection
    {
        private readonly List<T> sections;

        private readonly Context context;

        public ListSource(Context context) : base()
        {
            this.context = context;
            this.sections = new List<T>();
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var row = this.RowForPosition(position);

            throw new NotImplementedException();
        }

        public override int Count
        {
            get
            {
                return this.sections.Sum(x => x.Rows);
            }
        }

        public override ISection this[int index]
        {
            get
            {
                return this.sections[index];
            }
        }

        public IViewModel RowForPosition(int position)
        {
            if (position == 0)
            {
                return this.sections[0].Row(0);
            }

            throw new NotImplementedException();

        }
    }
    
}
