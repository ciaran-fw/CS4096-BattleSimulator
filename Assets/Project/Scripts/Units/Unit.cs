using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] private GameObject selectionIndicator;

    public UnitDefinition Definition { get; private set; }
    public UnitTeam Team { get; private set; }

    public void Initialize(UnitDefinition definition, UnitTeam team)
    {
        Definition = definition;
        Team = team;
    }

    public void SetSelected(bool selected)
    {
        selectionIndicator.SetActive(selected);
    }
}
