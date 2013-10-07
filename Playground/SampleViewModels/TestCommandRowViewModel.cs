using System;
using Mobile.Mvvm.ViewModel;
using Mobile.Mvvm;
using Mobile.Mvvm.ViewModel.Dialog;

namespace SampleViewModels
{
    public class TestCommandRowViewModel : StringViewModel, ITapCommand
    {
        private SectionViewModel section;

        public TestCommandRowViewModel(SectionViewModel section, string caption) : base(caption)
        {
            this.section = section;   
            this.TapCommand = new DelegateCommand(() => {
                this.section.Rows.Add(new StringViewModel("added"));
            });
        }

        public Mobile.Mvvm.ICommand TapCommand { get; private set; }
    }
}
