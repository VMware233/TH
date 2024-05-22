using System;
using System.Runtime.CompilerServices;
using VMFramework.Core;

namespace VMFramework.Timer
{
    public class Timer : ITimer
    {
        double IGenericPriorityQueueNode<double>.Priority { get; set; }

        int IGenericPriorityQueueNode<double>.QueueIndex { get; set; }

        long IGenericPriorityQueueNode<double>.InsertionIndex { get; set; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void ITimer.OnTimed()
        {
            _onTimed?.Invoke();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void ITimer.OnStopped()
        {
            _onStopped?.Invoke();
        }
        
        private readonly Action _onTimed;
        private readonly Action _onStopped;

        public Timer(Action onTimed, Action onStopped = null)
        {
            _onTimed = onTimed;
            _onStopped = onStopped;
        }
    }
}