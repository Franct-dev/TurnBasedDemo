using UnityEngine;

public interface IDamageable
{
    void TakeDamage(int amount);
}

public interface IHealable
{
    void Heal(int amount);
}

public interface IStatusEffectable
{
    void ApplyStatus(string status, int duration);
}

public interface IManaUser
{
    void ModifyMana(int amount);
}
