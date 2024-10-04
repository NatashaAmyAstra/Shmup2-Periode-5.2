using UnityEngine;

[CreateAssetMenu(fileName = "WeaponSO", menuName = "Weapon")]
public class WeaponSO : ScriptableObject
{
    public int ProjectileCount;
    public int Damage;
    public float MultishotAngle;
    public GameObject ProjectilePrefab;
}
