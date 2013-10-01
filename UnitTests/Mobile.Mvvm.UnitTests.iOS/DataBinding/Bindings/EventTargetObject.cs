using System;
using System.ComponentModel;

namespace Mobile.Mvvm.UnitTests.Bindings
{
    public class EventTargetObject
    {
        private string propertyA;

        public event EventHandler PropertyChanged; 

        public string PropertyA
        {
            get
            {
                return this.propertyA;
            }

            set
            {
                if (value != this.propertyA)
                {
                    this.propertyA = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        public bool HasEventHandler
        {
            get
            {
                return this.PropertyChanged != null;
            }
        }

        private void NotifyPropertyChanged()
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }
    }
}
