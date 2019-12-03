using Slaggy.Utility;
using UnityEngine;

public class Usable { }
public class UsableA : Usable { }
public class UsableB : Usable { }
public class UsableC : UsableB { }

[System.Serializable] public class UsableType : ConstrainedSerializableType<Usable> { }
[System.Serializable] public class MonoBehaviourType : ConstrainedSerializableType<MonoBehaviour> { }

public class SerializableTypeExampleScript : MonoBehaviour
{
    [Header("Normal Types")]
    public SerializableType type;
    public SerializableType[] types;

    [Header("Constrained Types")]
    public UsableType usableType;
    public UsableType[] usableTypes;
    public MonoBehaviourType[] monoBehaviourTypes;
}
