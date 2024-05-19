using System;
using VMFramework.Procedure;

namespace TH.Utilities
{
    [GameInitializerRegister(typeof(GameInitializationProcedure))]
    public class LayerManagerInitializer : IGameInitializer
    {
        public void OnPreInit(Action onDone)
        {
            LayerManager.Init();
            onDone();
        }
    }
}