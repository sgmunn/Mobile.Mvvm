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
            yield return new DataTemplate("c1")
                .Creates<TableViewCell>((id, root) => new TableViewCell(UITableViewCellStyle.Default, (string)id))
                    .WhenBinding<StringViewModel, TableViewCell>((c, vm, view) => {
                        // ok, we need to reset the bindings for the current target, also our reference to the inner text label
                        // gets cleared - we have a choice. use a strong binding to the target or write custom classes.
                        // the dispose semantics of built in classes allow MT to dispose managed objects if no references to them
                        // 
                        // android has a similar issue, we need to reset bindings for target as well
                        // and because we have a scope for each table / controller we have to explicity pass it to here, or pass to source and then to datatemplate extensions

                        //c.Bindings.AddBinding(view, "Text", vm, "Caption");

                        var binding = new Binding("Caption", new DelegatePropertyAccessor<StringViewModel, string>(x => x.Caption, (x,v) => x.Caption = v));
                        c.Bindings.AddBinding(view, "Text", vm, binding);


                    });
        }
    }

    public class SimpleListControllerController : UITableViewController
    {
        private SectionSource source;
        private IList<ISection> sections;

        public SimpleListControllerController() : base (UITableViewStyle.Grouped)
        {
            this.sections = new ObservableCollection<ISection>();

            var section1 = new SectionViewModel();
            section1.Header = new StringViewModel("Header");
            this.sections.Add(section1);

            section1.Rows.Add(new TestCommandRowViewModel(section1, "Add"));
            section1.Rows.Add(new StringElementViewModel("update") { TapCommand = new DelegateCommand(() => {
                    ((StringViewModel)section1.Rows[1]).Caption = "Updated!";
                }) });
            section1.Rows.Add(new StringViewModel("item 1"));
            section1.Rows.Add(new StringViewModel("item 2"));

            this.sections.Add(new SectionViewModel());
            this.sections[1].Rows.Add(new StringViewModel("item 1"));
            this.sections[1].Rows.Add(new StringViewModel("item 2"));
            this.sections[1].Rows.Add(new StringViewModel("item 3"));
            
            this.sections.Add(new SectionViewModel());
            this.sections[2].Header = new StringViewModel("Header");
            for (int i = 0; i < 100; i++)
            {
                this.sections[2].Rows.Add(new StringViewModel("item " + i.ToString()));
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
            this.source = new SectionSource(DialogDataTemplates.DefaultTemplates());
            this.source.TableView = this.TableView;

            
            // Register the TableView's data source
            this.TableView.Source = this.source;

            this.source.Bind(this.sections);
        }
    }
}

