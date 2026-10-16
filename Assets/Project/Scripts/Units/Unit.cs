using UnityEngine;

public class Unit : MonoBehaviour
{
    public UnitDefinition Definition { get; private set; }

    public void Initialize(UnitDefinition definition)
    {
        Definition = definition;
    }
}
