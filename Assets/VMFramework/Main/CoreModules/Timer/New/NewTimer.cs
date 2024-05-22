using System;
using System.Runtime.CompilerServices;
using VMFramework.Core;

namespace VMFramework.NewTimer
{
    public class NewTimer : NewITimer
    {
        double IGenericPriorityQueueNode<double>.Priority { get; set; }

        int IGenericPriorityQueueNode<double>.QueueIndex { get; set; }

        long IGenericPriorityQueueNode<double>.InsertionIndex { get; set; }

        int repetition;
        public int Repetition { get => repetition; set => repetition = value; }

        double elapsedTime;
        public double ElapsedTime { get => elapsedTime; set => elapsedTime = value; }

        private float delay { get; set; }
        public float Delay { get => delay; set => delay = value; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void NewITimer.OnTimed()
        {
            _onTimed?.Invoke();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void NewITimer.OnStopped()
        {
            _onStopped?.Invoke();
        }

        private readonly Action _onTimed;
        private readonly Action _onStopped;

        public NewTimer(Action onTimed, Action onStopped = null)
        {
            _onTimed = onTimed;
            _onStopped = onStopped;
        }

        public NewTimer(Action onTimed, float delay, int repetition = 0, Action onStopped = null)
        {
            _onTimed = onTimed;
            _onStopped = onStopped;

            Delay = delay;
            Repetition = repetition;
        }
    }
}