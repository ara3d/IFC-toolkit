using System.Collections;
using System.Collections.Generic;
using Speckle.Core.Models;

namespace Ara3D.Speckle.Data
{
    public static class SpeckleConverter 
    {
        public static object ToSpeckleObject(this object o, Dictionary<string, SpeckleObject> lookup)
        {
            if (o is Base b)
                return ToSpeckleObject(b, lookup);

            if (o is string s)
                return s;

            if (o is IDictionary d)
            {
                var r = new SpeckleObject { DotNetType = o.GetType().Name };
                foreach (var k in d.Keys)
                    r.Properties.Add((string)k, ToSpeckleObject(d[k], lookup));
                if (r.Properties.Count == 0)
                    return null;
                return r;
            }
            
            if (o is IEnumerable seq)
            {
                var r = new SpeckleObject { DotNetType = o.GetType().Name };
                foreach (var item in seq)
                    r.Elements.Add(ToSpeckleObject(item, lookup));
                if (r.Elements.Count == 0)
                    return null;
                return r;
            }

            return o;
        }

        public static SpeckleObject ToSpeckleObject(this Base b, Dictionary<string, SpeckleObject> lookup = null)
        {
            if (b == null)
                return null;

            lookup = lookup ?? new Dictionary<string, SpeckleObject>();

            // Get or compute the ID. 
            // NOTE: "ComputeId" would be a better name for ID.
            var id = b.id ?? b.GetId();

            if (lookup.TryGetValue(id, out var found))
                return found;

            // Create a new SpeckleObject
            var r = lookup[id] 
                = new SpeckleObject { Id = id };

            var type = b.GetType();
            r.DotNetType = type.Name;
            r.SpeckleType = b.speckle_type;

            foreach (var kv in b.GetDynamicAndInstanceMembers())
                r.Properties.Add(kv.Key, ToSpeckleObject(kv.Value, lookup));

            return r;
        }
    }

    public static class SpeckleExtensions
    {
        public static IEnumerable<KeyValuePair<string, object>> GetDynamicAndInstanceMembers(this Base self)
            => self.GetMembers(DynamicBaseMemberType.Instance | DynamicBaseMemberType.Dynamic);
    }
}