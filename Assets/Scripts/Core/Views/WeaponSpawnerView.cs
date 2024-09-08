using Core.Config;
using Core.Controllers;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class WeaponSpawnerView : MonoBehaviour
{
    [SerializeField]
    public List<WeaponConfig> AllWeapons;

    public static WeaponSpawnerView Instance;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        RandomSpawn();
    }

    async void RandomSpawn()
    {
        await Task.Delay(Random.Range(9000, 25000));
        Instantiate(CreateWeapon(), MatchController.RandomFieldVector(), Quaternion.identity);
        RandomSpawn();
    }

    GameObject CreateWeapon()
    {
        var randomIndex = Random.Range(0, AllWeapons.Count);
        AllWeapons[randomIndex].WeaponModel.name = AllWeapons[randomIndex].WeaponName; 
        return AllWeapons[randomIndex].WeaponModel;
    }
}
