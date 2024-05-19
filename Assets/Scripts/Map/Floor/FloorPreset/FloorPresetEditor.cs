#if UNITY_EDITOR
using UnityEngine;

namespace TH.Map
{
    public partial class FloorPreset
    {
        protected override void OnInspectorInit()
        {
            base.OnInspectorInit();

            boxColliderSize ??= new(Vector2.zero, Vector2.one);
        }
    }
}
#endif