﻿using System.Collections.Generic;
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

		private readonly Subject<int> _selectWeaponIndex= new Subject<int>();
		
		public void Bind(IBattlePageViewModel viewModel)
		{
			_switchWeaponView.Bind(_selectWeaponIndex);

			BattleController.InitData(viewModel, _selectWeaponIndex);

			viewModel.Weapons.ToReactiveCollection().ObserveCountChanged(true).Subscribe(newCount =>
			{
				_switchWeaponView.ShowNumberOfWeaponEnabled(newCount);
			}).AddTo(this);
			
			_pauseButton.OnClickAsObservable().Subscribe(_ =>
			{
				Time.timeScale = 0f;
				_pauseSubPageGameObject.SetActive(true);
			}).AddTo(this);
			
			_pauseButton.OnClickAsObservable().Subscribe(_ =>
			{
				_switchPartGameObject.SetActive(true);
			}).AddTo(this);
		}
		
		void OnApplicationPause(bool pauseStatus)
		{
			if (pauseStatus && ((int)Time.timeScale) != 0)
			{
				Time.timeScale = 0f;
				_pauseSubPageGameObject.SetActive(true);
			}
		}
	}
}
