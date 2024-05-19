using TH.Entities;
using UnityEngine;
using VMFramework.GameLogicArchitecture;
using VMFramework.Network;

namespace TH.Buffs
{
    public interface IBuff : IVisualGameItem, IUUIDOwner
    {
        public float duration { get; set; }
        
        public int level { get; set; }
        
        public Sprite icon { get; }

        public void OnAddToEntity(Entity entity);

        public void OnRemoveFromEntity(Entity entity);

        public void SetOwner(Entity owner);

        public bool AllowBackgroundRun();

        public bool ShouldOtherBuffDropOffBeforeRunningBackground(IBuff otherBuff);

        public int CompareStrength(IBuff otherBuff);

        public bool CanStack(IBuff otherBuff);

        public void Stack(IBuff otherBuff);
    }
}