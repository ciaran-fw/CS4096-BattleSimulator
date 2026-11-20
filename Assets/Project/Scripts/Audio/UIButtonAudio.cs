using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIButtonAudio : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (button.interactable)
        {
            if (AudioManager.Instance != null) AudioManager.Instance.PlayButtonHover();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (button.interactable)
        {
            if (AudioManager.Instance != null)  AudioManager.Instance.PlayButtonClick();
        }
        else
        {
            if (AudioManager.Instance != null)  AudioManager.Instance.PlayError();
        }
    }
}
