namespace RayWallSystem
{
    public interface IHandleInstantiation
    {
        public void HandleInstantiation();
        public bool ClickAllowed();

        public void Deactivate();
        public void Activate();
    }
}