using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Mobile.Mvvm.ViewModel;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using SampleViewModels;
using Mobile.Mvvm.ViewModel.Dialog;
using Mobile.Mvvm;
using Mobile.Mvvm.DataBinding;

namespace Sample.Touch.SampleControllers
{
    public class TableViewCell : UITableViewCell
    {
        private string text;

        public TableViewCell(string reuseIdentifer)
            : base(UITableViewCellStyle.Default, reuseIdentifer)
        {
        }
        
        public TableViewCell(UITableViewCellStyle style, string reuseIdentifer)
            : base(style, reuseIdentifer)
        {
        }

        ~TableViewCell()
        {
            Console.WriteLine("~TableViewCell {0}", this.GetType().Name);
        }

        public string Text
        {
            get
            {
                return this.text;
            }

            set
            {
                if (value != this.text)
                {
                    this.text = value;
                    this.TextChanged(value);
                }
            }
        }

        protected virtual void TextChanged(string newValue)
        {
            this.TextLabel.Text = newValue;
            // this.LayoutSubviews();
            //this.SetNeedsDisplay();
        }
    }




    public static class DialogDataTemplates
    {
        public static IEnumerable<IDataTemplate> DefaultTemplates()
        {
            // for iOS, we need to have either strong bindings or create our own stubb classes
            // not sure of the best way to go, 

            // a DataTemplate is the description of a single view type to a single view model type
            // therefore it needs to be a generic class <TView, TViewModel>
            
            yield return new DataTemplate<TableViewCell, StringWrapperElementViewModel>("c1", "Text: Value")
                .OnCreate((id) => new TableViewCell(UITableViewCellStyle.Default, id));

            yield return new DataTemplate<TableViewCell, CaptionViewModel>("c1", "Text: Caption")
                    .OnCreate((id) => new TableViewCell(UITableViewCellStyle.Default, id));

//            yield return new DataTemplate("c1")
//                .Creates<TableViewCell>((id) => new TableViewCell(UITableViewCellStyle.Default, (string)id))
//                    .Bind<StringViewModel>("Text: Caption");
//                    .WhenBinding<StringViewModel, UITableViewCell>((c, vm, view) => {

//                        //c.Bindings.AddBinding(view, "Text", vm, "Caption");
//
//                        // IPropertyAccessor instances can be shared
//
//                        var targetPropertySetter = new DelegatePropertyAccessor<UITableViewCell, string>(x => x.TextLabel.Text, (x,v) => x.TextLabel.Text = v);
//                        var sourcePropertySetter = new DelegatePropertyAccessor<StringViewModel, string>(x => x.Caption, (x,v) => x.Caption = v);
//                        var binding = new Binding("Caption", sourcePropertySetter);
//
//                        c.Bindings.AddBinding(view, "Text", targetPropertySetter, vm, binding);
//
//
//                    });
        }
    }

    public class SimpleListController : UITableViewController
    {
        private GroupedListSource source;

        public SimpleListController() : base (UITableViewStyle.Grouped)
        {

//            this.NavigationItem.RightBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Add, (s,e) => {
//                this.sections[0].Rows.Add(new RowViewModel());
//            });
        }

        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();
            
            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            this.source = new GroupedListSource(DialogDataTemplates.DefaultTemplates());
            this.source.TableView = this.TableView;

            
            // Register the TableView's data source
            this.TableView.Source = this.source;

            this.source.Bind(SimpleListViewModel.GetViewModel());
        }
    }

    public class SimpleViewModelController : UITableViewController
    {
        private GroupedListSource source;

        public SimpleViewModelController() : base (UITableViewStyle.Grouped)
        {

//            this.NavigationItem.RightBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Add, (s,e) => {
//                this.sections[0].Rows.Add(new RowViewModel());
//            });
        }

        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();

            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            this.source = new GroupedListSource(DialogDataTemplates.DefaultTemplates());
            this.source.TableView = this.TableView;

            var theViewModel = new SimpleViewModel();

            theViewModel.Property1 = "Hello";
            theViewModel.Property2 = "World";


            // Register the TableView's data source
            this.TableView.Source = this.source;

            this.source.Bind(WrappedSimpleViewModel.GetViewModel(theViewModel));
        }
    }
}

