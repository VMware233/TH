using Sirenix.OdinInspector;

namespace TH.Map
{
    public partial class RoomGeneralSetting
    {
        [LabelText("开启导入时ID和标记检查警告"), TabGroup(TAB_GROUP_NAME, ROOM_IMPORT_CATEGORY)]
        public bool enableIDAndFlagImportCheckingWarning = true;

        [LabelText("开启房间资源元数据字段丢失警告"), TabGroup(TAB_GROUP_NAME, ROOM_IMPORT_CATEGORY)]
        public bool enableRoomAssetMetaFieldMissingWarning = true;

        [LabelText("开启房间导入成功提示"), TabGroup(TAB_GROUP_NAME, ROOM_IMPORT_CATEGORY)]
        public bool enableRoomImportSuccessLog = true;
    }
}