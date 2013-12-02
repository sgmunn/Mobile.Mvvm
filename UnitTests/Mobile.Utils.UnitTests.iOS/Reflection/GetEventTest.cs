
using System;
using NUnit.Framework;

namespace Mobile.Utils.UnitTests.Reflection
{
    public class EventTest
    {
        public event EventHandler InstanceName;

        public static event EventHandler StaticName;
    }


    [TestFixture]
    public class GetEventTest
    {
        [Test]
        public void WhenGettingAnInstanceEvent_ThenTheEventIsNotNull()
        {
            var evInfo = Mobile.Utils.ReflectionExtensions.GetEvent(typeof(EventTest), "InstanceName");
            Assert.AreNotEqual(null, evInfo);
        }

        [Test]
        public void WhenGettingAStaticEvent_ThenTheEventIsNotNull()
        {
            var evInfo = Mobile.Utils.ReflectionExtensions.GetEvent(typeof(EventTest), "StaticName");
            Assert.AreNotEqual(null, evInfo);
        }

        [Test]
        public void WhenGettingAnUndefinedEvent_ThenTheEventIsNull()
        {
            var evInfo = Mobile.Utils.ReflectionExtensions.GetEvent(typeof(EventTest), "NotDefined");
            Assert.AreEqual(null, evInfo);
        }
    }
}
