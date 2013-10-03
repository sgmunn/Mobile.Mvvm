using System;
using Mobile.Mvvm.ViewModel;

namespace SampleViewModels
{
    // TODO: helper for property change and storage of values, drop in component


    public class SimpleViewModel : ViewModelBase
    {
        private string property1;

        public SimpleViewModel()
        {
        }

        public string Property1
        {
            get
            {
                return this.property1;
            }

            set
            {
                if (value != this.property1)
                {
                    this.property1 = value;
                    this.NotifyPropertyChanged("Property1");
                }
            }
        }
    }
}

