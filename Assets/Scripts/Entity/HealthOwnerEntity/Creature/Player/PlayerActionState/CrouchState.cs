using Sirenix.OdinInspector;
using System.Collections.Generic;
using TH.GameEvents;
using VMFramework.Core.FSM;
using VMFramework.GameEvents;

namespace TH.Entities
{
    public class CrouchState : PlayerActionState, IMultiFSMState<string, PlayerController>
    {
        protected CrouchStateConfig crouchStateConfig => (CrouchStateConfig)gamePrefab;

        [ShowInInspector]
        private HashSet<ICrouchActivatable> crouchActivatables = new();

        #region Init

        public override void OnInit()
        {
            base.OnInit();

            GameEventManager.AddCallback(PlayerGameEvents.CROUCH, EnterThisState);

            GameEventManager.AddCallback(PlayerGameEvents.CROUCH_CANCEL, ExitThisState);
        }

        #endregion

        void IMultiFSMState<string, PlayerController>.Update(bool isActive)
        {
            if (isActive)
            {
                foreach (var crouchActivatable in fsm.owner.playerCrouchTrigger.Raycast(crouchStateConfig
                             .raycastCount))
                {
                    if (crouchActivatables.Add(crouchActivatable))
                    {
                        crouchActivatable.CrouchActivate(fsm.owner.player);
                    }
                }
            }
        }

        void IMultiFSMState<string, PlayerController>.OnExit()
        {
            foreach (var crouchActivatable in crouchActivatables)
            {
                crouchActivatable.CrouchInActivate(fsm.owner.player);
            }

            crouchActivatables.Clear();
        }
    }
}
