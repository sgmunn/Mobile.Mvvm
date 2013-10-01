// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InjectionScope.cs" company="sgmunn">
//   (c) sgmunn 2013  
//
//   Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//   documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
//   the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
//   to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
//   The above copyright notice and this permission notice shall be included in all copies or substantial portions of 
//   the Software.
//
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO 
//   THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
//   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
//   CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS 
//   IN THE SOFTWARE.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Mobile.Mvvm.DataBinding
{
    using System;
    using System.Collections.Generic;

    public sealed class InjectionScope : IInjectionScope
    {
        private readonly Dictionary<object, IInjectedPropertyStore> injectedPropertyStores;

        private readonly Func<object, IInjectedPropertyStore> factory;

        public InjectionScope()
        {
            this.injectedPropertyStores = new Dictionary<object, IInjectedPropertyStore>();
        }
        
        public InjectionScope(Func<object, IInjectedPropertyStore> factory)
        {
            this.injectedPropertyStores = new Dictionary<object, IInjectedPropertyStore>();
            this.factory = factory;
        }

        public void SetInjectedProperty(object owner, InjectedProperty property, object value)
        {
            var store = this.GetInjectedPropertyies(owner);

            if (store == null && this.factory != null)
            {
                store = this.factory(owner);
                if (store != null)
                {
                    this.AddInjectionStore(owner, store);
                }
            }

            if (store != null)
            {
                store.SetInjectedProperty(property, value);
            }
        }

        public object GetInjectedProperty(object owner, InjectedProperty property)
        {
            var store = this.GetInjectedPropertyies(owner);
            if (store != null)
            {
                return store.GetInjectedProperty(property);
            }

            return null;
        }

        public void RemoveInjectedProperty(object owner, InjectedProperty property)
        {
            var store = this.GetInjectedPropertyies(owner);
            if (store != null)
            {
                store.RemoveInjectedProperty(property);
            }
        }

        public void RemoveAllInjectedProperties(object owner)
        {
            var store = this.GetInjectedPropertyies(owner);
            if (store != null)
            {
                store.RemoveAllInjectedProperties();
            }
        }

        public void AddInjectionStore(object owner, IInjectedPropertyStore store)
        {
            if (owner == null)
            {
                throw new ArgumentNullException("owner");
            }

            if (store == null)
            {
                throw new ArgumentNullException("store");
            }

            this.injectedPropertyStores[owner] = store;
        }        

        public void RemoveInjectionStore(object owner)
        {
            var store = this.GetInjectedPropertyies(owner);
            if (store != null)
            {
                store.RemoveAllInjectedProperties();
                this.injectedPropertyStores.Remove(owner);
            }
        }        

        public void Clear()
        {
            foreach (var store in this.injectedPropertyStores.Values)
            {
                store.RemoveAllInjectedProperties();
            }

            this.injectedPropertyStores.Clear();
        }        

        public IInjectedPropertyStore GetInjectedPropertyies(object owner)
        {
            IInjectedPropertyStore store;
            if (this.injectedPropertyStores.TryGetValue(owner, out store))
            {
                return store;
            }

            return null;
        }        

        public void Dispose()
        {
            this.Clear();
        }       
    }
}
