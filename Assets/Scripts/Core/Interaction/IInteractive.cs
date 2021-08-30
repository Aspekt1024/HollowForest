namespace HollowForest.Interactivity
{
    public interface IInteractive
    {
        void OnInteract(Character character);
        void OnOverlap(Character character);
        void OnOverlapEnd(Character character);
    }
}