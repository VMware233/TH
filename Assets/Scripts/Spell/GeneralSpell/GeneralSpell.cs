using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace TH.Spells
{
    public sealed class GeneralSpell : Spell
    {
        public class SpellOperationToken
        {
            public Action Abort { get; init; }

            public Func<bool> IsAborted { get; init; }

            public Func<SpellAbortInfo> GetAbortInfo { get; init; }

            public Action Complete { get; init; }
        }

        private GeneralSpellPreset generalSpellPreset => (GeneralSpellPreset)gamePrefab;

        private bool isAborted;
        private SpellAbortInfo spellAbortInfo;

        public override async void Cast(SpellCastInfo spellCastInfo)
        {
            if (cooldown > 0)
            {
                if (isDebugging)
                {
                    Debug.LogWarning("技能还在冷却中");
                }

                return;
            }

            int completedCount = 0;
            isAborted = false;

            var operationToken = new SpellOperationToken()
            {
                Abort = () => isAborted = true,
                IsAborted = () => isAborted,
                GetAbortInfo = () => spellAbortInfo,
                Complete = () => completedCount++,
            };

            foreach (var spellUnitAction in generalSpellPreset.spellUnitActions)
            {
                spellUnitAction.Examine(this, spellCastInfo, operationToken);
            }

            await UniTask.WaitUntil(() =>
                isAborted || completedCount >= generalSpellPreset.spellUnitActions.Count);

            cooldown.value = generalSpellPreset.maxCooldown;
        }

        public override void Abort(SpellAbortInfo spellAbortInfo)
        {
            if (cooldown > 0)
            {
                return;
            }

            isAborted = true;
            this.spellAbortInfo = spellAbortInfo;
        }
    }
}