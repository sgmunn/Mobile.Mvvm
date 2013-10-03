// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SectionSynchroniser.cs" company="sgmunn">
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
using System.Linq;

namespace Mobile.Mvvm.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;

    /// <summary>
    /// Helper class to keep a collection of ISections in sync with a ISectionSource
    /// </summary>
    public sealed class SectionSynchroniser
    {
        private readonly ISectionSource targetSource;

        private IList<ISection> sourceList;

        public SectionSynchroniser(ISectionSource source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            this.targetSource = source;
            /*
             * we want to bind to a collection of sections
             * we will watch that collection for changes and add / remove items to our internal list
             * we will provide virtual methods to override to be able to alter the add / remove behaviour
             * each section must also provide a notification of items within itself being added or removed
             * if we fail to provide either notification, then the table becomes a static list and we won't
             * dynamically add or remove items
             */ 
        }

        public void Bind(IList<ISection> sourceList)
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
                notifyingCollection.CollectionChanged -= this.HandleSectionsChanged;
                notifyingCollection.CollectionChanged += this.HandleSectionsChanged;
            }

            foreach (var section in this.sourceList)
            {
                this.RegisterSection(section);
            }

            this.targetSource.Load(sourceList);
        }

        public void UnBind()
        {
            if (this.sourceList != null)
            {
                var notifyingCollection = this.sourceList as INotifyCollectionChanged;
                if (notifyingCollection != null)
                {
                    notifyingCollection.CollectionChanged -= this.HandleSectionsChanged;
                }

                foreach (var section in this.sourceList)
                {
                    this.UnregisterSection(section);
                }
            }

            this.sourceList = null;
        }

        public void Clear()
        {
            this.UnBind();
            this.targetSource.Clear();
        }

        private void RegisterSection(ISection section)
        {
            var notifyingCollection = section.Rows as INotifyCollectionChanged;
            if (notifyingCollection != null)
            {
                notifyingCollection.CollectionChanged -= this.HandleSectionRowsChanged;
                notifyingCollection.CollectionChanged += this.HandleSectionRowsChanged;
            }
        }

        private void UnregisterSection(ISection section)
        {
            var notifyingCollection = section.Rows as INotifyCollectionChanged;
            if (notifyingCollection != null)
            {
                notifyingCollection.CollectionChanged -= this.HandleSectionRowsChanged;
            }
        }

        private void HandleSectionsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action) 
            {
                case NotifyCollectionChangedAction.Add:
                    this.targetSource.Insert(e.NewStartingIndex, e.NewItems.OfType<ISection>().ToList());
                    break;
                    case NotifyCollectionChangedAction.Remove:
                    this.targetSource.Remove(e.OldStartingIndex, e.OldItems.Count);
                    break;
                    case NotifyCollectionChangedAction.Reset:
                    // clear current list, unregister all sections, unbind all view models
                    // iterate over all items, register each section and full reload (binding as we go)
                    //this.ReloadSection(sectionIndex);
                    this.targetSource.Load(this.sourceList);
                    break;
                    default:
                    break;
            }           
        }

        private void HandleSectionRowsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var section = this.FindSectionFromRows(sender);
            if (section == null)
            {
                return;
            }

            switch (e.Action) 
            {
                case NotifyCollectionChangedAction.Add:
                    this.targetSource.Insert(section, e.NewStartingIndex, e.NewItems.OfType<IViewModel>().ToList());
                    break;
                    case NotifyCollectionChangedAction.Remove:
                    this.targetSource.Remove(section, e.OldStartingIndex, e.OldItems.Count);
                    break;
                    case NotifyCollectionChangedAction.Reset:
                    this.targetSource.Load(this.sourceList);
                    break;
                    default:
                    break;
            }           
        }

        private ISection FindSectionFromRows(object rows)
        {
            return this.sourceList.FirstOrDefault(x => x.Rows == rows);
        }
    }
}
