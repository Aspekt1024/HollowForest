using UnityEngine;

namespace HollowForest.World
{
    public class SpawnPoint : MonoBehaviour
    {
        public bool isDefaultSpawnPoint;

        public void SetAtSpawnPoint(Character character)
        {
            character.Physics.SetOnGround(transform.position);
        }
    }
}