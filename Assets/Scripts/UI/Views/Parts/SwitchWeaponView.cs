using System.Collections.Generic;
using System.Linq;
using BattleStage.Domain;
using UI.Views.Parts.Buttons;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views.Parts
{
    public class SwitchWeaponView : MonoBehaviour
    {
        public SelectWeaponButton[] ButtonWeapon;

        public Image[] ImageButtonBackground;

        private ISubject<int> _selectWeaponIndex;

        void Start()
        {
            foreach (var image in ImageButtonBackground)
            {
                image.alphaHitTestMinimumThreshold = 0.5f;
            }
        }

        public void ShowNumberOfWeaponEnabled(List<Weapon> weapons)
        {
            foreach (var buttonScript in ButtonWeapon)
            {
                buttonScript.Button.interactable = weapons.Any(d => d.ID.Value == buttonScript.WeaponID);
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