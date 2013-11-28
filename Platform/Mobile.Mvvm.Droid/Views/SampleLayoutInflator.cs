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
using System.Collections.Concurrent;
using Android.Views;
using Android.Content;

namespace Mobile.Mvvm.Views
{
    /*  usage, in an activity
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Android.Views.LayoutInflater.From(this).Factory = MyFactory.Default;

            // view did load equivalent
            this.source = new GroupedListSource(this, DialogDataTemplates.DefaultTemplates(this));
            this.source.ListView = this.ListView;

            // Register the TableView's data source
            this.ListView.Adapter = this.source;

            this.source.Bind(SimpleListViewModel.GetViewModel());
        }
     */

    public class SampleLayoutInflator : Java.Lang.Object, LayoutInflater.IFactory
    {
        private readonly ConcurrentDictionary<string, string> bindings;

        public static SampleLayoutInflator Default = new SampleLayoutInflator();

        public SampleLayoutInflator()
        {
            this.bindings = new ConcurrentDictionary<string, string>();    
        }

        public string BindingForId(int id)
        {
            var key = "@" + id.ToString();  
            if (this.bindings.ContainsKey(key))
            {
                return this.bindings[key];
            }

            return string.Empty;
        }

        public View OnCreateView(string name, Context context, Android.Util.IAttributeSet attrs)
        {
            var x = attrs.GetAttributeValue("http://schemas.android.com/apk/res/android", "id");
            //Console.WriteLine("id = {0}", x);

            var y = attrs.GetAttributeValue("http://schemas.mvvm.mobile.com/android", "bind");
            //Console.WriteLine("bind = {0}", y);

            if (!string.IsNullOrEmpty(x) && !string.IsNullOrEmpty(y))
            {
                this.bindings.TryAdd(x, y);
            }

            return null;
        }
    }
}

