using VMFramework.GameLogicArchitecture;
using FishNet.Serializing;
using UnityEngine;
using Sirenix.OdinInspector;

namespace TH.Entities
{
    public partial class Entity : VisualGameItem
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
