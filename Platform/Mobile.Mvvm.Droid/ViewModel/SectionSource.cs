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
    using Android.App;
    using Android.Content;
    using Android.Widget;
    using Android.Views;
    using Android.Views.Animations;
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

        public virtual void Insert(ISection section, int index, IList<IViewModel> rows)
        {
            //Animation anim = AnimationUtils.LoadAnimation(this.context, Android.Resource.Animation.FadeIn);

            //anim.Duration = 500;
            //this.ListView.GetChildAt(index+1).StartAnimation(anim);

            //anim.AnimationStart += (object sender, Animation.AnimationStartEventArgs e) => {
                this.NotifyDataSetChanged();
            //};
        }

        public virtual void Remove(ISection section, int index, int count)
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

            var inflator = ((Activity)this.context).LayoutInflater;
            var view = convertView;
            if (view == null)
            {
                view = inflator.Inflate(Android.Resource.Layout.SimpleListItem1, null);
            }

            view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = row.ToString();

            return view;
        }

        public override int Count
        {
            get
            {
                return this.sections.Sum(x => x.ViewModelCount());
            }
        }

        public override ISection this[int index]
        {
            get
            {
                return this.sections[index];
            }
        }

        public IViewModel ViewModelForPosition(int position)
        {
            if (position == 0)
            {
                return this.sections[0].ViewModelAtIndex(0);
            }

            int count = 0;
            foreach (var section in this.sections)
            {
                if (position < count + section.ViewModelCount())
                {
                    // it's here somewhere
                    return section.ViewModelAtIndex(position - count);
                }
                else
                {
                    count += section.ViewModelCount();
                }
            }

            throw new ArgumentOutOfRangeException("position");
        }
        
        public override bool IsEnabled(int position)
        {
            var row = this.ViewModelForPosition(position);
            var cmd = row as ICommand;
            if (cmd != null)
            {
                return cmd.GetCanExecute();
            }

            return true;
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
            var cmd = row as ICommand;
            if (cmd != null)
            {
                cmd.Execute();
                return;
            }
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
