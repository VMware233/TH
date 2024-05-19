using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using VMFramework.Core;

namespace TH.Map
{
    [HideReferenceObjectPicker]
        [HideDuplicateReferenceBox]

    public class RoomGatewayInfo
    {
        [ShowInInspector]
        public Vector2Int relativeUnitPos { get; init; }

        [ShowInInspector]
        public FourTypesDirection2D gatewayDirection { get; init; }

        [ShowInInspector]
        public IReadOnlyList<Vector2Int> poses { get; init; }

        [ShowInInspector]
        public ISet<int> indices { get; init; }
    }
}