using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitCard : MonoBehaviour
{
    [SerializeField] private Image unitIcon;
    [SerializeField] private TMP_Text unitName;
    [SerializeField] private TMP_Text unitPrice;

    public void Setup(Sprite icon, string name, string price)
    {
        unitIcon.sprite = icon;
        unitName.text = name;
        unitPrice.text = price;
    }
}
