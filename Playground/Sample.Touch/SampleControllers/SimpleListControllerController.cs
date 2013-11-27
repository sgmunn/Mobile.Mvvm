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

    public class SimpleListControllerController : UITableViewController
    {
        private GroupedListSource source;
        private readonly IList<IGroup> groups;

        public SimpleListControllerController() : base (UITableViewStyle.Grouped)
        {
            this.groups = new ObservableCollection<IGroup>();

            var section1 = new GroupViewModel();
            section1.Header = new CaptionViewModel("Header");
            this.groups.Add(section1);

            section1.Rows.Add(new TestCommandRowViewModel(section1, "Add"));
            section1.Rows.Add(new StringElementViewModel("update") { TapCommand = new DelegateCommand(() => {
                    ((CaptionViewModel)section1.Rows[1]).Caption = "Updated!";
                }) });
            section1.Rows.Add(new CaptionViewModel("item 1"));
            section1.Rows.Add(new CaptionViewModel("item 2"));

            this.groups.Add(new GroupViewModel());
            this.groups[1].Rows.Add(new CaptionViewModel("item 1"));
            this.groups[1].Rows.Add(new CaptionViewModel("item 2"));
            this.groups[1].Rows.Add(new CaptionViewModel("item 3"));
            
            this.groups.Add(new GroupViewModel());
            this.groups[2].Header = new CaptionViewModel("Header");
            for (int i = 0; i < 100; i++)
            {
                this.groups[2].Rows.Add(new CaptionViewModel("item " + i.ToString()));
            }

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

            this.source.Bind(this.groups);
        }
    }
}

