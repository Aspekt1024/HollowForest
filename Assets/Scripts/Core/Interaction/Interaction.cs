
namespace HollowForest.Interactivity
{
    public class Interaction
    {
        private readonly Character character;

        private IInteractive lastInteractive;

        public Interaction(Character character)
        {
            this.character = character;
        }

        public void Interact()
        {
            lastInteractive?.OnInteract(character);
        }

        public void SetInteractive(IInteractive interactive)
        {
            lastInteractive?.OnOverlapEnd(character);
            lastInteractive = interactive;
            interactive.OnOverlap(character);
        }

        public void UnsetInteractive(IInteractive interactive)
        {
            if (interactive != lastInteractive) return;
            
            interactive.OnOverlapEnd(character);
            lastInteractive = null;
        }
    }
}