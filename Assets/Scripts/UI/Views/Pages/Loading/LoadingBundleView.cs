using UI.PageTransitions.Title;
using UI.Scripts.Route;
using UnityEngine;

namespace UI.Views.Pages.Loading
{
	public class LoadingBundleView : MonoBehaviour {

		public void Bind()
		{
			Time.timeScale = 1f;
			PageRouter.Instance.DoTransition<TitleTransition>();
		}
	}
}
