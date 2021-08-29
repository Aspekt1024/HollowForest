namespace HollowForest.Interactivity
{
    public interface IInteractive
    {
        void OnInteract(Character character);
        void OnTargeted();
        void OnUntargeted();
    }
}