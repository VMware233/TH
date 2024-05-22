﻿using System;
using System.Runtime.CompilerServices;
using VMFramework.Core;

namespace VMFramework.Timer
{
    public class Timer : ITimer
    {
        private readonly Action<Timer> _onTimed;
        private readonly Action<Timer> _onStopped;
        private double _priority;
        
        public double expectedTime => _priority;
        
        public double stoppedTime { get; private set; }
        
        public double startTime { get; private set; }

        public Timer(Action<Timer> onTimed, Action<Timer> onStopped = null)
        {
            _onTimed = onTimed;
            _onStopped = onStopped;
        }

        #region Interface Implementation

        double IGenericPriorityQueueNode<double>.Priority
        {
            get => _priority;
            set => _priority = value;
        }

        int IGenericPriorityQueueNode<double>.QueueIndex { get; set; }

        long IGenericPriorityQueueNode<double>.InsertionIndex { get; set; }

        void ITimer.OnStart(double startTime, double expectedTime)
        {
            this.startTime = startTime;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void ITimer.OnTimed()
        {
            _onTimed?.Invoke(this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void OnStopped(double stoppedTime)
        {
            this.stoppedTime = stoppedTime;
            _onStopped?.Invoke(this);
        }

        #endregion
    }
}