using UnityEngine;

namespace HollowForest.AI
{
    public abstract class AISensor : MonoBehaviour
    {
        protected AIAgent agent;
        
        public void Init(AIAgent agent)
        {
            this.agent = agent;
            OnInit();
        }

        protected abstract void OnInit();
    }
}