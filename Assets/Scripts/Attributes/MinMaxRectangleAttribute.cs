#if UNITY_EDITOR
using System;
using Sirenix.OdinInspector.Editor;
using UnityEngine;
using UnityEditor;
using VMFramework.Configuration;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class MinMaxRectangleAttribute : Attribute
{
    public float MinX { get; private set; }
    public float MinY { get; private set; }
    public float MaxX { get; private set; }
    public float MaxY { get; private set; }
    public bool UseSpriteAspect { get; private set; } // 新的布尔属性

    public MinMaxRectangleAttribute(float minX, float minY, float maxX, float maxY, bool useSpriteAspect = true)
    {
        MinX = minX;
        MinY = minY;
        MaxX = maxX;
        MaxY = maxY;

        UseSpriteAspect = useSpriteAspect; // 设置布尔属性
    }
}
public class MinMaxRectangleAttributeDrawer : OdinAttributeDrawer<MinMaxRectangleAttribute, RectangleFloatConfig>
{
    private Sprite sprite; // Sprite变量来存储引用
    private const float MaxWidth = 300f; // 设置一个最大宽度

    protected override void DrawPropertyLayout(GUIContent label)
    {
        // 开始检查GUI是否有变动
        EditorGUI.BeginChangeCheck();

        // 获取属性值
        RectangleFloatConfig value = this.ValueEntry.SmartValue;
        var attribute = this.Attribute;

        // 限制min和max的值
        value.min.x = Mathf.Clamp(value.min.x, attribute.MinX, value.max.x);
        value.min.y = Mathf.Clamp(value.min.y, attribute.MinY, value.max.y);
        value.max.x = Mathf.Clamp(value.max.x, value.min.x, attribute.MaxX);
        value.max.y = Mathf.Clamp(value.max.y, value.min.y, attribute.MaxY);

        // 绘制Sprite选择器
        sprite = (Sprite)EditorGUILayout.ObjectField("Sprite", sprite, typeof(Sprite), false);

        // 根据attribute.UseSpriteAspect决定使用哪个宽高比
        float aspectRatio = (attribute.UseSpriteAspect && sprite != null)
            ? sprite.rect.width / sprite.rect.height
            : attribute.MaxX / attribute.MaxY; // 如果UseSpriteAspect为false，则使用MinMaxRectangle定义的比例

        // 计算绘制区域的宽度和高度
        float width = Mathf.Min(MaxWidth, EditorGUIUtility.currentViewWidth - 20); // 使用最大宽度或当前视图宽度减去一些边距
        float height = width / aspectRatio; // 根据比例计算高度

        // 创建背景区域，根据比例动态调整大小
        Rect squareRect = GUILayoutUtility.GetRect(width, height, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false));

        // 设置背景色并绘制背景
        Color backgroundColor = new Color(0.75f, 0.75f, 0.75f, 1f); // 浅灰色背景
        EditorGUI.DrawRect(squareRect, backgroundColor);

        // 如果有Sprite，并且Sprite有纹理，则在背景上绘制裁剪过的Sprite
        if (sprite != null && attribute.UseSpriteAspect)
        {
            Rect texCoords = sprite.textureRect;
            texCoords.x /= sprite.texture.width;
            texCoords.width /= sprite.texture.width;
            texCoords.y /= sprite.texture.height;
            texCoords.height /= sprite.texture.height;
            GUI.DrawTextureWithTexCoords(squareRect, sprite.texture, texCoords);
        }

        // 如果GUI变动了，更新属性值
        if (EditorGUI.EndChangeCheck())
        {
            this.ValueEntry.SmartValue = value;
        }

        // 映射MinMax值到正方形区域的局部坐标
        float localMinX = Mathf.InverseLerp(attribute.MinX, attribute.MaxX, value.min.x) * squareRect.width;
        float localMinY = Mathf.InverseLerp(attribute.MinY, attribute.MaxY, value.min.y) * squareRect.height;
        float localMaxX = Mathf.InverseLerp(attribute.MinX, attribute.MaxX, value.max.x) * squareRect.width;
        float localMaxY = Mathf.InverseLerp(attribute.MinY, attribute.MaxY, value.max.y) * squareRect.height;

        // 创建绿色矩形线框的Rect
        Rect greenRect = new Rect(
            squareRect.x + localMinX,
            squareRect.y + (squareRect.height - localMaxY),
            localMaxX - localMinX,
            localMaxY - localMinY
        );

        // 绘制绿色矩形线框
        Handles.DrawSolidRectangleWithOutline(greenRect, Color.clear, Color.green);

        // 确保调用CallNextDrawer来继续绘制链中的其他绘制器
        CallNextDrawer(label);
    }
}
#endif