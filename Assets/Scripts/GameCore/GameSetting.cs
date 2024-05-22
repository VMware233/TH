using TH.Buffs;
using TH.Damage;
using TH.Entities;
using TH.Items;
using TH.Map;
using TH.Spells;
using VMFramework.GameLogicArchitecture;

namespace TH.GameCore
{
    public class GameSetting : GameCoreSetting
    {
        public static GameSettingFile gameSettingFile => (GameSettingFile)gameCoreSettingsFile;

        public static WorldGeneralSetting worldGeneralSetting =>
            gameSettingFile.worldGeneralSetting;

        public static WorldGenerationRuleGeneralSetting worldGenerationRuleGeneralSetting =>
            gameSettingFile.worldGenerationRuleGeneralSetting;

        public static RoomGeneralSetting roomGeneralSetting =>
            gameSettingFile.roomGeneralSetting;

        public static FloorGeneralSetting floorGeneralSetting =>
            gameSettingFile.floorGeneralSetting;

        public static DamageGeneralSetting damageGeneralSetting =>
            gameSettingFile.damageGeneralSetting;

        public static ItemGeneralSetting itemGeneralSetting =>
            gameSettingFile.itemGeneralSetting;

        public static EntityGeneralSetting entityGeneralSetting =>
            gameSettingFile.entityGeneralSetting;

        public static PlayerGeneralSetting playerGeneralSetting =>
            gameSettingFile.playerGeneralSetting;

        public static PlayerActionStateGeneralSetting playerActionStateGeneralSetting =>
            gameSettingFile.playerActionStateGeneralSetting;

        public static ProjectileGeneralSetting projectileGeneralSetting =>
            gameSettingFile.projectileGeneralSetting;

        public static BuffGeneralSetting buffGeneralSetting => 
            gameSettingFile.buffGeneralSetting;

        //public static CreatureGeneralSetting creatureGeneralSetting =>
        //    gameSettingFile.creatureGeneralSetting;

        //public static CharacterGeneralSetting characterGeneralSetting =>
        //    gameSettingFile.characterGeneralSetting;

        //public static ContainerUIGeneralSetting containerUIGeneralSetting =>
        //    gameSettingFile.containerUIGeneralSetting;

        public static SpellGeneralSetting spellGeneralSetting =>
            gameSettingFile.spellGeneralSetting;
    }
}
