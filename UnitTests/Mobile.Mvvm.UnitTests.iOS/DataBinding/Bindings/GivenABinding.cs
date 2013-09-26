
using System;
using Mobile.Mvvm.DataBinding;

namespace Mobile.Mvvm.UnitTests.Bindings
{
    public abstract class GivenABinding
    {
        public Binding Binding { get; protected set; }

        public virtual void SetUp()
        {
            this.Binding = new Binding("Property1");
        }
    }
}
