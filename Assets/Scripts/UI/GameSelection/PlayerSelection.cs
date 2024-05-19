using TH.Entities;
using TH.GameCore;
using VMFramework.Core;
using UnityEngine;
using VMFramework.Procedure;

namespace TH.UI
{
    [ManagerCreationProvider(nameof(GameManagerType.UI))]
    public class PlayerSelection : ManagerBehaviour<PlayerSelection>
    {
        public static Vector2 GetThisPlayerPositionOnScreen()
        {
            if (PlayerManager.isThisPlayerInitialized == false)
            {
                return default;
            }

            var playerTransform = PlayerManager.GetThisPlayerController().transform;

            var camera = CameraManager.mainCamera;

            var screenPosition = camera.WorldToScreenPoint(playerTransform.position);

            return screenPosition;
        }

        public static Vector2 GetThisPlayerToMouseDirection()
        {
            var playerPosition = GetThisPlayerPositionOnScreen();
            var mousePosition = Input.mousePosition.XY();

            return (mousePosition - playerPosition).normalized;
        }
    }
}
