using System;
using NUnit.Framework;

namespace Mobile.Mvvm.UnitTests.DataBinding.Bindings.ComplexPaths
{
    [TestFixture]
    public class G
    {
//        [SetUp]
//        public override void SetUp()
//        {
//            base.SetUp();
//        }

        [Test]
        public void WhenSettingTheTargetProperty_ThenTheSourceIsUpdated()
        {
            //this.Target.PropertyA = Guid.NewGuid().ToString();
            //Assert.AreEqual(this.Target.PropertyA, this.Source.Property1);
        }

        [Test]
        public void WhenSettingTheSourceProperty_ThenTheTargetIsUpdated()
        {
            //this.Source.Property1 = Guid.NewGuid().ToString();
            //Assert.AreEqual(this.Source.Property1, this.Target.PropertyA);
        }
    }
}
