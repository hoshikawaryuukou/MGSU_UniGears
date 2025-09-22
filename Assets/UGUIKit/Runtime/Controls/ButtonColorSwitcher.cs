using Alchemy.Inspector;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace UniGears.UGUIKit.Controls
{
    [RequireComponent(typeof(Button))]
    public sealed class ButtonColorSwitcher : MonoBehaviour
    {
        [SerializeField] private Image buttonImage;
        [SerializeField] private TextMeshProUGUI buttonText;

        [Title("Normal Colors")]
        [SerializeField] private Color normalImageColor = Color.white;
        [SerializeField] private Color normalTextColor = Color.black;

        [Title("Selected Colors")]
        [SerializeField] private Color selectedImageColor = new(0.2f, 0.6f, 1f); 
        [SerializeField] private Color selectedTextColor = Color.white;

        void Reset()
        {
            if (buttonImage == null)
            {
                buttonImage = GetComponent<Image>();
            }

            if (buttonText == null)
            {
                buttonText = GetComponentInChildren<TextMeshProUGUI>();
            }
        }

        public void SetSelected(bool isSelected)
        {
            if (isSelected)
            {
                SetSelectedState();
            }
            else
            {
                SetNormalState();
            }
        }

        public void SetSelectedState()
        {
            if (buttonImage != null)
            {
                buttonImage.color = selectedImageColor;
            }

            if (buttonText != null)
            {
                buttonText.color = selectedTextColor;
            }
        }

        public void SetNormalState()
        {
            if (buttonImage != null)
            {
                buttonImage.color = normalImageColor;
            }

            if (buttonText != null)
            {
                buttonText.color = normalTextColor;
            }
        }


# if UNITY_EDITOR
        [Button] 
        private void _SetNormalState() 
        {
            SetNormalState() ; 
            EditorUtility.SetDirty(gameObject);
        }

        [Button] 
        private void _SetSelectedState() 
        { 
            SetSelectedState(); ; 
            EditorUtility.SetDirty(gameObject);
        }
#endif
    }
}
