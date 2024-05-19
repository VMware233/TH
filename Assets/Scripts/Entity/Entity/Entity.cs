using System;
using VMFramework.GameLogicArchitecture;
using FishNet.Serializing;
using UnityEngine;
using VMFramework.Core;
using Sirenix.OdinInspector;
using FishNet.Connection;
using VMFramework.Network;
using VMFramework.UI;

namespace TH.Entities
{
    public partial class Entity : VisualGameItem, IUUIDOwner, ITracingTooltipProvider
    {
        #region Properties

        protected EntityConfig entityConfig => (EntityConfig)gamePrefab;

        public GameObject prefab => entityConfig.prefab;

        #endregion

        #region UUID Owner

        public string uuid { get; private set; }

        bool IUUIDOwner.isDirty
        {
            get => false;
            set { }
        }

        public event Action<IUUIDOwner, bool, NetworkConnection> OnObservedEvent;
        public event Action<IUUIDOwner, NetworkConnection> OnUnobservedEvent;

        void IUUIDOwner.OnObserved(bool isDirty, NetworkConnection connection)
        {
            OnObservedEvent?.Invoke(this, isDirty, connection);
        }

        void IUUIDOwner.OnUnobserved(NetworkConnection connection)
        {
            OnUnobservedEvent?.Invoke(this, connection);
        }

        #endregion

        [ShowInInspector]
        public EntityController controller { get; private set; }

        public Transform transform => controller.transform;

        #region Init

        protected override void OnCreate()
        {
            base.OnCreate();

            isDestroyed = false;
        }

        public void Init(EntityController controller)
        {
            this.controller = controller;

            if (isServer)
            {
                if (uuid.IsNullOrEmpty() == false)
                {
                    Debug.LogWarning($"{this}已有UUID");
                }

                uuid = Guid.NewGuid().ToString();
            }

            EntityManager.Register(this);

            OnInit();
        }

        protected virtual void OnInit()
        {
            
        }

        #endregion

        #region Destroy

        public bool isDestroyed { get; private set; }

        public void Destroy()
        {
            OnDestroy();

            isDestroyed = true;
        }

        protected virtual void OnDestroy()
        {
            
        }

        #endregion

        #region Network Serialization

        protected override void OnWrite(Writer writer)
        {
            base.OnWrite(writer);

            writer.WriteString(uuid);
        }

        protected override void OnRead(Reader reader)
        {
            base.OnRead(reader);

            uuid = reader.ReadString();
        }

        #endregion
    }
}
