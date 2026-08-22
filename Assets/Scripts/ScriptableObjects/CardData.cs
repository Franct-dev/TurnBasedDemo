using UnityEngine;
using System.Collections.Generic;
using System;

[Flags]
public enum TargetType
{
    None = 0,
    Self = 1 << 0, // La propia unidad que lanza la carta
    Ally = 1 << 1, // Otras unidades aliadas
    Enemy = 1 << 2, // Unidades enemigas
    Interactable = 1 << 3  // Objetos del mapa (barriles, cofres, etc.)
}


[CreateAssetMenu(menuName = "ScriptableObjects/CardData")]
public class CardData : ScriptableObject
{
    [Header("DISPLAY")]
    public string CardName;
    public Sprite Artwork;
    public string CardType;
    [TextArea]
    public string Description;

    [Header("GAMEPLAY")]
    public TargetType validTargets = TargetType.Enemy; // Se puede seleccionar múltiple en el Inspector
    public float castRange = 5; //distancia (metros)
    public bool hasInfiniteRange = false; //habilidades de rango global

    [SerializeReference, SubclassSelector]
    private List<CardEffect> effects = new List<CardEffect>();

    public void PlayCard(EffectContext context)
    {
        Debug.Log($"Played card {CardName}");
        foreach (var effect in effects)
        {
            effect.Execute(context);
        }
    }

    // Método de validación
    public bool IsValidTarget(BaseUnit caster, GameObject target)
    {
        if (target == null) return false;

        // 1. Si el objetivo es el propio emisor
        if (target == caster.gameObject)
        {
            return validTargets.HasFlag(TargetType.Self);
        }

        if (!hasInfiniteRange)
        {
            float distance = (caster.transform.position - target.transform.position).sqrMagnitude;
            if (distance > castRange * castRange)
            {
                Debug.Log("Target outside of range");
                return false;
            }
        }

        // 2. Si el objetivo es una unidad (BaseUnit)
        if (target.TryGetComponent<BaseUnit>(out var targetUnit))
        {
            if (targetUnit.Faction == caster.Faction)
            {
                return validTargets.HasFlag(TargetType.Ally);
            }
            else
            {
                return validTargets.HasFlag(TargetType.Enemy);
            }
        }

        // 3. Si el objetivo es un objeto interactivo del escenario
        if (target.TryGetComponent<ISelectable>(out _))
        {
            return validTargets.HasFlag(TargetType.Interactable);
        }

        return false;
    }
}
