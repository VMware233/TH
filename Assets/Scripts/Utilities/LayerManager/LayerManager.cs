using System.Runtime.CompilerServices;
using TH.GameCore;
using UnityEngine;

namespace TH.Utilities
{
    public static class LayerManager
    {
        public static int entityLayer
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => GameSetting.entityGeneralSetting.entityLayer;
        }

        public static int projectileLayer
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => GameSetting.projectileGeneralSetting.projectileLayer;
        }

        public static int playerLayer
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => GameSetting.playerGeneralSetting.playerLayer;
        }

        public static int floorLayer
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => GameSetting.floorGeneralSetting.floorLayer;
        }
        
        public static LayerMask entityLayerMask { get; private set; }
        
        public static LayerMask projectileLayerMask { get; private set; }
        
        public static LayerMask playerLayerMask { get; private set; }
        
        public static LayerMask allEntityLayerMask { get; private set; }
        
        public static LayerMask floorLayerMask { get; private set; }

        public static void Init()
        {
            entityLayerMask = entityLayer.ToLayerMask();
            projectileLayerMask = projectileLayer.ToLayerMask();
            playerLayerMask = playerLayer.ToLayerMask();
            
            allEntityLayerMask = entityLayerMask | projectileLayerMask | playerLayerMask;
            
            floorLayerMask = floorLayer.ToLayerMask();
        }
    }
}