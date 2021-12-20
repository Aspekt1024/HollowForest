using UnityEngine;

namespace HollowForest.Interactivity
{
    public struct InteractiveOverlayDetails
    {
        public bool isHidden;
        public Transform indicatorPos;
        public string mainText;
        public string subText;

        public static InteractiveOverlayDetails None => new InteractiveOverlayDetails
        {
            isHidden = true,
        };
    }
    
    public interface IInteractive
    {
        void OnInteract(Character character);
        void OnOverlap(Character character);
        void OnOverlapEnd(Character character);
        InteractiveOverlayDetails GetOverlayDetails();
    }
}