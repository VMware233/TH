﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace VMFramework.UI
{
    public partial class TracingUIManager
    {
        #region Debug Utility

        private static void WarningAlreadyTracing(ITracingUIPanel tracingUIPanel, TracingType tracingType)
        {
            if (tracingType == TracingType.Transform)
            {
                Debug.LogWarning($"{tracingUIPanel} already tracing a transform");
            }
            else if (tracingType == TracingType.MousePosition)
            {
                Debug.LogWarning($"{tracingUIPanel} already tracing mouse position");
            }
        }

        #endregion

        #region Start Tracing

        public static void StartTracingPosition(ITracingUIPanel tracingUIPanel, Vector3 position,
            int maxTracingCount = int.MaxValue)
        {
            if (allTracingConfigs.TryGetValue(tracingUIPanel, out var existedConfig))
            {
                if (existedConfig.tracingType != TracingType.Position)
                {
                    WarningAlreadyTracing(tracingUIPanel, existedConfig.tracingType);
                    return;
                }
            }

            tracingPositions[tracingUIPanel] = position;
            allTracingConfigs[tracingUIPanel] = new TracingConfig
            {
                tracingType = TracingType.Position,
                tracingPosition = position,
                maxTracingCount = maxTracingCount
            };
        }

        public static void StartTracingMousePosition(ITracingUIPanel tracingUIPanel,
            int maxTracingCount = int.MaxValue)
        {
            if (allTracingConfigs.TryGetValue(tracingUIPanel, out var existedConfig))
            {
                if (existedConfig.tracingType != TracingType.Position)
                {
                    WarningAlreadyTracing(tracingUIPanel, existedConfig.tracingType);
                    return;
                }
            }

            tracingMousePositionUIPanels.Add(tracingUIPanel);
            allTracingConfigs[tracingUIPanel] = new TracingConfig
            {
                tracingType = TracingType.MousePosition,
                maxTracingCount = maxTracingCount
            };
        }

        public static void StartTracingMousePosition(ITracingUIPanel tracingUIPanel, bool persistentTracing)
        {
            if (persistentTracing)
            {
                StartTracingMousePosition(tracingUIPanel);
            }
            else
            {
                StartTracingMousePosition(tracingUIPanel, 1);
            }
        }

        public static void StartTracingTransform(ITracingUIPanel tracingUIPanel, Transform target,
            int maxTracingCount = int.MaxValue)
        {
            if (target == null)
            {
                return;
            }

            if (allTracingConfigs.TryGetValue(tracingUIPanel, out var existedConfig))
            {
                if (existedConfig.tracingType != TracingType.Position)
                {
                    WarningAlreadyTracing(tracingUIPanel, existedConfig.tracingType);
                    return;
                }
            }

            if (tracingTransforms.ContainsKey(target) == false)
            {
                tracingTransforms[target] = new List<ITracingUIPanel>();
            }

            var tracingUIPanels = tracingTransforms[target];

            tracingUIPanels.Add(tracingUIPanel);

            allTracingConfigs[tracingUIPanel] = new TracingConfig
            {
                tracingType = TracingType.Transform,
                tracingTransform = target,
                maxTracingCount = maxTracingCount
            };
        }

        public static void StartTracingTransform(ITracingUIPanel tracingUIPanel, Transform target,
            bool persistentTracing)
        {
            if (persistentTracing)
            {
                StartTracingTransform(tracingUIPanel, target);
            }
            else
            {
                StartTracingTransform(tracingUIPanel, target, 1);
            }
        }

        #endregion

        #region Stop Tracing

        public static void StopTracing(ITracingUIPanel tracingUIPanel)
        {
            if (allTracingConfigs.TryGetValue(tracingUIPanel, out var config))
            {
                switch (config.tracingType)
                {
                    case TracingType.MousePosition:
                        tracingMousePositionUIPanels.Remove(tracingUIPanel);
                        break;
                    case TracingType.Transform:
                        var tracingTransform = config.tracingTransform;

                        if (tracingTransforms.ContainsKey(tracingTransform))
                        {
                            var tracingUIPanels = tracingTransforms[tracingTransform];

                            tracingUIPanels.Remove(tracingUIPanel);

                            if (tracingUIPanels.Count == 0)
                            {
                                tracingTransforms.Remove(tracingTransform);
                            }
                        }

                        break;
                    case TracingType.Position:
                        tracingPositions.Remove(tracingUIPanel);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                allTracingConfigs.Remove(tracingUIPanel);
            }
        }

        #endregion
    }
}