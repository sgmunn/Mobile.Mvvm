// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GroupedListSource.cs" company="sgmunn">
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

namespace Mobile.Mvvm.ViewModel.Dialog
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Android.App;
    using Android.Content;
    using Android.Widget;
    using Android.Views;
    using Android.Views.Animations;
    using Mobile.Mvvm.DataBinding;

    public class GroupedListSource : BaseAdapter<IGroup>, IGroupSource, IBindingContext
    {
        private readonly List<IGroup> groups;

        private readonly GroupSourceSynchroniser sync;

        private readonly Context context;

        private readonly object locker = new object();

        private readonly List<IDataTemplate> templates;

        private ListView listView;

        public GroupedListSource(Context context) : this(context, Enumerable.Empty<IDataTemplate>())
        {
        }

        public GroupedListSource(Context context, IEnumerable<IDataTemplate> templates)
        {
            this.templates = templates.ToList();
            this.context = context;
            this.groups = new List<IGroup>();
            this.sync = new GroupSourceSynchroniser(this);
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
                        this.listView.Adapter = null;
                    }

                    this.listView = value; 

                    if (this.listView != null)
                    {
                        this.RegisterListView(this.listView);
                        this.listView.Adapter = this;
                        this.ReloadView();
                    }
                }
            }
        }

        public IBindingScope Bindings { get; private set; }

        public IInjectionScope InjectedProperties { get; private set; }

        public void Bind(IList<IGroup> source)
        {
            this.sync.Bind(source);
        }

        public void Unbind()
        {
            this.sync.Unbind();
        }

        public void Clear()
        {
            this.Bindings.Clear();
            this.InjectedProperties.Clear();
            this.groups.Clear();
            this.ReloadView();
        }

        public void Load(IList<IGroup> sourceList)
        {
            this.Bindings.Clear();
            this.InjectedProperties.Clear();
            this.groups.Clear();
            if (sourceList != null)
            {
                this.groups.AddRange(sourceList);
            }

            this.ReloadView();
        }

        public void Insert(int index, IList<IGroup> newGroups)
        {
            this.groups.InsertRange(index, newGroups);
            // lazy, just reload
            this.ReloadView();
        }

        public void Remove(int index, int count)
        {
            this.groups.RemoveRange(index, count);
            // lazy, just reload
            this.ReloadView();
        }

        public virtual void Insert(IGroup group, int index, IList<IViewModel> rows)
        {
            //Animation anim = AnimationUtils.LoadAnimation(this.context, Android.Resource.Animation.FadeIn);

            //anim.Duration = 500;
            //this.ListView.GetChildAt(index+1).StartAnimation(anim);

            //anim.AnimationStart += (object sender, Animation.AnimationStartEventArgs e) => {
            this.NotifyDataSetChanged();
            //};
        }

        public virtual void Remove(IGroup group, int index, int count)
        {
            this.NotifyDataSetChanged();
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var row = this.ViewModelForPosition(position);
            var view = (View)row.GetTemplate(this.templates).GetViewForViewModel(this, row, (id) => convertView, parent);

            // fallback default view
            if (view == null)
            {
                var inflator = LayoutInflater.FromContext(this.context);
                view = convertView;
                if (view == null)
                {
                    view = inflator.Inflate(Android.Resource.Layout.SimpleListItem1, parent, false);
                }

                view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = row.ToString();
            }

            return view;
        }

        public override int Count
        {
            get
            {
                return this.groups.Sum(x => x.ViewModelCount());
            }
        }

        public override int GetItemViewType(int position)
        {
            if (this.templates.Count > 0)
            {
                var row = this.ViewModelForPosition(position);
                var template = row.GetTemplate(this.templates);
                return this.templates.IndexOf(template);
            }

            return base.GetItemViewType(position);
        }

        public override int ViewTypeCount
        {
            get
            {
                // naive implementation here
                // assume that the most number of templates that could be returned are in the list of templates
                // that we were told about
                if (this.templates.Count > 0)
                {
                    return this.templates.Count;
                }

                return base.ViewTypeCount;
            }
        }

        public override IGroup this[int index]
        {
            get
            {
                return this.groups[index];
            }
        }

        public IViewModel ViewModelForPosition(int position)
        {
            bool isHeaderOrFooter;
            return this.ViewModelForPosition(position, out isHeaderOrFooter);
        }

        public override bool IsEnabled(int position)
        {
            var row = this.ViewModelForPosition(position);
            var cmd = row as ICommand;
            if (cmd != null)
            {
                return cmd.GetCanExecute();
            }

            // items are enabled by default, unless they are a header or footer
            bool isHeaderOrFooter;
            this.ViewModelForPosition(position, out isHeaderOrFooter);
            return !isHeaderOrFooter;
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
            var row = this.ViewModelForPosition(e.Position);
            row.ExecuteTapCommand();
        }

        protected virtual void HandleListViewItemLongClick(object sender, AdapterView.ItemLongClickEventArgs e)
        {
            var row = this.ViewModelForPosition(e.Position);
            row.ExecuteLongTapCommand();
        }

        private IViewModel ViewModelForPosition(int position, out bool isHeaderOrFooter)
        {
            if (position == 0)
            {
                isHeaderOrFooter = this.groups[0].IsHeaderOrFooterAtIndex(0);
                return this.groups[0].ViewModelAtIndex(0);
            }

            int count = 0;
            foreach (var group in this.groups)
            {
                if (position < count + group.ViewModelCount())
                {
                    // it's here somewhere
                    isHeaderOrFooter = group.IsHeaderOrFooterAtIndex(position - count);
                    return group.ViewModelAtIndex(position - count);
                }
                else
                {
                    count += group.ViewModelCount();
                }
            }

            throw new ArgumentOutOfRangeException("position");
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
