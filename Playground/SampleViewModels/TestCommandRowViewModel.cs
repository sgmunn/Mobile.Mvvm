using System;
using Mobile.Mvvm.ViewModel;
using Mobile.Mvvm;
using Mobile.Mvvm.ViewModel.Dialog;

namespace SampleViewModels
{
    public class TestCommandRowViewModel : StringViewModel, ITapCommand
    {
        private GroupViewModel group;

        public TestCommandRowViewModel(GroupViewModel group, string caption) : base(caption)
        {
            this.group = group;   
            this.TapCommand = new DelegateCommand(() => {
                this.group.Rows.Add(new StringViewModel("added"));
            });
        }

        public Mobile.Mvvm.ICommand TapCommand { get; private set; }
    }
}
