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

        [ShowInInspector]
        public EntityController controller { get; private set; }

        public Transform transform => controller.transform;

        #region Init

        public void Init(EntityController controller)
        {
            this.controller = controller;

            OnInit();
        }

        protected virtual void OnInit()
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
