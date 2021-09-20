using UnityEngine;

namespace HollowForest.World
{
    public class Surface : MonoBehaviour
    {
        public bool isSticky;
        public bool isBouncy;

        private Character interactingCharacter;

        public void OnCollision(Character character)
        {
            interactingCharacter = character;
        }

        public void Release(Character character)
        {
            if (interactingCharacter == character)
            {
                interactingCharacter = null;
            }
        }
    }
}