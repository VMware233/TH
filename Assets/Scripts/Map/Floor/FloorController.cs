using Sirenix.OdinInspector;
using TH.GameCore;
using UnityEngine;
using VMFramework.UI;

namespace TH.Map
{
    public class FloorController : MonoBehaviour, ITracingTooltipProviderController
    {
        [ShowInInspector]
        public Floor floor { get; private set; }

        private Transform _graphic;

        public Transform graphicTransform
        {
            get
            {
                if (_graphic == null)
                {
                    _graphic = new GameObject("FloorGraphic")
                    {
                        transform =
                        {
                            parent = transform,
                            localPosition = Vector3.zero,
                            localRotation = Quaternion.identity,
                            localScale = Vector3.one
                        },
                        layer = GameSetting.floorGeneralSetting.floorLayer
                    }.transform;
                }

                return _graphic;
            }
        }

        public void Init(Floor floor)
        {
            this.floor = floor;

            OnInit();
        }

        protected virtual void OnInit()
        {

        }

        public ITracingTooltipProvider provider => floor;
    }
}
