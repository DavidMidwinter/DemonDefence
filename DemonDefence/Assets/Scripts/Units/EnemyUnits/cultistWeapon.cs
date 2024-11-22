using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cultistWeapon : MonoBehaviour
{
    public List<GameObject> weapons;
    public GameObject weapon;

    public void Awake()
    {
        spawnWeapon();
    }

    public void spawnWeapon()
    {
        Debug.Log(this);
        int weaponIndex = Random.Range(0, weapons.Count);
        weapon = Instantiate(weapons[weaponIndex]);
        weapon.transform.parent = transform;
        weapon.transform.localPosition = Vector3.zero;
        weapon.transform.localRotation = Quaternion.identity;
        Debug.Log(weapon.transform.position);
        weapon.transform.localScale = Vector3.one;
        Debug.Log("Weapon spawned");

    }
}
