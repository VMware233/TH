using System.Collections.Generic;
using System.Linq;
using FishNet.Serializing;
using TH.Entities;
using UnityEngine;
using UnityEngine.Scripting;

namespace TH.Spells
{
    [Preserve]
    public static class SpellSerializer
    {
        public static void WriteSpellCastInfo(this Writer writer, Spell.SpellCastInfo spellCastInfo)
        {
            writer.WriteString(spellCastInfo.caster.uuid);

            writer.WriteInt32((int)spellCastInfo.targetType);

            if (spellCastInfo.targetType.HasFlag(SpellTargetType.Entities))
            {
                var entitiesUUID = new List<string>();

                entitiesUUID.AddRange(spellCastInfo.entities.Select(entity => entity.uuid));

                writer.WriteList(entitiesUUID);
            }

            if (spellCastInfo.targetType.HasFlag(SpellTargetType.Direction))
            {
                writer.WriteVector2(spellCastInfo.mainDirection);
            }

            if (spellCastInfo.targetType.HasFlag(SpellTargetType.Position))
            {
                writer.WriteVector2(spellCastInfo.mainPosition);
            }
        }

        public static Spell.SpellCastInfo ReadSpellCastInfo(this Reader reader)
        {
            var casterUUID = reader.ReadString();

            if (EntityManager.TryGetOwner(casterUUID, out var casterEntity) == false)
            {
                Debug.LogWarning($"不存在此uuid:{casterUUID}对应的{nameof(Entity)}");
            }

            ISpellCaster caster = casterEntity as ISpellCaster;

            SpellTargetType spellTargetType = (SpellTargetType)reader.ReadInt32();

            List<Entity> entities = new();

            if (spellTargetType.HasFlag(SpellTargetType.Entities))
            {
                var entitiesUUID = new List<string>();

                reader.ReadList(ref entitiesUUID);

                foreach (var uuid in entitiesUUID)
                {
                    if (EntityManager.TryGetOwner(uuid, out var entity))
                    {
                        entities.Add(entity);
                    }
                }
            }

            Vector2 mainDirection = default;

            if (spellTargetType.HasFlag(SpellTargetType.Direction))
            {
                mainDirection = reader.ReadVector2();
            }

            Vector2 mainPosition = default;

            if (spellTargetType.HasFlag(SpellTargetType.Position))
            {
                mainPosition = reader.ReadVector2();
            }

            return new Spell.SpellCastInfo()
            {
                caster = caster,
                targetType = spellTargetType,
                entities = entities,
                mainDirection = mainDirection,
                mainPosition = mainPosition,
            };
        }
    }
}