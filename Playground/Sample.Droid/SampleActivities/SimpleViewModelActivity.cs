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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Collections.ObjectModel;
using Mobile.Mvvm.ViewModel;
using SampleViewModels;
using Mobile.Mvvm.ViewModel.Dialog;
using Mobile.Mvvm;
using Mobile.Mvvm.DataBinding;
using Android.Util;
using System.Threading;
using System.Collections.Concurrent;

namespace Sample.Droid.SampleActivities
{

    [Activity (Label = "SimpleViewModelActivity")]            
    public class SimpleViewModelActivity : ListActivity
    {
        private GroupedListSource source;
        private IList<IGroup> groups;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            var theViewModel = new SimpleViewModel();

            theViewModel.Property1 = "Hello";
            theViewModel.Property2 = "World";


            this.groups = new ObservableCollection<IGroup>();




            var group1 = new GroupViewModel();
            group1.Header = new CaptionViewModel("Header 1");
            this.groups.Add(group1);

            group1.Rows.Add(new StringWrapperElementViewModel(theViewModel, "Property1"));
            group1.Rows.Add(new StringWrapperElementViewModel(theViewModel, "Property2"));

            group1.Rows.Add(new StringElementViewModel("tap me") { TapCommand = new DelegateCommand(() => {
                ((StringWrapperElementViewModel)group1.Rows[1]).Value = "i was clicked";
            }) });


            // view did load equivalent
            this.source = new GroupedListSource(this, DialogDataTemplates.DefaultTemplates(this));
            this.source.ListView = this.ListView;

            // Register the TableView's data source
            this.ListView.Adapter = this.source;

            this.source.Bind(this.groups);

            // Create your application here
        }
    }
}
