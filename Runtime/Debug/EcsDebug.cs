using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

[assembly: InternalsVisibleTo("FFS.StaticEcs.Unity.Editor")]

namespace FFS.Libraries.StaticEcs.Unity {
    #if UNITY_EDITOR
    internal sealed class StaticEcsDebugData {
        internal static WorldData Current;
    }
    #endif

    public abstract class EcsDebug {

        #if UNITY_EDITOR
        #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
        public static DebugViewSystem DebugViewSystem { get; private set; }
        #endif
        #endif

        public static void AddWorld<TSystemsType>(Func<EntityGID, string> windowEntityNameFunction = null, short systemOrder = short.MaxValue)
            where TSystemsType : struct, ISystemsType {
            #if UNITY_EDITOR
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            if (World.Status == WorldStatus.NotCreated) {
                throw new InvalidOperationException("StaticEcsWorldDebug Debug mode connection is possible only after world creation");
            }

            var worldData = new WorldData {
                Handle = World.Data.Handle,
                WindowNameFunction = windowEntityNameFunction,
            };

            StaticEcsDebugData.Current = worldData;
            DebugViewSystem = new DebugViewSystem();
            World.Systems<TSystemsType>.Add(DebugViewSystem, systemOrder);
            #endif
            #endif
        }

        public static void RemoveWorld() {
            #if UNITY_EDITOR
            #if ((DEBUG || FFS_ECS_ENABLE_DEBUG) && !FFS_ECS_DISABLE_DEBUG)
            DebugViewSystem = null;
            StaticEcsDebugData.Current = null;
            #endif
            #endif
        }
    }
}