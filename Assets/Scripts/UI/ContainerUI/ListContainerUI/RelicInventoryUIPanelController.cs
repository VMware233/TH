using System.Threading;
using Cysharp.Threading.Tasks;
using TH.Entities;
using VMFramework.UI;

namespace TH.UI
{
    public class RelicInventoryUIPanelController : ContainerUIPanelController
    {
        protected CancellationTokenSource autoOpenInventoryCancellationTokenSource;

        protected override async void OnOpenInstantly(IUIPanelController source)
        {
            base.OnOpenInstantly(source);

            autoOpenInventoryCancellationTokenSource = new();

            if (containerUICore.bindContainer == null)
            {
                await UniTask.WaitUntil(() => PlayerManager.isThisPlayerInitialized,
                    cancellationToken: autoOpenInventoryCancellationTokenSource.Token);

                SetBindContainer(PlayerManager.GetThisPlayerRelicInventory());
            }
        }

        protected override void OnCloseInstantly(IUIPanelController source)
        {
            base.OnCloseInstantly(source);

            autoOpenInventoryCancellationTokenSource?.Cancel();
        }
    }
}
