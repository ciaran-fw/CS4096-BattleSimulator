using UnityEngine;

[CreateAssetMenu(fileName = "UnitDefinition", menuName = "Scriptable Objects/UnitDefinition")]
public class UnitDefinition : ScriptableObject
{
    [Header("Unit Information")]
    public string unitName;
    public Sprite icon;
    public int cost;

    [Header("Unit Prefab")]
    public GameObject prefab;
}
