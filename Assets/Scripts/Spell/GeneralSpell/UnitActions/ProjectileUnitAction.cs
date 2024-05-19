using VMFramework.Configuration;
using VMFramework.Core;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using TH.Damage;
using TH.Entities;
using UnityEngine;
using VMFramework.GameLogicArchitecture;
using VMFramework.OdinExtensions;

namespace TH.Spells
{
    [TypeInfoBox("投射物技能单元")]
    public sealed partial class ProjectileUnitAction : SpellUnitAction
    {
        public override SpellTargetType supportedTargetType
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (projectileType == ProjectileType.SeekerProjectile)
                {
                    return SpellTargetType.Entities;
                }

                return SpellTargetType.Entities | SpellTargetType.Direction;
            }
        }

        [LabelText("投射物种类")]
        [SerializeField]
        private ProjectileType projectileType;

        private bool isLaser => projectileType == ProjectileType.FixedLaserBeam;

        [LabelText("线性投掷物")]
        [ShowIf(nameof(projectileType), ProjectileType.LinearProjectile)]
        [SerializeField]
        private IGamePrefabIDChooserConfig<ILinearProjectileConfig> linearProjectileID;

        [LabelText("追踪投掷物")]
        [ShowIf(nameof(projectileType), ProjectileType.SeekerProjectile)]
        [SerializeField]
        private IGamePrefabIDChooserConfig<ISeekerProjectileConfig> seekerProjectileID;

        [LabelText("激光投掷物")]
        [ShowIf(nameof(projectileType), ProjectileType.FixedLaserBeam)]
        [SerializeField]
        private IGamePrefabIDChooserConfig<IFixedLaserBeamConfig> laserProjectileID;

        [LabelText("散射角度"), PreviewCompositeSettings("°")]
        [Minimum(0)]
        [SerializeField]
        private IChooserConfig<float> projectileScatterAngle = new SingleValueChooserConfig<float>(30);

        [LabelText("不跟随鼠标")]
        [SerializeField]
        private bool noMouseAngle = false;

        [LabelText("非散射")]
        [SerializeField]
        private bool noScatter = false;

        [LabelText("打乱投射物顺序")]
        [SerializeField]
        private bool shuffleProjectiles = false;

        [LabelText("投射物数量"), PreviewCompositeSettings("个")]
        [Minimum(1)]
        [SerializeField]
        private IChooserConfig<int> projectileNumbers = new SingleValueChooserConfig<int>(1);

        [LabelText("投射物间隔"), PreviewCompositeSettings("秒")]
        [Minimum(0)]
        [SerializeField]
        private IChooserConfig<float> projectileInterval = new SingleValueChooserConfig<float>();

        [LabelText("投射物速度")]
        [Minimum(0)]
        [SerializeField]
        [HideIf(nameof(isLaser))]
        private IChooserConfig<float> projectileSpeed = new SingleValueChooserConfig<float>(10);

        [LabelText("投射物生成位置")]
        [EnumToggleButtons]
        [SerializeField]
        private ProjectileSpawnPosition projectileSpawnPosition;

        [LabelText("延迟"), PreviewCompositeSettings("秒")]
        [Minimum(0)]
        [SerializeField]
        private IChooserConfig<float> delay = new SingleValueChooserConfig<float>();
        
        [LabelText("是否是近战攻击")]
        [SerializeField]
        private bool isMelee = false;
        
        [LabelText("物理攻击")]
        [SerializeField]
        private IChooserConfig<int> physicalAttack = new SingleValueChooserConfig<int>(1);

        [LabelText("魔法攻击")]
        [SerializeField]
        private IChooserConfig<int> magicalAttack = new SingleValueChooserConfig<int>(0);

        public override async void Examine(Spell spell, Spell.SpellCastInfo spellCastInfo,
            GeneralSpell.SpellOperationToken operationToken)
        {
            float unitDelay = delay.GetValue();

            if (unitDelay > 0)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(unitDelay));
            }

            Vector2 mainDirection;

            if (spellCastInfo.targetType.HasFlag(SpellTargetType.Direction))
            {
                mainDirection = spellCastInfo.mainDirection;
            }
            else if (spellCastInfo.targetType.HasFlag(SpellTargetType.Entities))
            {
                Transform nearestEntityTransform = default;
                float nearestDistance = float.MaxValue;

                var casterPosition = spellCastInfo.caster.casterPosition;
                foreach (var entity in spellCastInfo.entities)
                {
                    var entityTransform = entity.controller.transform;

                    var distance = entityTransform.position.XY().EuclideanDistance(casterPosition);

                    if (distance < nearestDistance)
                    {
                        nearestDistance = distance;
                        nearestEntityTransform = entityTransform;
                    }
                }

                if (nearestEntityTransform == null)
                {
                    return;
                }

                mainDirection = nearestEntityTransform.position.XY() - casterPosition;
            }
            else
            {
                throw new ArgumentException($"不支持的目标类型:{spellCastInfo.targetType}");
            }

            float targetAngle;

            if (!noMouseAngle)
                targetAngle = mainDirection.ClockwiseAngle();
            else targetAngle = 0;

            int numbers = projectileNumbers.GetValue();

            float[] angles;

            if (numbers == 1)
            {
                angles = new[] { targetAngle };
            }
            else
            {
                /*var (minAngle, maxAngle) =
                    targetAngle.GetMinMaxPointFromPivotExtents(projectileScatterAngle / 2);*/

                float minAngle, maxAngle;

                if (!noScatter)
                {
                    (minAngle, maxAngle) =
                    targetAngle.GetMinMaxPointFromPivotExtents(projectileScatterAngle.GetValue() / 2);
                }
                else
                {
                    minAngle = 0;
                    maxAngle = projectileScatterAngle.GetValue();
                }

                angles = minAngle.GetUniformlySpacedPoints(maxAngle, numbers).ToArray();
            }

            var initialVelocities = angles.Select(angle => angle.ClockwiseAngleToDirection()).ToList();

            if (isLaser == false)
            {
                for (int i = 0; i < initialVelocities.Count; i++)
                {
                    initialVelocities[i] *= projectileSpeed.GetValue();
                }
            }

            if (shuffleProjectiles)
            {
                initialVelocities.Shuffle();
            }

            for (int i = 0; i < numbers; i++)
            {
                Vector2 spawnPosition = projectileSpawnPosition switch
                {
                    ProjectileSpawnPosition.Caster => spellCastInfo.caster.castPosition,
                    ProjectileSpawnPosition.Owner => spell.owner.castPosition,
                    _ => throw new ArgumentOutOfRangeException()
                };

                if (projectileType == ProjectileType.LinearProjectile)
                {
                    var entity = IGameItem.Create<Entity>(linearProjectileID.GetID());

                    if (entity is ILinearProjectile linearProjectile)
                    {
                        spellCastInfo.caster.ProduceDamagePacket(null, out var packet);
                        
                        packet = ProcessDamagePacket(packet);

                        linearProjectile.Init(new GeneralLinearProjectile.InitInfo()
                        {
                            sourceEntity = spell.owner as Entity,
                            sourceSpell = spell,
                            initialVelocity = initialVelocities[i],
                            damagePacket = packet
                        });

                        EntityManager.CreateEntity(entity, spawnPosition);
                    }
                }
                else if (projectileType == ProjectileType.SeekerProjectile)
                {
                    var entity = IGameItem.Create<Entity>(seekerProjectileID.GetID());

                    if (entity is ISeekerProjectile seekerProjectile)
                    {
                        var trackingTarget = spellCastInfo.entities.ChooseOrDefault();

                        spellCastInfo.caster.ProduceDamagePacket(trackingTarget as IDamageable,
                            out var packet);
                        
                        packet = ProcessDamagePacket(packet);

                        seekerProjectile.Init(new GeneralSeekerProjectile.InitInfo()
                        {
                            sourceEntity = spell.owner as Entity,
                            sourceSpell = spell,
                            damagePacket = packet,
                            initialVelocity = initialVelocities[i],
                            trackingTarget = trackingTarget?.controller.transform,
                        });

                        EntityManager.CreateEntity(entity, spawnPosition);
                    }
                }
                else if (projectileType == ProjectileType.FixedLaserBeam)
                {
                    var entity = IGameItem.Create<GeneralFixedLaserBeam>(laserProjectileID.GetID());

                    if (entity is IFixedLaserBeam laserProjectile)
                    {
                        spellCastInfo.caster.ProduceDamagePacket(null, out var packet);
                        
                        packet = ProcessDamagePacket(packet);

                        laserProjectile.Init(new GeneralFixedLaserBeam.InitInfo()
                        {
                            sourceEntity = spell.owner as Entity,
                            sourceSpell = spell,
                            damagePacket = packet,

                            //if ()
                            
                            direction = initialVelocities[i]
                        });

                        EntityManager.CreateEntity(entity, spawnPosition);
                    }
                }

                if (i != numbers - 1)
                {
                    float interval = projectileInterval.GetValue();

                    if (interval > 0)
                    {
                        await UniTask.Delay(TimeSpan.FromSeconds(interval));

                        if (operationToken.IsAborted())
                        {
                            return;
                        }
                    }
                }
            }

            operationToken.Complete();
        }

        private DamagePacket ProcessDamagePacket(DamagePacket damagePacket)
        {
            damagePacket.isMelee = isMelee;
            damagePacket.physicalDamage += physicalAttack.GetValue();
            damagePacket.magicalDamage += magicalAttack.GetValue();
            return damagePacket;
        }
    }

}