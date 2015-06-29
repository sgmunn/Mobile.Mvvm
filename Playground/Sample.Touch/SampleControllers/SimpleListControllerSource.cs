using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Sample.Touch.SampleControllers
{
    public class SimpleListControllerSource : UITableViewSource
    {
        public SimpleListControllerSource()
        {
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            // TODO: return the actual number of sections
            return 1;
        }

        public override nint RowsInSection(UITableView tableview, nint group)
        {
            // TODO: return the actual number of items in the group
            return 1;
        }

        public override string TitleForHeader(UITableView tableView, nint group)
        {
            return "Header";
        }

        public override string TitleForFooter(UITableView tableView, nint group)
        {
            return "Footer";
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell(SimpleListControllerCell.Key) as SimpleListControllerCell;
            if (cell == null)
                cell = new SimpleListControllerCell();
            
            // TODO: populate the cell with the appropriate data based on the indexPath
            cell.DetailTextLabel.Text = "DetailsTextLabel";
            
            return cell;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            tableView.DeselectRow(indexPath, true);
        }
    }
}

