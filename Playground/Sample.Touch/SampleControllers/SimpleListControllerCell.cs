using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Sample.Touch.SampleControllers
{
    public class SimpleListControllerCell : UITableViewCell
    {
        public static readonly NSString Key = new NSString("SimpleListControllerCell");

        public SimpleListControllerCell() : base (UITableViewCellStyle.Value1, Key)
        {
            // TODO: add subviews to the ContentView, set various colors, etc.
            TextLabel.Text = "TextLabel";
        }
    }
}

