using System;
using Mobile.Mvvm.ViewModel;

namespace SampleViewModels
{
    public class SimpleViewModel : ViewModelBase
    {
        public SimpleViewModel()
        {
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
    }
}

