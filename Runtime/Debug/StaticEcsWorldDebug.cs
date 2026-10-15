#if UNITY_EDITOR
using System;
using System.Collections.Generic;

namespace FFS.Libraries.StaticEcs.Unity {
    internal static class TypeDescriptorData {
        public static ushort Value = 1;
        public static readonly Dictionary<ushort, Type> Types = new();
    }

    internal static class TypeDescriptor<T> {
        public static readonly ushort Value;

        static TypeDescriptor() {
            Value = TypeDescriptorData.Value++;
            TypeDescriptorData.Types[Value] = typeof(T);
        }
    }

    public struct TypeIdx {
        public ushort Value;

        public static TypeIdx Create<T>() {
            return new TypeIdx {
                Value = TypeDescriptor<T>.Value
            };
        }

        public Type Type => TypeDescriptorData.Types[Value];
    }

    public abstract class AbstractWorldData {
        public WorldHandle Handle;
        public Func<EntityGID, string> WindowNameFunction;
    }

    internal class WorldData<TWorld> : AbstractWorldData
        where TWorld : struct, IWorldType {
    }
}
#endif
