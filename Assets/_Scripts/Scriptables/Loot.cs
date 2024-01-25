using UnityEngine;

[CreateAssetMenu]
public class Loot : ScriptableObject
{
    public GameObject LootGameobject;
    public string LootName;
    public int DropChance;

    public Loot(string lootName, int dropChance){
        LootName = lootName;
        DropChance = dropChance;
    }
}
