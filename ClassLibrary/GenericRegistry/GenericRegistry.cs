using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.GenericRegistry
{
    public class GenericRegistry<TItemInterface>
    {
        private readonly Dictionary<Type, TItemInterface> registryItems = new Dictionary<Type, TItemInterface>();

        public void RegisterWithType<TItemType>(TItemType item)
            where TItemType : TItemInterface
        {
            if (this.registryItems.ContainsKey(typeof(TItemType)))
            {
                throw new InvalidOperationException($"Item with type {typeof(TItemType).FullName} has already been registered");
            }
            else
            {
                this.registryItems.Add(typeof(TItemType), item);
            }
        }

        public TItemType GetByType<TItemType>()
            where TItemType : TItemInterface
        {
            if (this.registryItems.TryGetValue(typeof(TItemType), out TItemInterface item))
            {
                return (TItemType)item;
            }
            else
            {
                throw new InvalidOperationException($"Item with type {typeof(TItemType).FullName} isn't registered");
            }
        }

        public IEnumerable<TItemInterface> GetItems()
        {
            return this.registryItems.Values;
        }
    }
}
