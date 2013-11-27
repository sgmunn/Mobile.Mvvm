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

            this.TestCommand3 = new DelegateCommand(() =>
                {
                    this.Property2 = Guid.NewGuid().ToString();
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

        public string Property2
        {
            get
            {
                return (string)this.GetPropertyValue("Property2");
            }

            set
            {
                this.SetPropertyValue("Property2", value);
            }
        }

        public bool BoolProperty1
        {
            get
            {
                return (bool)this.GetPropertyValue("BoolProperty1");
            }

            set
            {
                this.SetPropertyValue("BoolProperty1", value);
            }
        }

        public ICommand TestCommand { get; set; }
        
        public ICommand TestCommand2 { get; set; }

        public ICommand TestCommand3 { get; set; }
    }
}

