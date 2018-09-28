using UI.PageTransitions.Title;
using UI.Scripts.Route;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views.SubPage
{
	public class PauseSubPage : MonoBehaviour {

		[SerializeField]
		private Button _resumeButton;
	
		[SerializeField]
		private Button _exitButton;

		[SerializeField] private GameObject _currentPage;
	
		// Use this for initialization
		void Start ()
		{
			_resumeButton.OnClickAsObservable().Subscribe(_ =>
			{
				Time.timeScale = 1f;
				_currentPage.SetActive(false);
			}).AddTo(this);

			_exitButton.OnClickAsObservable().Subscribe(_ =>
			{
				Time.timeScale = 1f;
				PageRouter.Instance.DoTransition<TitleTransition>(true);
			}).AddTo(this);
		}
	

	}
}
