namespace VMFramework.UI
{
    public interface ITooltip : IUIPanelController
    {
        public void Open(ITooltipProvider tooltipProvider, IUIPanelController source);

        public void Close(ITooltipProvider tooltipProvider);
    }
}