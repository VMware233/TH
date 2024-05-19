using Sirenix.OdinInspector;
using System.Runtime.CompilerServices;
using UnityEngine;

public abstract class GameSettingBase : SerializedScriptableObject
{
    #region Categories

    protected const string TAB_GROUP_NAME = "TabGroup";

    public const string DEBUGGING_CATEGORY = "调试";
    
    protected const string MISCELLANEOUS_CATEGORY = "杂项设置";
    
    protected const string METADATA_CATEGORY = "设置元数据";

    #endregion

    public virtual bool isSettingUnmovable => false;

    public virtual string forcedFileName => null;

    public void PreInit()
    {
        OnPreInit();
    }

    public void Init()
    {
        OnInit();
    }

    public void PostInit()
    {
        OnPostInit();
    }

    public void FinishInit()
    {
        OnFinishInit();
    }

    protected virtual void OnPreInit()
    {

    }

    protected virtual void OnInit()
    {

    }

    protected virtual void OnPostInit()
    {

    }

    protected virtual void OnFinishInit()
    {

    }

    public virtual void CheckSettings()
    {

    }

    public virtual void CheckSettingsGUI()
    {
        CheckSettings();
    }

    private bool hasSettingError = false;

    protected string checkSettingInfo = "未检查设置！！！";

    protected virtual void OnValidate()
    {
        checkSettingInfo = "设置已更新，请重新点击检查设置按钮！！！";
    }

    [Title("检查")]
    [InfoBox("此配置文件不可移动，不可创建多份", InfoMessageType.Warning, 
        VisibleIf = nameof(isSettingUnmovable))]
    [InfoBox(@"@""此配置文件不可改名，当前名称："" + name + ""，请改回名称："" + forcedFileName", 
        InfoMessageType.Warning, VisibleIf = @"@forcedFileName != null && name != forcedFileName")]
    [InfoBox("@" + nameof(checkSettingInfo), InfoMessageType.Warning)]
    [HideInInlineEditors]
    [GUIColor(nameof(GetSettingCheckColorGUI)), PropertySpace(SpaceBefore = 10), PropertyOrder(999)]
    [Button("检查设置", ButtonSizes.Large, Icon = SdfIconType.ArrowRightCircle)]
    private void CheckSettingsButton()
    {
        hasSettingError = true;
        checkSettingInfo = "设置有问题！请查看控制台";

        CheckSettingsGUI();

        checkSettingInfo = "设置无问题！";
        hasSettingError = false;
    }

    private Color GetSettingCheckColorGUI()
    {
        if (hasSettingError)
        {
            return new(1, 0, 0);
        }
        else
        {
            return new(0, 1, 0);
        }
    }
}