using UnityEngine;

namespace VMFramework.UI
{
    public readonly struct TracingConfig
    {
        public readonly TracingType tracingType;
        public readonly Vector3 tracingWorldPosition;
        public readonly Transform tracingTransform;
        public readonly bool hasMaxTracingCount;
        public readonly int maxTracingCount;

        public TracingConfig(Vector3 worldPosition, int count = -1)
        {
            tracingType = TracingType.WorldPosition;
            tracingWorldPosition = worldPosition;
            tracingTransform = null;
            hasMaxTracingCount = count > 0;
            maxTracingCount = count;
        }
        
        public TracingConfig(Transform transform, int count = -1)
        {
            tracingType = TracingType.Transform;
            tracingWorldPosition = Vector3.zero;
            tracingTransform = transform;
            hasMaxTracingCount = count > 0;
            maxTracingCount = count;
        }

        public TracingConfig(Transform transform, bool persistentTracing)
        {
            tracingType = TracingType.Transform;
            tracingWorldPosition = Vector3.zero;
            tracingTransform = transform;
            if (persistentTracing)
            {
                hasMaxTracingCount = false;
                maxTracingCount = -1;
            }
            else
            {
                hasMaxTracingCount = true;
                maxTracingCount = 1;
            }
        }

        public TracingConfig(int count = -1)
        {
            tracingType = TracingType.MousePosition;
            tracingWorldPosition = Vector3.zero;
            tracingTransform = null;
            hasMaxTracingCount = count > 0;
            maxTracingCount = count;
        }

        public TracingConfig(bool persistentTracing)
        {
            tracingType = TracingType.MousePosition;
            tracingWorldPosition = Vector3.zero;
            tracingTransform = null;
            if (persistentTracing)
            {
                hasMaxTracingCount = false;
                maxTracingCount = -1;
            }
            else
            {
                hasMaxTracingCount = true;
                maxTracingCount = 1;
            }
        }

        public static implicit operator TracingConfig(Vector3 worldPosition)
        {
            return new(worldPosition, 1);
        }
    }
}