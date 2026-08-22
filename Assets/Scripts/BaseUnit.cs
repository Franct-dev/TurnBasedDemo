using UnityEngine;

public interface ISelectable
{
    GameObject gameObject { get; }
    void OnSelect();
    void OnDeselect();
}

public interface ITargeteable
{
    GameObject gameObject { get; }
    Transform GetTransform();
    void ApplyEffect(EffectContext context);
}

public enum Faction
{
    Player, Enemy, Neutral
}

public class BaseUnit : MonoBehaviour, ISelectable, ITargeteable, IDamageable, IHealable
{
    public Faction Faction;

    //magia para que devuelva el transform tal cual
    public Transform GetTransform() => transform;

    public UnitData Data;

    //SUBCOMPONENTES
    public UnitStats Stats { get; private set; }
    public UnitCardController CardController { get; private set; }

    public float MovementRange => Stats != null ? Stats.MovementRange : Data.BaseMovementRange;

    void Awake()
    {
        Stats = GetComponent<UnitStats>();
        CardController = GetComponent<UnitCardController>();

        if(Data != null)
        {
            Initialize();
        }
    }

    void Initialize()
    {
        Stats?.Initialize(Data);
        CardController?.Initialize(Data);
    }

    public void ApplyEffect(EffectContext context)
    {

    }

    public void OnDeselect()
    {
        Debug.Log($"Deselected unit {gameObject}");
    }

    public void OnSelect()
    {
        Debug.Log($"Selected unit {gameObject}");
    }

    public void TakeDamage(int amount)
    {
        Debug.Log($"Unit {gameObject} takes {amount} damage");
    }

    public void Heal(int amount)
    {
        Debug.Log($"Unit {gameObject} heals {amount} damage");
    }
}
