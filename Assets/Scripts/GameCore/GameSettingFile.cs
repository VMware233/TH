
using TH.Buffs;
using TH.Damage;
using TH.Entities;
using TH.Items;
using TH.Map;
using TH.Spells;
using VMFramework.GameLogicArchitecture;

namespace TH.GameCore
{
    public class GameSettingFile : GameCoreSettingBaseFile
    {
        public WorldGeneralSetting worldGeneralSetting;
        public WorldGenerationRuleGeneralSetting worldGenerationRuleGeneralSetting;
        public RoomGeneralSetting roomGeneralSetting;
        public FloorGeneralSetting floorGeneralSetting;

        public BuffGeneralSetting buffGeneralSetting;
        
        public DamageGeneralSetting damageGeneralSetting;
        
        public ItemGeneralSetting itemGeneralSetting;
        
        public EntityGeneralSetting entityGeneralSetting;
        public PlayerGeneralSetting playerGeneralSetting;
        public PlayerActionStateGeneralSetting playerActionStateGeneralSetting;
        public ProjectileGeneralSetting projectileGeneralSetting;

        //public CreatureGeneralSetting creatureGeneralSetting;
        //public CharacterGeneralSetting characterGeneralSetting;
        //public ContainerUIGeneralSetting containerUIGeneralSetting;
        public SpellGeneralSetting spellGeneralSetting;
    }
}
