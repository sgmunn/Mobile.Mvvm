using System;
using Mobile.Mvvm.ViewModel;
using Mobile.Mvvm;

namespace SampleViewModels
{
    public class SimpleViewModel : ViewModelBase
    {
        private bool enabled = true;

        public SimpleViewModel()
        {
            this.TestCommand = new DelegateCommand(() =>
            {
                this.Property1 = "Clicked";

            }, () => this.enabled);

            this.TestCommand2 = new DelegateCommand(() =>
                                                   {
                this.enabled = !this.enabled;
                this.TestCommand.RaiseCanExecuteChanged();

            });
        }

        public string Property1
        {
            get
            {
                return (string)this.GetPropertyValue("Property1");
            }

            set
            {
                this.SetPropertyValue("Property1", value);
            }
        }

        public ICommand TestCommand { get; set; }
        
        public ICommand TestCommand2 { get; set; }
    }
}

