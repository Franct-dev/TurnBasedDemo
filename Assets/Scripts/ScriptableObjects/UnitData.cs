using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct CardEntry
{
    public CardData card;
    [Min(1)] public int amount;
}

[CreateAssetMenu(menuName = "ScriptableObjects/UnitData")]
public class UnitData : ScriptableObject
{
    public string UnitName;
    public Sprite Portrait;
    public float BaseMovementRange = 5;
    public float BaseHealth;
    public float BaseDamage;
    public float BaseAttackRange = 1;
    public float BaseArmor;

    [Header("Mazo Inicial")]
    public List<CardEntry> startingDeck; // Cartas plantilla de esta unidad
}
