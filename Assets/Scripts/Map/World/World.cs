using System;
using System.Collections.Generic;
using VMFramework.GameLogicArchitecture;
using FishNet.Connection;
using FishNet.Serializing;
using VMFramework.Network;

namespace TH.Map
{
    public class World : GameItem, IUUIDOwner
    {
        protected WorldPreset worldPreset => (WorldPreset)gamePrefab;

        public string uuid { get; private set; }

        public GameMapNetwork gameMapNetwork { get; private set; }

        public GameMap gameMap { get; private set; }

        public WorldGenerationRule generationRule =>
            GamePrefabManager.GetGamePrefabStrictly<WorldGenerationRule>(worldPreset.generationRuleID);

        #region Init

        public void Init(GameMapNetwork gameMapNetwork)
        {
            this.gameMapNetwork = gameMapNetwork;
            gameMap = gameMapNetwork.gameMap;
        }

        #endregion

        #region Net Serialization

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

        #region UUID Owner

        string IUUIDOwner.uuid
        {
            get => uuid;
            set => uuid = value;
        }

        bool IUUIDOwner.isDirty
        {
            get => true;
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

        public void SetUUID(string uuid)
        {
            
        }

        #endregion

        #region To String

        protected override IEnumerable<(string propertyID, string propertyContent)> OnGetStringProperties()
        {
            foreach (var property in base.OnGetStringProperties())
            {
                yield return property;
            }
            
            yield return ("UUID", uuid);
        }

        #endregion
    }
}
