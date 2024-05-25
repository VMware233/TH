using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sirenix.OdinInspector;
using UnityEngine;
using VMFramework.Core;
using VMFramework.Procedure;

namespace VMFramework.UI
{
    [ManagerCreationProvider(ManagerType.UICore)]
    public sealed partial class TracingUIManager : ManagerBehaviour<TracingUIManager>, IManagerBehaviour
    {
        [ShowInInspector]
        private static readonly Dictionary<Transform, List<ITracingUIPanel>> tracingTransforms = new();

        [ShowInInspector]
        private static readonly HashSet<ITracingUIPanel> tracingMousePositionUIPanels = new();

        [ShowInInspector]
        private static readonly Dictionary<ITracingUIPanel, Vector3> tracingPositions = new();

        [ShowInInspector]
        private static readonly Dictionary<ITracingUIPanel, TracingInfo> allTracingInfos = new();

        private static readonly List<ITracingUIPanel> tracingUIPanelsToRemove = new();

        [ShowInInspector]
        private new static Camera camera;

        #region Init

        void IInitializer.OnPostInit(Action onDone)
        {
            camera = CameraManager.mainCamera;
            onDone();
        }

        #endregion

        #region TracingInfo

        [ShowInInspector]
        private static readonly Stack<TracingInfo> infoCache = new();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static TracingInfo GetTracingInfoFromCache()
        {
            if (infoCache.Count == 0)
            {
                return new TracingInfo();
            }
            
            return infoCache.Pop();
        }

        #endregion

        #region Start/Stop Tracing

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void StartTracing(ITracingUIPanel tracingUIPanel, TracingConfig tracingConfig)
        {
            if (tracingConfig is { hasMaxTracingCount: true, maxTracingCount: <= 0 })
            {
                return;
            }

            if (allTracingInfos.TryGetValue(tracingUIPanel, out var tracingInfo))
            {
                tracingInfo.Set(tracingConfig);
                return;
            }
            
            tracingInfo = GetTracingInfoFromCache();
            tracingInfo.Set(tracingConfig);
            
            allTracingInfos.Add(tracingUIPanel, tracingInfo);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void StopTracing(ITracingUIPanel tracingUIPanel)
        {
            if (allTracingInfos.Remove(tracingUIPanel, out var tracingInfo))
            {
                infoCache.Push(tracingInfo);
            }
        }

        #endregion

        #region Update

        private void Update()
        {
            var mousePosition = Input.mousePosition.To2D();

            foreach (var (panel, info) in allTracingInfos)
            {
                Vector2 targetScreenPosition = info.tracingType switch
                {
                    TracingType.MousePosition => mousePosition,
                    TracingType.WorldPosition => camera.WorldToScreenPoint(info.tracingWorldPosition).To2D(),
                    TracingType.Transform => camera.WorldToScreenPoint(info.tracingTransform.position).To2D(),
                    _ => throw new ArgumentOutOfRangeException(nameof(info.tracingType))
                };

                if (panel.TryUpdatePosition(targetScreenPosition))
                {
                    if (info.hasMaxTracingCount)
                    {
                        info.IncreaseTracingCount();

                        if (info.tracingCount >= info.maxTracingCount)
                        {
                            tracingUIPanelsToRemove.Add(panel);
                        }
                    }
                }
            }

            if (tracingUIPanelsToRemove.Count > 0)
            {
                foreach (var tracingUIPanel in tracingUIPanelsToRemove)
                {
                    StopTracing(tracingUIPanel);
                }

                tracingUIPanelsToRemove.Clear();
            }
        }

        #endregion

        #region Set Camera

        [Button]
        public static void SetCamera(Camera camera)
        {
            TracingUIManager.camera = camera;
        }

        #endregion
    }
}