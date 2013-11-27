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
    using MonoTouch.Foundation;
    using MonoTouch.UIKit;
    using Mobile.Mvvm.DataBinding;

    public class GroupedListSource: UITableViewSource, IGroupSource, IBindingContext
    {
        private readonly List<IGroup> groups;

        private readonly GroupSourceSynchroniser sync;

        private readonly List<IDataTemplate> templates;

        private UITableView tableView;

        public GroupedListSource() : this(Enumerable.Empty<IDataTemplate>())
        {
        }
        
        public GroupedListSource(IEnumerable<IDataTemplate> templates)
        {
            this.groups = new List<IGroup>();
            this.sync = new GroupSourceSynchroniser(this);
            this.Bindings = new BindingScope();
            this.InjectedProperties = new InjectionScope();
            this.AddAnimation = UITableViewRowAnimation.Automatic;
            this.RemoveAnimation = UITableViewRowAnimation.Automatic;
            this.templates = templates.ToList();
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
            this.Bindings.ClearBindings();
            this.InjectedProperties.Clear();
            this.groups.Clear();
            this.ReloadView();
        }

        public void Load(IList<IGroup> sourceList)
        {
            this.Bindings.ClearBindings();
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

        public void Insert(IGroup group, int index, IList<IViewModel> rows)
        {
            // just update the table view
            var groupIndex = this.groups.IndexOf(group);
            var paths = new NSIndexPath[rows.Count];
            for (int i = 0; i < rows.Count; i++)
            {
                paths[i] = NSIndexPath.FromRowSection(index + i, groupIndex);
            }

            this.TableView.InsertRows(paths, this.AddAnimation);
        }

        public void Remove(IGroup group, int index, int count)
        {
            // just update the table view
            var groupIndex = this.groups.IndexOf(group);
            var paths = new NSIndexPath[count];
            for (int i = 0; i < count; i++)
            {
                paths [i] = NSIndexPath.FromRowSection(index + i, groupIndex);
            }

            this.TableView.DeleteRows(paths, this.RemoveAnimation);
        }
        
        public override string TitleForHeader(UITableView tableView, int group)
        {
            return this.groups[group].Header != null ? this.groups[group].Header.ToString() : null;
        }

        public override string TitleForFooter(UITableView tableView, int group)
        {
            return this.groups[group].Footer != null ? this.groups[group].Footer.ToString() : null;
        }

        public override int NumberOfSections(UITableView tableView)
        {
            return this.groups.Count;
        }

        public override int RowsInSection(UITableView tableview, int group)
        {
            return this.groups[group].Rows.Count;
        }

        public override UITableViewCell GetCell(UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
        {
            var row = this.ViewModelForIndexPath(indexPath);
            var cell = (UITableViewCell)row.GetTemplate(this.templates).GetViewForViewModel(this, row, (id) => tableView.DequeueReusableCell((string)id));

            // fallback default view
            if (cell == null)
            {
                cell = tableView.DequeueReusableCell("cell");
                if (cell == null)
                {
                    cell = new UITableViewCell(UITableViewCellStyle.Default, "cell");
                }

                cell.TextLabel.Text = row.ToString();
            }

            return cell;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            tableView.DeselectRow(indexPath, true);

            var row = this.ViewModelForIndexPath(indexPath);
            row.ExecuteTapCommand();
        }

        protected IViewModel ViewModelForIndexPath(NSIndexPath indexPath)
        {
            return this.groups[indexPath.Section].Rows[indexPath.Row];
        }

        protected virtual void ReloadView()
        {
            this.TableView.ReloadData();
        }
    }
}

