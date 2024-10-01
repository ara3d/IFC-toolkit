using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebIfcDotNetTests
{
    public class ProjectId
    {
        public readonly string Value;
        public ProjectId(string value) => Value = value;
        public static implicit operator string(ProjectId id) => id.Value;
        public static implicit operator ProjectId(string value) => new(value);
    }

    public class ObjectId
    {
        public readonly string Value;
        public ObjectId(string value) => Value = value;
        public static implicit operator string(ObjectId id) => id.Value;
        public static implicit operator ObjectId(string value) => new(value);
    }

    public class ModelId
    {
        public readonly string Value;
        public ModelId(string value) => Value = value;
        public static implicit operator string(ModelId id) => id.Value;
        public static implicit operator ModelId(string value) => new(value);
    }

    public class VersionId
    {
        public readonly string Value;
        public VersionId(string value) => Value = value;
        public static implicit operator string(VersionId id) => id.Value;
        public static implicit operator VersionId(string value) => new(value);
    }
}
