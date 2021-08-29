
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
            lastInteractive?.OnUntargeted();
            lastInteractive = interactive;
            interactive.OnTargeted();
        }

        public void UnsetInteractive(IInteractive interactive)
        {
            if (interactive != lastInteractive) return;
            
            interactive.OnUntargeted();
            lastInteractive = null;
        }
    }
}