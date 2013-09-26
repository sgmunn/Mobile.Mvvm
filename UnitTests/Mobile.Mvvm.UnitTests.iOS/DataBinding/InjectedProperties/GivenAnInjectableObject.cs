
using System;
using Mobile.Mvvm.DataBinding;

namespace Mobile.Mvvm.UnitTests.InjectedProperties
{
    public abstract class GivenAnInjectableObject
    {
        public virtual void Setup()
        {
            this.Injectable = new InjectableObject();
        }

        public IPropertyInjection Injectable{ get; private set; }
    }

    public class InjectableObject : IPropertyInjection
    {
        public InjectableObject()
        {
            this.InjectedProperties = new InjectedPropertyStore(this);
        }

        public IInjectedPropertyStore InjectedProperties
        {
            get; 
            private set;
        } 
    }
}
