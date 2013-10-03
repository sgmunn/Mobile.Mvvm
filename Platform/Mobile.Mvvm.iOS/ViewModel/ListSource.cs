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
using System.Collections.Generic;
using System.Collections.Specialized;
using MonoTouch.Foundation;

namespace Mobile.Mvvm.ViewModel
{
    using System;
    using MonoTouch.UIKit;

    public interface ISectionSource
    {
        // clears all sections and unbinds the UI
        void Clear();

        // performs a clear and reload with the given source
        void Load(IList<ISection> sourceList);

        void Insert(int index, IList<ISection> sections);

        void Remove(int index, int count);
        
        void Insert(ISection section, int index, IList<IViewModel> rows);

        void Remove(ISection section, int index, int count);
    }


    /// <summary>
    /// Helper class to keep a collection of ISections in sync with a ISectionSource
    /// </summary>
    public sealed class SectionSynchroniser
    {
        private readonly ISectionSource targetSource;

        private IList<ISection> sourceList;

        public SectionSynchroniser(ISectionSource source)
        {
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
            var notifyingCollection = this.sourceList as INotifyCollectionChanged;
            if (notifyingCollection != null)
            {
                notifyingCollection.CollectionChanged -= this.HandleSectionsChanged;
            }

            foreach (var section in this.sourceList)
            {
                this.UnregisterSection(section);
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
                    this.targetSource.Insert(e.NewStartingIndex, (IList<ISection>)e.NewItems);
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
            var section = (ISection)sender;
            switch (e.Action) 
            {
                case NotifyCollectionChangedAction.Add:
                    this.targetSource.Insert(section, e.NewStartingIndex, (IList<IViewModel>)e.NewItems);
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
    }






    public class SectionSource: UITableViewSource, ISectionSource
    {
        private readonly List<ISection> sections;

        private readonly SectionSynchroniser sync;

        private UITableView tableView;

        public SectionSource()
        {
            this.sections = new List<ISection>();
            this.sync = new SectionSynchroniser(this);
            this.AddAnimation = UITableViewRowAnimation.Automatic;
            this.RemoveAnimation = UITableViewRowAnimation.Automatic;
        }
        
        public UITableView TableView
        {
            get 
            { 
                return this.tableView; 
            }

            set 
            { 
                if (value != this.tableView)
                {
                    if (this.tableView != null)
                    {
                        this.tableView.Source = null;
                    }

                    this.tableView = value; 

                    if (this.tableView != null)
                    {
                        this.tableView.Source = this;
                    }
                }
            }
        }

        public UITableViewRowAnimation AddAnimation { get; set; }
        
        public UITableViewRowAnimation RemoveAnimation { get; set; }

        public void Bind(IList<ISection> source)
        {
            this.sync.Bind(source);
        }

        public void Clear()
        {
            // unbind ..
            this.sections.Clear();
            this.ReloadView();
        }

        public void Load(IList<ISection> sourceList)
        {
            this.sections.Clear();
            this.sections.AddRange(sourceList);
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
            // just update the table view
            var sectionIndex = this.sections.IndexOf(section);
            var paths = new NSIndexPath[rows.Count];
            for (int i = 0; i < rows.Count; i++)
            {
                paths[i] = NSIndexPath.FromRowSection(index + i, sectionIndex);
            }

            this.TableView.InsertRows(paths, this.AddAnimation);
        }

        public void Remove(ISection section, int index, int count)
        {
            // just update the table view
            var sectionIndex = this.sections.IndexOf(section);
            var paths = new NSIndexPath[count];
            for (int i = 0; i < count; i++)
            {
                paths [i] = NSIndexPath.FromRowSection(index + i, sectionIndex);
            }

            this.TableView.DeleteRows(paths, this.RemoveAnimation);
        }
        
        public override string TitleForHeader(UITableView tableView, int section)
        {
            return this.sections[section].Header != null ? this.sections[section].Header.ToString() : null;
        }

        public override string TitleForFooter(UITableView tableView, int section)
        {
            return this.sections[section].Footer != null ? this.sections[section].Footer.ToString() : null;
        }

        public override int NumberOfSections(UITableView tableView)
        {
            return this.sections.Count;
        }

        public override int RowsInSection(UITableView tableview, int section)
        {
            return this.sections[section].Rows.Count;
        }

        public override UITableViewCell GetCell(UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
        {
            var row = this.sections[indexPath.Section].Rows[indexPath.Row];
            var cell = tableView.DequeueReusableCell("cell");
            if (cell == null)
            {
                cell = new UITableViewCell(UITableViewCellStyle.Default, "cell");
            }

            cell.TextLabel.Text = row.ToString();

            return cell;
        }

        protected virtual void ReloadView()
        {
            this.TableView.ReloadData();
        }
    }
}

