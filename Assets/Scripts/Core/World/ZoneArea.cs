using HollowForest.Interactivity;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HollowForest.World
{
    public class ZoneArea : MonoBehaviour, IInteractive
    {
        [Header("Zone Area Details")]
        public ZoneAreaReference id;
        public Transform spawnPoint;
        public Transform indicatorPos;
        
        [Header("Target Details")]
        public bool isAutomatic;
        public AssetReference areaReference;
        public ZoneAreaReference targetZoneArea;

        public void OnInteract(Character character)
        {
            if (isAutomatic) return;
            TransitionArea(character);
        }

        private void TransitionArea(Character character)
        {
            character.Interaction.UnsetInteractive(this);
            
            var fadeUI = Game.UI.GetUI<Fadeout>();
            Time.timeScale = 0f;
            fadeUI.FadeComplete = isHidden =>
            {
                fadeUI.FadeComplete = null;
                Game.World.LoadArea(areaReference, area =>
                {
                    Time.timeScale = 1f;
                    Game.UI.GetUI<Fadeout>().Hide();
                    area.Setup();
                    area.SetAtSpawnPoint(character, targetZoneArea);
                });
            };
            
            fadeUI.Show();
        }

        public void OnOverlap(Character character)
        {
            if (isAutomatic)
            {
                TransitionArea(character);
            }
        }

        public void OnOverlapEnd(Character character)
        {
        }

        public InteractiveOverlayDetails GetOverlayDetails()
        {
            if (isAutomatic)
            {
                return InteractiveOverlayDetails.None;
            }
            
            return new InteractiveOverlayDetails
            {
                mainText = "Enter",
                subText = "Press [E] to Enter",
                indicatorPos = indicatorPos,
            };
        }
    }
}