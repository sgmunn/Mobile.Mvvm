using System;
using Mobile.Mvvm.DataBinding;
using NUnit.Framework;

namespace Mobile.Mvvm.UnitTests.Bindings
{
    [TestFixture]
    public class GivenATargetWithAnEvent
    {
        public EventTargetObject Target { get; private set; }
        public SimpleSourceObject Source { get; private set; }

        public void SetUp()
        {
            this.Source = new SimpleSourceObject();
            this.Target = new EventTargetObject();
        }

        [Test]
        public void WhenBindingWithAnEvent_ThenTheSourcePropertyIsUpdated()
        {
            this.SetUp();
            var binding = new Binding("Property1");
            var expression = new EventTriggeredBindingExpression<EventTargetObject, EventArgs>(this.Target, "PropertyA", "PropertyChanged", this.Source, binding).Bind();

            this.Target.PropertyA = Guid.NewGuid().ToString();

            Assert.AreEqual(this.Target.PropertyA, this.Source.Property1);
        }
        
        [Test]
        public void WhenDisposingTheExpression_ThenTargetObjectEventIsNull()
        {
            this.SetUp();
            var binding = new Binding("Property1");
            var expression = new EventTriggeredBindingExpression<EventTargetObject, EventArgs>(this.Target, "PropertyA", "PropertyChanged", this.Source, binding).Bind();

            expression.Dispose();

            Assert.AreEqual(this.Target.HasEventHandler, false);
        }
    }
}
