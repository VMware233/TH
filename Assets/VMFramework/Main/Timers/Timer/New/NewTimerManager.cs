using System.Runtime.CompilerServices;
using UnityEngine;
using VMFramework.Core;
using VMFramework.Procedure;

namespace VMFramework.NewTimer
{
    [ManagerCreationProvider(ManagerType.EventCore)]
    public sealed partial class NewTimerManager : ManagerBehaviour<NewTimerManager>
    {
        private const int INITIAL_QUEUE_SIZE = 100;
        private const int QUEUE_SIZE_GAP = 50;

        private static readonly GenericPriorityQueue<NewITimer, double> queue = new(INITIAL_QUEUE_SIZE);

        private static double currentTime = 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Add(NewITimer timer, float delay)
        {
            int capacity = queue.capacity;
            if (queue.count >= capacity)
            {
                queue.Resize(capacity + QUEUE_SIZE_GAP);
            }

            timer.Delay = delay;

            queue.Enqueue(timer, currentTime + delay);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Stop(NewITimer timer)
        {
            timer.ElapsedTime = currentTime;

            queue.Remove(timer);

            timer.OnStopped();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Restart(NewITimer timer, float delay)
        {
            Add(timer, (float)(delay - timer.ElapsedTime));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains(NewITimer timer)
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

                NewITimer timer = queue.Dequeue();

                timer.OnTimed();

                if (timer.Repetition > 0)
                {
                    timer.Repetition--;
                    Add(timer, timer.Delay);
                }
            }
        }
    }
}