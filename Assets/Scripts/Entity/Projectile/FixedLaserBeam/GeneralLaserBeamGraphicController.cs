using Sirenix.OdinInspector;
using UnityEngine;

namespace TH.Entities
{
    public class GeneralLaserBeamGraphicController : MonoBehaviour
    {
        #region Config

        [SerializeField]
        [Required]
        private LineRenderer lineRenderer;

        #endregion

        public void SetStart(Vector2 start)
        {
            lineRenderer.SetPosition(0, start);
        }

        public void SetEnd(Vector2 end)
        {
            lineRenderer.SetPosition(1, end);
        }
    }
}
