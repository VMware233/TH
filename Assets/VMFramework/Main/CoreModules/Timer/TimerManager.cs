using System.Runtime.CompilerServices;
using UnityEngine;
using VMFramework.Core;
using VMFramework.Procedure;

namespace VMFramework.Timer
{
    [ManagerCreationProvider(ManagerType.EventCore)]
    public sealed partial class TimerManager : ManagerBehaviour<TimerManager>
    {
        private const int INITIAL_QUEUE_SIZE = 100;
        private const int QUEUE_SIZE_GAP = 50;
        
        private static readonly GenericPriorityQueue<ITimer, double> queue = new(INITIAL_QUEUE_SIZE);
        
        private static double currentTime = 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Add(ITimer timer, float delay)
        {
            int capacity = queue.capacity;
            if (queue.count >= capacity)
            {
                queue.Resize(capacity + QUEUE_SIZE_GAP);
            }
            
            queue.Enqueue(timer, currentTime + delay);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Stop(ITimer timer)
        {
            queue.Remove(timer);
            
            timer.OnStopped();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains(ITimer timer)
        {
            return queue.Contains(timer);
        }

        private void Update()
        {
            currentTime += Time.deltaTime;

            while (queue.count > 0)
            {
                if (currentTime < queue.first.Priority)
                {
                    break;
                }
                
                ITimer timer = queue.Dequeue();
                
                timer.OnTimed();
            }
        }
    }
}