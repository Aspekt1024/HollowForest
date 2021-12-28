using System;
using System.Collections.Generic;
using UnityEngine;

namespace HollowForest
{
    public class AttackCollider : MonoBehaviour
    {
        private Collider2D coll;
        private Character owner;

        private void Awake()
        {
            owner = GetComponentInParent<Character>();
            coll = GetComponent<Collider2D>();
        }

        public void ActionAttack(Action<Character, Vector3> onHitCallback)
        {
            var contacts = new List<Collider2D>();
            coll.GetContacts(contacts);
            foreach (var contact in contacts)
            {
                var character = contact.GetComponent<Character>();
                if (character != null && character != owner)
                {
                    var hitDirection = contact.transform.position - transform.position;
                    onHitCallback?.Invoke(character, hitDirection);
                }
            }
        }
    }
}