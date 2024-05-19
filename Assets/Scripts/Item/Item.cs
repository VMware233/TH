using VMFramework.GameLogicArchitecture;
using VMFramework.UI;
using FishNet.Serializing;
using TH.Entities;
using UnityEngine;
using UnityEngine.UIElements;
using VMFramework.Containers;
using VMFramework.Property;

namespace TH.Items
{
    public class Item : ContainerItem, ISlotProvider, IItemDropProvider
    {
        protected ItemPreset itemPreset => (ItemPreset)gamePrefab;
        
        public override int maxStackCount =>
            itemPreset.finiteMaxStackCount ? itemPreset.maxStackCount : int.MaxValue;

        #region Init

        protected override void OnCreate()
        {
            base.OnCreate();

            count = new BaseIntProperty(1);
        }

        #endregion

        #region ISlot Provider

        StyleBackground ISlotProvider.GetIconImage()
        {
            return new StyleBackground(itemPreset.icon);
        }

        string ISlotProvider.GetDescriptionText()
        {
            if (count == 1)
            {
                return "";
            }

            return count.ToString();
        }

        void ISlotProvider.HandleMouseEnterEvent(
            UIPanelController source)
        {
            if (isDebugging)
            {
                Debug.LogWarning($"{this}被鼠标进入");
            }

            TracingTooltipManager.Open(this, source);
        }

        void ISlotProvider.HandleMouseLeaveEvent(
            UIPanelController source)
        {
            if (isDebugging)
            {
                Debug.LogWarning($"{this}被鼠标退出");
            }

            TracingTooltipManager.Close(this);
        }

        #endregion

        #region IItem Drop Provider

        Sprite IItemDropProvider.GetItemDropIcon() => itemPreset.icon;

        #endregion

        #region Net Serialization

        protected override void OnWrite(Writer writer)
        {
            base.OnWrite(writer);
            
            writer.WriteInt32(count.value);
        }

        protected override void OnRead(Reader reader)
        {
            base.OnRead(reader);

            count.value = reader.ReadInt32();
        }

        #endregion

        #region Create

        public static Item Create(string id, int count)
        {
            var newItem = IGameItem.Create<Item>(id);

            newItem.count.value = count;

            return newItem;
        }

        public static TItem Create<TItem>(string id, int count) where TItem : Item
        {
            var newItem = IGameItem.Create<TItem>(id);

            newItem.count.value = count;

            return newItem;
        }

        #endregion
    }
}