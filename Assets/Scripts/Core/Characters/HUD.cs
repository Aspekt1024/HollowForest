using System.Collections.Generic;
using HollowForest.Combat;
using UnityEngine;
using UnityEngine.UI;

namespace HollowForest.UI
{
    public class HUD : UIBase
    {
        public GameObject healthContainer;
        public Image healthPrefab;

        private readonly List<Image> healthIcons = new List<Image>();

        public void SetupCharacter(Character character)
        {
            SetHealth(character.Health.CurrentHealth);
            character.Health.HealthModified += SetHealth;
        }

        private void SetHealth(int health)
        {
            health = Mathf.Max(health, 0);
            for (int i = 0; i < health; i++)
            {
                if (i >= healthIcons.Count)
                {
                    var icon = Instantiate(healthPrefab, healthContainer.transform);
                    healthIcons.Add(icon);
                }
                else
                {
                    healthIcons[i].gameObject.SetActive(true);
                }
            }

            for (int i = health; i < healthIcons.Count; i++)
            {
                healthIcons[i].gameObject.SetActive(false);
            }
        }
        
        public override void OnAcceptPressed()
        {
        }

        protected override bool OnShow()
        {
            return true;
        }

        protected override bool OnHide()
        {
            return true;
        }

        protected override void OnHideImmediate()
        {
        }

        protected override void OnAwake()
        {
        }
    }
}