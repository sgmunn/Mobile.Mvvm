using System;

namespace Mobile.Mvvm.UnitTests.InjectedProperties
{
    public class DisposableObject : IDisposable
    {
        private Action onDispose;

        public DisposableObject(Action onDispose)
        {
            this.onDispose = onDispose;
        }

        public void Dispose()
        {
            this.onDispose();
        }
    }
}
