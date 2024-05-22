using VMFramework.Core;

namespace VMFramework.Timer
{
    public interface ITimer : IGenericPriorityQueueNode<double>
    {
        public void OnTimed();

        public void OnStopped();
    }
}