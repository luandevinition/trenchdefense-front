using UnityEngine;
using UnityEngine.EventSystems;

public class FixedJoystick : Joystick
{
    [Header("Fixed Joystick")]
    Vector2 joystickPosition = Vector2.zero;

    [SerializeField]
    private bool _isKeepValue;
    
    /// <summary>
    /// Set Caemra 
    /// </summary>
    /// <param name="camera"></param>
    public override void SetCamera(Camera camera)
    {
        joystickPosition = background.position;   
    }

    public override void OnDrag(PointerEventData eventData)
    {
        Vector2 direction = eventData.position - joystickPosition;
        inputVector = (direction.magnitude > background.sizeDelta.x / 2f) ? direction.normalized : direction / (background.sizeDelta.x / 2f);
        handle.anchoredPosition = (inputVector * background.sizeDelta.x / 2f) * handleLimit;
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (!_isKeepValue)
        {
            inputVector = Vector2.zero;
        }
        handle.anchoredPosition = Vector2.zero;
    }
}