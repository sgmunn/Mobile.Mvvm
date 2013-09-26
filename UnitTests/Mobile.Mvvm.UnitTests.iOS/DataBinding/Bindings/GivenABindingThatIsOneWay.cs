
using System;
using NUnit.Framework;
using Mobile.Mvvm.DataBinding;

namespace Mobile.Mvvm.UnitTests.Bindings
{
    [TestFixture]
    public class GivenABindingThatIsOneWay : GivenABindingExpression
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            this.Binding.Mode = BindingMode.OneWay;
        }
        
        [Test]
        public void WhenSettingTheTargetProperty_ThenTheSourceIsNotUpdated()
        {
            this.Target.PropertyA = Guid.NewGuid().ToString();
            Assert.AreEqual(null, this.Source.Property1);
        }
        
        [Test]
        public void WhenSettingTheSourceProperty_ThenTheTargetIsUpdated()
        {
            this.Source.Property1 = Guid.NewGuid().ToString();
            Assert.AreEqual(this.Source.Property1, this.Target.PropertyA);
        }
    }
}
