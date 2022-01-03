using System;
using UnityEngine;

namespace HollowForest.Combat
{
    public class CombatAnimationHandler : MonoBehaviour
    {
        public bool IsComboAvailable { get; private set; }
        public bool CanAttack => canAttack || IsComboAvailable;
        
        public Action OnAttack;

        private bool canAttack;
        private float timeComboWindowClosed;
        private float timeAttackComplete;

        private void Awake()
        {
            canAttack = true;
            IsComboAvailable = false;
        }

        private void Update()
        {
            if (IsComboAvailable && Time.time >= timeComboWindowClosed)
            {
                IsComboAvailable = false;
            }

            if (!canAttack && Time.time >= timeAttackComplete)
            {
                canAttack = true;
            }
        }
        
        public void ComboWindowOpened(float duration)
        {
            IsComboAvailable = true;
            timeComboWindowClosed = Time.time + duration;
        }

        public void AttackBegin(float attackDuration)
        {
            canAttack = false;
            timeAttackComplete = Time.time + attackDuration;
        }

        public void AttackDamageCheck()
        {
            OnAttack?.Invoke();
        }
    }
}