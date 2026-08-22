using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonSound : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Play sound when mouse moves over button
        AudioManager.Instance?.PlayButtonHover();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Play sound when button is clicked
        AudioManager.Instance?.PlayButtonClick();
    }
}