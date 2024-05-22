using VMFramework.Core;

namespace VMFramework.Timer
{
    public interface ITimer : IGenericPriorityQueueNode<double>
    {
        public void OnStart(double startTime, double expectedTime);
        
        public void OnTimed();

        public void OnStopped(double stoppedTime);
    }
}