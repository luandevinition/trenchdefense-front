using System.Collections.Generic;
using System.Linq;
using BattleStage.Controller;
using Domain.Wave;
using UI.ViewModels.Pages.Battle;
using UI.Views.Parts;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views.Pages.Battle
{
	public class BattlePageView : MonoBehaviour
	{
		[SerializeField]
		private BattleController BattleController;
		
		[SerializeField]
		private Button _pauseButton;
		
		[SerializeField]
		public Button _SwitchWeaponButton;
		
		[SerializeField]
		private GameObject _pauseSubPageGameObject;
		
		[SerializeField]
		private GameObject _switchPartGameObject;

		[SerializeField]
		private SwitchWeaponView _switchWeaponView;

		private bool isOpenSwitchWeapon = false;

		private readonly Subject<int> _selectWeaponIndex= new Subject<int>();
		
		public void Bind(IBattlePageViewModel viewModel)
		{
			_switchWeaponView.Bind(_selectWeaponIndex);

			BattleController.InitData(viewModel, _selectWeaponIndex);

			_switchWeaponView.ShowNumberOfWeaponEnabled(viewModel.Weapons.ToList());

			viewModel.Weapons.ObserveEveryValueChanged(data => data.ToList()).Subscribe(weapons =>
			{
				_switchWeaponView.ShowNumberOfWeaponEnabled(weapons);
			}).AddTo(this);
			
			_pauseButton.OnClickAsObservable().Subscribe(_ =>
			{
				Time.timeScale = 0f;
				_pauseSubPageGameObject.SetActive(true);
			}).AddTo(this);
			
			_SwitchWeaponButton.OnClickAsObservable().Subscribe(_ =>
			{
				if (isOpenSwitchWeapon)
					isOpenSwitchWeapon = false;
				else
					isOpenSwitchWeapon = true;
				_switchPartGameObject.SetActive(isOpenSwitchWeapon);
			}).AddTo(this);
		}
		
		void OnApplicationPause(bool pauseStatus)
		{
			if (pauseStatus && Mathf.RoundToInt(Time.timeScale) == 1)
			{
				Time.timeScale = 0f;
				_pauseSubPageGameObject.SetActive(true);
			}
		}
	}
}
