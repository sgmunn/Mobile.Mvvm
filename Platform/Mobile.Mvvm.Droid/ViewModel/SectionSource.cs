// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SectionSource.cs" company="sgmunn">
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
    using Mobile.Mvvm.DataBinding;

    public class SectionSource : BaseAdapter<ISection>, ISectionSource, IBindingContext
    {
        private readonly List<ISection> sections;

        private readonly SectionSynchroniser sync;

        private readonly Context context;
        
        private readonly object locker = new object();

        private ListView listView;

        public SectionSource(Context context)
        {
            this.context = context;
            this.sections = new List<ISection>();
            this.sync = new SectionSynchroniser(this);
            this.Bindings = new BindingScope();
            this.InjectedProperties = new InjectionScope();
        }
        
        public ListView ListView
        {
            get 
            { 
                return this.listView; 
            }

            set 
            { 
                if (value != this.listView)
                {
                    if (this.listView != null)
                    {
                        // clear source / adapter etc
                        this.UnregisterListView(this.listView);
                        //this.listView.Source = null;
                    }

                    this.listView = value; 

                    if (this.listView != null)
                    {
                        this.RegisterListView(this.listView);
                        // reload / set adapter etc

                       // this.listView.Source = this;
                    }
                }
            }
        }

        public IBindingScope Bindings { get; private set; }

        public IInjectionScope InjectedProperties { get; private set; }

        public void Bind(IList<ISection> source)
        {
            this.sync.Bind(source);
        }
        
        public void Unbind()
        {
            this.sync.Unbind();
        }

        public void Clear()
        {
            this.Bindings.ClearBindings();
            this.InjectedProperties.Clear();
            this.sections.Clear();
            this.ReloadView();
        }

        public void Load(IList<ISection> sourceList)
        {
            this.Bindings.ClearBindings();
            this.InjectedProperties.Clear();
            this.sections.Clear();
            if (sourceList != null)
            {
                this.sections.AddRange(sourceList);
            }

            this.ReloadView();
        }

        public void Insert(int index, IList<ISection> newSections)
        {
            this.sections.InsertRange(index, newSections);
            // lazy, just reload
            this.ReloadView();
        }

        public void Remove(int index, int count)
        {
            this.sections.RemoveRange(index, count);
            // lazy, just reload
            this.ReloadView();
        }

        public void Insert(ISection section, int index, IList<IViewModel> rows)
        {
//            // just update the table view
//            var sectionIndex = this.sections.IndexOf(section);
//            var paths = new NSIndexPath[rows.Count];
//            for (int i = 0; i < rows.Count; i++)
//            {
//                paths[i] = NSIndexPath.FromRowSection(index + i, sectionIndex);
//            }
//
//            this.TableView.InsertRows(paths, this.AddAnimation);
        }

        public void Remove(ISection section, int index, int count)
        {
//            // just update the table view
//            var sectionIndex = this.sections.IndexOf(section);
//            var paths = new NSIndexPath[count];
//            for (int i = 0; i < count; i++)
//            {
//                paths [i] = NSIndexPath.FromRowSection(index + i, sectionIndex);
//            }
//
//            this.TableView.DeleteRows(paths, this.RemoveAnimation);
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
                return this.sections.Sum(x => x.Rows.Count);
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
                return this.sections[0].Rows[0];
            }

            throw new NotImplementedException();

        }
        
        public override bool IsEnabled(int position)
        {
            return true;
            //var element = ElementAtIndex(position);
            //return !(element is Section) && element != null;
        }

        public override bool AreAllItemsEnabled()
        {
            return false;
        }

        protected override void Dispose(bool disposing)
        {
            //DeregisterListView();
            this.Bindings.Dispose();
            this.InjectedProperties.Dispose();
            base.Dispose(disposing);
        }

        protected virtual void ReloadView()
        {
            ((Activity)this.context).RunOnUiThread(() => {
                this.NotifyDataSetChanged();
            });
        }

        protected virtual void HandleListViewItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
        }

        protected virtual void HandleListViewItemLongClick(object sender, AdapterView.ItemLongClickEventArgs e)
        {
        }

        private void RegisterListView(ListView list)
        {
            if (list == null)
            {
                throw new ArgumentNullException("list");
            }

            lock (this.locker)
            {
                list.ItemClick -= this.HandleListViewItemClick;
                list.ItemClick += this.HandleListViewItemClick;

                list.ItemLongClick -= this.HandleListViewItemLongClick;
                list.ItemLongClick += this.HandleListViewItemLongClick;
            }
        }

        private void UnregisterListView(ListView list)
        {
            if (list == null)
            {
                throw new ArgumentNullException("list");
            }

            lock (this.locker)
            {
                list.ItemClick -= this.HandleListViewItemClick;
                list.ItemLongClick -= this.HandleListViewItemLongClick;
            }
        }

    }
}
