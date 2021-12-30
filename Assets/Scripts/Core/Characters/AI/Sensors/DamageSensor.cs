using HollowForest.Combat;
using UnityEngine;

namespace HollowForest.AI
{
    public class DamageSensor : AISensor
    {
        private bool isEnabled;
        
        protected override void OnInit()
        {
            agent.character.State.RegisterStateObserver(CharacterStates.IsAlive, OnAliveStateChanged);
            isEnabled = agent.character.State.GetState(CharacterStates.IsAlive);
        }

        private void OnAliveStateChanged(bool isAlive)
        {
            isEnabled = isAlive;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!isEnabled) return;
            
            var character = other.GetComponent<Character>();
            if (character.IsPlayer())
            {
                character.TakeDamage(new HitDetails
                {
                    damage = 1,
                    direction = character.transform.position - transform.position,
                    source = agent.character,
                });
            }
        }
    }
}