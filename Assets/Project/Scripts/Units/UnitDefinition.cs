using UnityEngine;

[CreateAssetMenu(fileName = "UnitDefinition", menuName = "Scriptable Objects/UnitDefinition")]
public class UnitDefinition : ScriptableObject
{
    [Header("Unit Information")]
    public string unitName;
    public Sprite icon;
    public int cost;
    public UnitRole role;

    [Header("Unit Prefab")]
    public GameObject prefab;

    [Header("AI and Stats")]
    public float health = 100f;
    public float damage = 10f;
    public float range = 1.5f;
    public float attack_speed = 1f;
    public float moveSpeed = 5f;
}
