using System;
using UnityEngine;
using VMFramework.GameEvents;
using VMFramework.Procedure;
using VMFramework.UI;

namespace TH.UI
{
    [ManagerCreationProvider(ManagerType.EventCore)]
    public class ColliderTooltipManager : ManagerBehaviour<ColliderTooltipManager>, IManagerBehaviour
    {
        void IInitializer.OnInitComplete(Action onDone)
        {
            ColliderMouseEventManager.AddCallback(MouseEventType.PointerEnter, OnEnter);
            ColliderMouseEventManager.AddCallback(MouseEventType.PointerLeave, OnLeave);
            
            onDone();
        }

        private void OnEnter(ColliderMouseEvent mouseEvent)
        {
            if (mouseEvent.trigger.owner == null)
            {
                return;
            }
            
            if (mouseEvent.trigger.owner.TryGetComponent(out ITooltipProviderController controller))
            {
                TooltipManager.Open(controller.provider, null);
            }
        }

        private void OnLeave(ColliderMouseEvent mouseEvent)
        {
            if (mouseEvent.trigger.owner == null)
            {
                return;
            }
            
            if (mouseEvent.trigger.owner.TryGetComponent(out ITooltipProviderController controller))
            {
                TooltipManager.Close(controller.provider);
            }
        }
    }
}