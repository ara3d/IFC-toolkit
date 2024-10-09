using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ara3D.IfcParser
{
    public static class Extensions
    {
        public static void Add<TKey, TValue>(this IDictionary<TKey, List<TValue>> self, TKey key, TValue value)
        {
            if (!self.ContainsKey(key))
                self[key] = new List<TValue>();
            self[key].Add(value);
        }
    }
}
