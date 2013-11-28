using System;
using Mobile.Mvvm.DataBinding;

namespace Mobile.Mvvm.UnitTests.Bindings
{
    public abstract class GivenABindingExpression : GivenASourceAndTarget
    {
        public IBindingExpression Expression { get; protected set; }

        public override void SetUp()
        {
            base.SetUp();

            this.Expression = this.GetExpression();
        }

        protected virtual IBindingExpression GetExpression()
        {
            var binding = new WeakBindingExpression(this.Target, "PropertyA", this.Source, this.Binding);
            binding.Bind();
            return binding;
        }
    }
}
