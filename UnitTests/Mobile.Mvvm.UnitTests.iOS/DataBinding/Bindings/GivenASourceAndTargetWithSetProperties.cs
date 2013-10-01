using System;
using NUnit.Framework;
using Mobile.Mvvm.DataBinding;

namespace Mobile.Mvvm.UnitTests.Bindings
{
    [TestFixture]
    public class GivenASourceAndTargetWithSetProperties : GivenASourceAndTarget
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            this.Source.Property1 = Guid.NewGuid().ToString();
            this.Target.PropertyA = Guid.NewGuid().ToString();
        }

        [Test]
        public void WhenBindingTheSourceAndTarget_ThenTheTargetPropertyIsUpdated()
        {
            new WeakBindingExpression(this.Target, "PropertyA", this.Source, this.Binding).Bind();
            Assert.AreEqual(this.Source.Property1, this.Target.PropertyA);
        }
    }
}
