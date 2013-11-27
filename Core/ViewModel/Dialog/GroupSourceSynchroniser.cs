// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GroupSourceSynchroniser.cs" company="sgmunn">
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
    using System.Collections.Specialized;
    using System.Linq;

    /// <summary>
    /// Helper class to keep a collection of IGroups in sync with a IGroupSource
    /// </summary>
    public sealed class GroupSourceSynchroniser
    {
        private readonly IGroupSource targetSource;

        private IList<IGroup> sourceList;

        public GroupSourceSynchroniser(IGroupSource source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            this.targetSource = source;
            /*
             * we want to bind to a collection of groups
             * we will watch that collection for changes and add / remove items to our internal list
             * we will provide virtual methods to override to be able to alter the add / remove behaviour
             * each group must also provide a notification of items within itself being added or removed
             * if we fail to provide either notification, then the table becomes a static list and we won't
             * dynamically add or remove items
             */ 
        }

        public void Bind(IList<IGroup> sourceList)
        {
            if (sourceList == null)
            {
                throw new ArgumentNullException("sourceList");
            }

            this.Clear();

            this.sourceList = sourceList;

            var notifyingCollection = this.sourceList as INotifyCollectionChanged;
            if (notifyingCollection != null)
            {
                notifyingCollection.CollectionChanged -= this.HandleGroupsChanged;
                notifyingCollection.CollectionChanged += this.HandleGroupsChanged;
            }

            foreach (var group in this.sourceList)
            {
                this.RegisterGroup(group);
            }

            this.targetSource.Load(sourceList);
        }

        public void Unbind()
        {
            if (this.sourceList != null)
            {
                var notifyingCollection = this.sourceList as INotifyCollectionChanged;
                if (notifyingCollection != null)
                {
                    notifyingCollection.CollectionChanged -= this.HandleGroupsChanged;
                }

                foreach (var group in this.sourceList)
                {
                    this.UnregisterGroup(group);
                }
            }

            this.sourceList = null;
            this.targetSource.Load(null);
        }

        public void Clear()
        {
            this.Unbind();
            this.targetSource.Clear();
        }

        private void RegisterGroup(IGroup group)
        {
            var notifyingCollection = group.Rows as INotifyCollectionChanged;
            if (notifyingCollection != null)
            {
                notifyingCollection.CollectionChanged -= this.HandleGroupRowsChanged;
                notifyingCollection.CollectionChanged += this.HandleGroupRowsChanged;
            }
        }

        private void UnregisterGroup(IGroup group)
        {
            var notifyingCollection = group.Rows as INotifyCollectionChanged;
            if (notifyingCollection != null)
            {
                notifyingCollection.CollectionChanged -= this.HandleGroupRowsChanged;
            }
        }

        private void HandleGroupsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action) 
            {
                case NotifyCollectionChangedAction.Add:
                    this.targetSource.Insert(e.NewStartingIndex, e.NewItems.OfType<IGroup>().ToList());
                    break;
                case NotifyCollectionChangedAction.Remove:
                    this.targetSource.Remove(e.OldStartingIndex, e.OldItems.Count);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    // clear current list, unregister all groups, unbind all view models
                    // iterate over all items, register each group and full reload (binding as we go)
                    //this.ReloadGroup(sectionIndex);
                    this.targetSource.Load(this.sourceList);
                    break;
                default:
                    break;
            }           
        }

        private void HandleGroupRowsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var group = this.FindGroupFromRows(sender);
            if (group == null)
            {
                return;
            }

            switch (e.Action) 
            {
                case NotifyCollectionChangedAction.Add:
                    this.targetSource.Insert(group, e.NewStartingIndex, e.NewItems.OfType<IViewModel>().ToList());
                    break;
                case NotifyCollectionChangedAction.Remove:
                    this.targetSource.Remove(group, e.OldStartingIndex, e.OldItems.Count);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    this.targetSource.Load(this.sourceList);
                    break;
                default:
                    break;
            }           
        }

        private IGroup FindGroupFromRows(object rows)
        {
            return this.sourceList.FirstOrDefault(x => x.Rows == rows);
        }
    }
}
