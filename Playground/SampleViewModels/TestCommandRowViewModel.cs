using System;
using Mobile.Mvvm.ViewModel;

namespace SampleViewModels
{
    public class TestCommandRowViewModel : RowViewModel
    {
        private SectionViewModel section;

        public TestCommandRowViewModel(SectionViewModel section)
        {
            this.section = section;    
        }

        public override void Execute()
        {
            this.section.Rows.Add(new RowViewModel());
        }

        public override bool GetCanExecute()
        {
            return true;
        }
    }
}
