using System;
using Mobile.Mvvm.ViewModel;
using Mobile.Mvvm;
using Mobile.Utils;

namespace SampleViewModels
{
    public class SimpleViewModel : ViewModelBase
    {
        public SimpleViewModel()
        {
            this.DecimalProperty1 = 123.4m;

            this.TestCommand = new DelegateCommand(() =>
            {
                this.Property1 = "Clicked";

                }, () => this.CommandDisabled);

            this.TestCommand2 = new DelegateCommand(() =>
                                                   {
                    this.CommandDisabled = !this.CommandDisabled;
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
                return this.GetPropertyValue<string>("Property1");
            }

            set
            {
                this.SetPropertyValue("Property1", value);
                if (value == "xxx")
                {
                    this.Property1Error = "Sorry, this value is invalid";
                }
                else
                {
                    this.Property1Error = null;
                }
            }
        }

        public string Property1Error
        {
            get
            {
                return this.GetPropertyValue<string>("Property1Error");
            }

            set
            {
                this.SetPropertyValue("Property1Error", value);
            }
        }

        public string Property2
        {
            get
            {
                return this.GetPropertyValue<string>("Property2");
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
                return this.GetPropertyValue<bool>("BoolProperty1");
            }

            set
            {
                this.SetPropertyValue("BoolProperty1", value);
            }
        }

        public bool CommandDisabled
        {
            get
            {
                return this.GetPropertyValue<bool>("CommandDisabled", false);
            }

            set
            {
                this.SetPropertyValue("CommandDisabled", value);
            }
        }

        public decimal DecimalProperty1
        {
            get
            {
                return this.GetPropertyValue<decimal>("DecimalProperty1", 0);
            }

            set
            {
                this.SetPropertyValue("DecimalProperty1", value);
            }
        }

        public ICommand TestCommand { get; set; }
        
        public ICommand TestCommand2 { get; set; }

        public ICommand TestCommand3 { get; set; }


        public override IStateBundle SaveState()
        {
            var state = base.SaveState();

            state.Data["CommandDisabled"] = CommandDisabled;
            state.Data["P1"] = Property1;

            return state;
        }

        public override void RestoreState(IStateBundle state)
        {
            base.RestoreState(state);
            if (state.Data.Count > 0)
            {
                this.CommandDisabled = (bool)state.Data["CommandDisabled"];
                this.Property1 = (string)state.Data["P1"];
            }
        }
    }
}

