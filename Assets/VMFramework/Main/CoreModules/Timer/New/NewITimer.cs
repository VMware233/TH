using VMFramework.Core;

namespace VMFramework.NewTimer
{
    public interface NewITimer : IGenericPriorityQueueNode<double>
    {
        public float Delay { get; set; }

        public int Repetition { get; set; }

        public double ElapsedTime { get; set; }

        public void OnTimed();

        public void OnStopped();
    }
}