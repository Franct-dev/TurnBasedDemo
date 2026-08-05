using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/UnitData")]
public class UnitData : ScriptableObject
{
    public string unitName;
    public Sprite portrait;
    public float baseHealth;
    public float baseDamage;
}
