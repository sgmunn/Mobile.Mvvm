using System;
using Mobile.Mvvm.ViewModel.Dialog;
using System.Collections.Generic;
using Mobile.Mvvm;

namespace SampleViewModels
{
    public static class SimpleListViewModel
    {
        public static IList<IGroup> GetViewModel()
        {
            var groups = new List<IGroup>();

            var section1 = new GroupViewModel();
            section1.Header = new CaptionViewModel("Header 1");
            groups.Add(section1);

            section1.Rows.Add(new TestCommandRowViewModel(section1, "add"));
            section1.Rows.Add(new StringElementViewModel("update") { TapCommand = new DelegateCommand(() => {
                ((CaptionViewModel)section1.Rows[1]).Caption = "Updated!";
            }) });
            section1.Rows.Add(new CaptionViewModel("item 1"));
            section1.Rows.Add(new CaptionViewModel("item 2"));

            groups.Add(new GroupViewModel());
            groups[1].Rows.Add(new CaptionViewModel("item 1"));
            groups[1].Rows.Add(new CaptionViewModel("item 3"));
            groups[1].Rows.Add(new CaptionViewModel("item 2"));

            groups[1].Header = new CaptionViewModel("Header 2");
            groups[1].Footer = new CaptionViewModel("Footer 2");

            groups.Add(new GroupViewModel());
            groups[2].Header = new CaptionViewModel("Header 3");
            for (int i = 0; i < 100; i++)
            {
                groups[2].Rows.Add(new CaptionViewModel("item " + i.ToString()));
            }

            return groups;
        }
    }
}

