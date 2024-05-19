using System;
using Cysharp.Threading.Tasks;
using TH.GameCore;
using UnityEngine.Scripting;
using VMFramework.Procedure;

namespace TH.Entities
{
    [GameInitializerRegister(typeof(ClientLoadingProcedure))]
    [Preserve]
    public sealed class PlayerClientInitializer : IGameInitializer
    {
        async void IInitializer.OnInit(Action onDone)
        {
            PlayerManager.instance.RequestCreatePlayer(GameSetting.playerGeneralSetting.defaultPlayerID);

            await UniTask.WaitUntil(() => PlayerManager.isThisPlayerInitialized);

            onDone();
        }
    }
}