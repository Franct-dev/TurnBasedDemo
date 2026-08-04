using UnityEngine;
using System;

// 1. El Contexto: Tiene la info de quién lanza la carta y a quién va dirigida.
public class EffectContext
{
    public GameObject Caster;
    public GameObject Target;
}

public interface ICardEffect
{
    void Execute(EffectContext context);
}

// 2. La Interfaz: El contrato de nuestros efectos
[Serializable]
public abstract class CardEffect: ICardEffect
{
    public abstract void Execute(EffectContext context);
}

// 3. Los Efectos Concretos
[Serializable]
public class DamageEffect : CardEffect
{
    [SerializeField] private int damageAmount = 3;

    public override void Execute(EffectContext context)
    {
        // En un juego real, aquí buscarías un componente de vida:
        // context.Target.GetComponent<Health>().TakeDamage(damageAmount);
        Debug.Log($"[DamageEffect] Haciendo {damageAmount} de daño a {context.Target.name}");
    }
}

[Serializable]
public class HealEffect : CardEffect
{
    [SerializeField] private int healAmount = 5;

    public override void Execute(EffectContext context)
    {
        Debug.Log($"[HealEffect] Curando {healAmount} de vida a {context.Caster.name}");
    }
}

[Serializable]
public class DrawCardEffect : CardEffect
{
    [SerializeField] private int cardsToDraw = 1;

    public override void Execute(EffectContext context)
    {
        //context.user.DrawCards(cardsToDraw);
        Debug.Log($"Robando {cardsToDraw} cartas.");
    }
}
