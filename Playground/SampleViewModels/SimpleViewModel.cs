using System;
using Mobile.Mvvm.ViewModel;
using Mobile.Mvvm;

namespace SampleViewModels
{
    public class SimpleViewModel : ViewModelBase
    {
        public SimpleViewModel()
        {
            this.TestCommand = new DelegateCommand(() =>
            {
                this.Property1 = "Clicked";
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
    }
}

