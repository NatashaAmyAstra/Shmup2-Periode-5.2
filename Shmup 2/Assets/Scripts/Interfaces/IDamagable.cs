using UnityEngine;

public interface IDamagable
{
    /// <summary>
    /// Damage instance of IDamagable. Pass damage value or leave empty to deal 1 damage;
    /// </summary>
    public void Damage(int damage = 1);
}
