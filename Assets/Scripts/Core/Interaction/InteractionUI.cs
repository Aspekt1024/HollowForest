using HollowForest.Interactivity;
using HollowForest.UI;
using TMPro;
using UnityEngine;

namespace HollowForest
{
    public class InteractionUI : UIBase
    {
        public TextMeshProUGUI mainText;
        public TextMeshProUGUI subText;
        public GameObject ui;

        private Transform target;

        public void SetupAndShow(IInteractive interactive)
        {
            var details = interactive.GetOverlayDetails();
            if (details.isHidden) return;
            
            mainText.text = details.mainText;
            subText.text = details.subText;
            target = details.indicatorPos;
            
            Show();
        }

        private void Update()
        {
            if (target != null)
            {
                transform.localScale = Mathf.Abs(Game.Camera.mainCamera.transform.position.z) * Vector3.one;
                transform.position = target.position;
            }
        }

        protected override void OnAwake()
        {
        }
        
        public override void OnAcceptPressed()
        {
        }

        protected override bool OnShow()
        {
            ui.SetActive(true);
            return true;
        }

        protected override bool OnHide()
        {
            ui.SetActive(false);
            target = null;
            return true;
        }

        protected override void OnHideImmediate()
        {
            ui.SetActive(false);
        }
    }
}