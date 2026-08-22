using UnityEngine;

public class UnitStats : MonoBehaviour
{
    [SerializeField] private UnitData unitData;

    // Estado en tiempo de ejecución (Read-Only desde fuera, modificado solo por funciones)
    public float CurrentHealth { get; private set; }

    // Propiedades calculadas considerando modificadores
    public float MaxHealth => unitData.BaseHealth + bonusMaxHealth;
    public float Attack => Mathf.Max(0, unitData.BaseDamage + bonusAttack);
    public float Armor => Mathf.Max(0, unitData.BaseArmor + bonusArmor);

    public float MovementRange => Mathf.Max(0, unitData.BaseMovementRange + bonusMovementRange);

    // Modificadores temporales (Buffs / Debuffs)
    private int bonusMaxHealth;
    private int bonusAttack;
    private int bonusArmor;
    private float bonusMovementRange;

    //private void Start()
    //{
    //    InitializeStats();
    //}

    public void Initialize(UnitData data)
    {
        unitData = data;

        if (unitData == null) return;

        // Copiamos la vida base a la vida actual al iniciar
        CurrentHealth = data.BaseHealth;
    }

    public void ModifyHealth(int amount)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0, MaxHealth);
    }

    public void AddAttackBonus(int amount)
    {
        bonusAttack += amount;
    }
}