using UnityEngine;

namespace WhaleShark.Config
{
    [CreateAssetMenu(menuName = "WhaleShark/Config/WeaponConfig")]
    public class WeaponConfig : ScriptableObject
    {
        [Header("Basic Settings")]
        public string weaponName = "Basic Weapon";
        public float damage = 10f;
        public float fireRate = 1f; // shots per second
        public float range = 10f;

        [Header("Ammo")]
        public int maxAmmo = 30;
        public int currentAmmo = 30;
        public float reloadTime = 2f;
        public bool infiniteAmmo = false;

        [Header("Projectile")]
        public GameObject projectilePrefab;
        public float projectileSpeed = 20f;
        public float projectileLifetime = 5f;

        [Header("Effects")]
        public GameObject muzzleFlash;
        public GameObject hitEffect;
        public AudioClip fireSound;
        public AudioClip reloadSound;

        [Header("Recoil")]
        public float recoilForce = 5f;
        public float recoilRecoveryTime = 0.5f;

        public bool CanFire => infiniteAmmo || currentAmmo > 0;
        public float FireInterval => 1f / fireRate;
    }
}