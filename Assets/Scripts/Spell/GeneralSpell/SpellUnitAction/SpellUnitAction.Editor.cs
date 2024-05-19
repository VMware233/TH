#if UNITY_EDITOR
using Sirenix.OdinInspector;
using VMFramework.Core.Editor;

namespace TH.Spells
{
    public partial class SpellUnitAction
    {
        [Button("打开技能单元脚本")]
        private void OpenScript()
        {
            GetType().OpenScriptOfType();
        }
    }
}
#endif