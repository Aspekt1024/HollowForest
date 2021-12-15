using System;
using UnityEngine;

namespace HollowForest
{
    public class AnimationEventListener : MonoBehaviour
    {
        public Action Attacked;
        
        public void AttackBegin()
        {
            Attacked?.Invoke();
        }
    }
}