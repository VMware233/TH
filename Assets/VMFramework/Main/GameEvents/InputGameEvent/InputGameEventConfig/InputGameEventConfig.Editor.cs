﻿#if UNITY_EDITOR
namespace VMFramework.GameEvents
{
    public partial class InputGameEventConfig
    {
        protected InputActionGroup AddActionGroupGUI()
        {
            return new()
            {
                actions = new()
                {
                    new()
                    {
                        keyBoardTriggerType = KeyBoardTriggerType.KeyDown,
                        holdThreshold = 0.3f
                    }
                }
            };
        }
    }
}
#endif