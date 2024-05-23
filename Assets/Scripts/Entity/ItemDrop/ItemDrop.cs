using System.Collections.Generic;
using VMFramework.UI;
using FishNet.Serializing;
using TH.Items;
using UnityEngine;
using VMFramework.Core;
using VMFramework.Procedure;
using VMFramework.Timers;

namespace TH.Entities
{
    public class ItemDrop : Entity, ITooltipProvider
    {
        public new ItemDropController controller =>
            base.controller as ItemDropController;

        public ItemDropConfig itemDropConfig => (ItemDropConfig)gamePrefab;

        public Item item { get; private set; }

        public void SetItem(Item newItem)
        {
            if (newItem == null && base.controller != null)
            {
                EntityManager.DestroyEntity(this);
                return;
            }

            if (item != null)
            {
                Debug.LogWarning($"重复设置{nameof(ItemDrop)}的{nameof(Item)}");
            }

            item = newItem;
        }

        #region Init

        protected override void OnCreate()
        {
            base.OnCreate();

            lifeTime = 0f;
        }

        protected override void OnInit()
        {
            base.OnInit();

            if (item is IItemDropProvider provider)
            {
                controller.SetIcon(provider.GetItemDropIcon());
            }

            if (isServer)
            {
                UpdateDelegateManager.AddUpdateDelegate(UpdateType.Update, OnUpdate);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (isServer)
            {
                UpdateDelegateManager.RemoveUpdateDelegate(UpdateType.Update, OnUpdate);
            }
        }

        #endregion

        #region Update

        private float lifeTime = 0f;

        private void OnUpdate()
        {
            lifeTime += Time.deltaTime;

            if (lifeTime > itemDropConfig.maxLifeTime)
            {
                EntityManager.DestroyEntity(this);
            }
        }

        #endregion

        #region Net Serializer

        protected override void OnWrite(Writer writer)
        {
            base.OnWrite(writer);

            writer.WriteItem(item);
        }

        protected override void OnRead(Reader reader)
        {
            base.OnRead(reader);

            item = reader.ReadItem();
        }

        #endregion

        #region ITracing Tooltip Provider

        public override string GetTooltipTitle()
        {
            return item?.GetTooltipTitle();
        }

        public override string GetTooltipDescription()
        {
            return item?.GetTooltipDescription();
        }

        public override IEnumerable<TooltipPropertyInfo> GetTooltipProperties()
        {
            return item?.GetTooltipProperties();
        }

        #endregion
    }
}
