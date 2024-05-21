using System;
using Sirenix.OdinInspector;
using VMFramework.Configuration;
using VMFramework.GameLogicArchitecture;
using VMFramework.OdinExtensions;

namespace TH.Map
{
    public class RoomGenerationProcedureUnit : BaseConfig
    {
        [LabelText("房间种类")]
        [GameTypeID]
        [IsNotNullOrEmpty]
        public string roomGameTypeID;

        public override string ToString()
        {
            return GameTypeNameUtility.GetGameTypeName(roomGameTypeID);
        }
    }
}