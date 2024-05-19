using System;
using TH.Entities;
using VMFramework.Property;

namespace TH.Property
{
    public abstract class PropertyOfPlayer : PropertyOfGameItem
    {
        protected Player player => (Player)target;

        protected sealed override void OnTargetChanged(object previous, object current)
        {
            base.OnTargetChanged(previous, current);

            var previousPlayer = (Player)previous;
            var currentPlayer = (Player)current;

            OnPlayerChanged(previousPlayer, currentPlayer);
        }

        protected abstract void OnPlayerChanged(Player previous, Player current);
    }

    public abstract class PlayerPropertyConfig : PropertyConfig
    {
        public override Type targetType => typeof(Player);

        protected override string idPrefix => "player";

        public sealed override string GetValueString(object target)
        {
            return GetPlayerValueString((Player)target);
        }

        protected abstract string GetPlayerValueString(Player player);
    }
}