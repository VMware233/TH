using UnityEngine;

namespace VMFramework.UI
{
    internal sealed class TracingInfo
    {
        public TracingType tracingType { get; private set; }
        public Vector3 tracingWorldPosition { get; private set; }
        public Transform tracingTransform { get; private set; }
        public bool hasMaxTracingCount { get; private set; }
        public int tracingCount { get; private set; }
        public int maxTracingCount { get; private set; }

        public void Set(TracingConfig config)
        {
            tracingType = config.tracingType;
            tracingWorldPosition = config.tracingWorldPosition;
            tracingTransform = config.tracingTransform;
            hasMaxTracingCount = config.hasMaxTracingCount;
            tracingCount = 0;
            maxTracingCount = config.maxTracingCount;
        }
        
        public void IncreaseTracingCount() => tracingCount++;
    }
}