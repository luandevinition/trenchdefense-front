using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views.Parts
{
    public class SwitchWeaponView : MonoBehaviour
    {
        public Button[] ButtonWeapon;

        public Image[] ImageButtonBackground;

        private ISubject<int> _selectWeaponIndex;

        void Start()
        {
            foreach (var image in ImageButtonBackground)
            {
                image.alphaHitTestMinimumThreshold = 0.5f;
            }
        }

        public void ShowNumberOfWeaponEnabled(int count)
        {
            for (int i = 0; i < Mathf.Min(ButtonWeapon.Length,count); i++)
            {
                ButtonWeapon[i].interactable = true;
            }
        }

        public void Bind(ISubject<int> selectWeaponIndex)
        {
            _selectWeaponIndex = selectWeaponIndex;
        }

        public void SelectButton(int indexButton)
        {
            _selectWeaponIndex.OnNext(indexButton);
            gameObject.SetActive(false);
        }
    }
}