// --------------------------------------------------------------------------------------------------------------------
// <copyright file=".cs" company="sgmunn">
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

using System;
using MonoTouch.UIKit;
using Mobile.Mvvm.ViewModel.Dialog;
using System.Collections.Generic;
using Mobile.Mvvm;
using Sample.Touch.SampleControllers;
using Mobile.Mvvm.UI;
using Mobile.Utils;

namespace Sample.Touch.SampleControllers
{
    public class WidgetSampleController : UITableViewController
    {
        private GroupedListSource source;

        public WidgetSampleController() : base (UITableViewStyle.Grouped)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            this.source = new GroupedListSource();
            this.source.TableView = this.TableView;

            // Register the TableView's data source
            this.TableView.Source = this.source;

            var groups = new List<IGroup>();

            var section1 = new GroupViewModel();
            section1.Header = new CaptionViewModel("Samples");
            groups.Add(section1);

            section1.Rows.Add(new StringElementViewModel("Toast") { TapCommand = new DelegateCommand(() => {
                var md = new AlertMessageDisplay();
                md.DisplayToast("toasted\nHello", true);
            }) });

            section1.Rows.Add(new StringElementViewModel("Message") { TapCommand = new DelegateCommand(() => {
                var md = new AlertMessageDisplay();
                md.DisplayMessage("title", "message", Actions.Null);
            }) });

            this.source.Bind(groups);
        }
    }
    
}
