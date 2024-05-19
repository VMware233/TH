using System.Runtime.CompilerServices;
using VMFramework.GameLogicArchitecture;
using VMFramework.Core.FSM;
using VMFramework.GameEvents;

namespace TH.Entities
{
    public abstract class PlayerActionState : GameItem, IMultiFSMState<string, PlayerController>
    {
        protected Player player { get; private set; }

        protected IMultiFSM<string, PlayerController> fsm { get; private set; }

        void IMultiFSMState<string, PlayerController>.Init(IMultiFSM<string, PlayerController> fsm)
        {
            this.fsm = fsm;
            player = fsm.owner.player;
            
            OnInit();
        }
        
        public virtual void OnInit()
        {
            
        }

        bool IMultiFSMState<string, PlayerController>.CanEnter()
        {
            return true;
        }

        void IMultiFSMState<string, PlayerController>.OnEnter()
        {
            
        }

        bool IMultiFSMState<string, PlayerController>.CanExit()
        {
            return true;
        }

        void IMultiFSMState<string, PlayerController>.OnExit()
        {
            
        }

        void IMultiFSMState<string, PlayerController>.Update(bool isActive)
        {
            
        }

        void IMultiFSMState<string, PlayerController>.FixedUpdate(bool isActive)
        {
            
        }

        void IMultiFSMState<string, PlayerController>.OnDestroy()
        {
            
        }

        #region This State Utility

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void EnterThisState(BoolInputGameEvent boolEvent)
        {
            fsm.EnterState(id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void ExitThisState(BoolInputGameEvent boolEvent)
        {
            if (fsm.HasCurrentState(id))
            {
                fsm.ExitState(id);
            }
        }

        #endregion
    }
}
