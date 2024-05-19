using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace VMFramework.Procedure
{
    public static class InitializerUtility
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Initialize(this IInitializer initializer, InitializeType type, Action onDone)
        {
            switch (type)
            {
                case InitializeType.BeforeInit:
                    initializer.OnBeforeInit(onDone);
                    break;
                case InitializeType.PreInit:
                    initializer.OnPreInit(onDone);
                    break;
                case InitializeType.Init:
                    initializer.OnInit(onDone);
                    break;
                case InitializeType.PostInit:
                    initializer.OnPostInit(onDone);
                    break;
                case InitializeType.InitComplete:
                    initializer.OnInitComplete(onDone);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Initialize(this IEnumerable<IInitializer> initializers, InitializeType type,
            Action onDone)
        {
            foreach (var initializer in initializers)
            {
                initializer.Initialize(type, onDone);
            }
        }
    }
}