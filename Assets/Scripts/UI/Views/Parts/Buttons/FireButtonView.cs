using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Views.Parts.Buttons
{
	public class FireButtonView : MonoBehaviour , IPointerUpHandler, IPointerDownHandler
	{

		private bool _isButtonDown;

		public bool IsButtonDown
		{
			get { return _isButtonDown; }
		}
		
		//OnPointerDown is also required to receive OnPointerUp callbacks
		public void OnPointerDown(PointerEventData eventData)
		{
			_isButtonDown = true;
		}

		//Do this when the mouse click on this selectable UI object is released.
		public void OnPointerUp(PointerEventData eventData)
		{
			_isButtonDown = false;
		}
	}
}
