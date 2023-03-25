using System;
using UnityEngine;
using UnityEngine.UI;

namespace KOI {
    public class FollowMeCameraToggle : MonoBehaviour
    {
        private Toggle toggle;
        private Image toggleImage;
        // private ToolTipComponent tooltip;
        
        public static event EventHandler<OnFollowEntityArgs> EnableFollowEntity;
        public static event EventHandler<OnFollowEntityArgs> DisableFollowEntity;
        
        private void Awake() {
            if(!toggle) toggle = GetComponent<Toggle>();
            if(!toggleImage) toggleImage = GetComponent<Image>();

            PlayerInterface.OnCloseEntityInfoPanel += ResetToggle;
            // tooltip = gameObject.transform.GetChild(0).GetComponent<ToolTipComponent>();
        }

        private void OnEnable()
        {
            toggle.onValueChanged.AddListener(ToggleValueChanged);
            // tooltip.text = "Stop Following";
        }

        private void OnDisable()
        {
            toggle.onValueChanged.RemoveListener(ToggleValueChanged);
            // tooltip.text = "Follow Me";
        }

        private void ToggleValueChanged(bool isOn)
        {
            if (isOn)
            {
                toggleImage.color = new Color(1f, 1f, 1f, 1f);
                Debug.Log("Toggle is On");
                GameObject entity = PlayerInterface.SelectedEntityObject;
                EnableFollowEntity?.Invoke(this, new OnFollowEntityArgs { Entity = entity });
            }
            else
            {
                toggleImage.color = new Color(0.5f, 0.5f, 0.5f, 1f);
                Debug.Log("Toggle is Off");
                DisableFollowEntity?.Invoke(this, null);
            }
        }

        private void ResetToggle(object sender, OnFollowEntityArgs eventArgs) {
            toggle.isOn = false;
        }
    }
}