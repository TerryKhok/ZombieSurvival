using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class LootManager : MonoBehaviour
{
    [SerializeField] private GameObject _droppedItemPrefab;
    [SerializeField] private List<Loot> _lootList = new List<Loot>();

    Loot GetDroppedItem()
    {
        int randonNumber = Random.Range(1, 101);
        List<Loot> possibleItems = new List<Loot>();
        foreach (Loot item in _lootList)
        {
            if (randonNumber <= item.DropChance)
            {
                possibleItems.Add(item);
            }
        }
        if (possibleItems.Count > 0)
        {
            Loot droppedItem = possibleItems[Random.Range(0, possibleItems.Count)];
            return droppedItem;
        }
        Debug.Log("No loot dropped");
        return null;
    }

    public void InstantiateLoot(Vector3 spawnPosition){
        Loot droppedItem = GetDroppedItem();
        if(droppedItem !=null){ 
            GameObject lootGameObject = Instantiate(_droppedItemPrefab,spawnPosition,Quaternion.identity);
            droppedItem.LootGameobject = GameObject.Find(droppedItem.name + "Test");
            GameObject lootGameObjectModel = Instantiate(droppedItem.LootGameobject,spawnPosition,Quaternion.identity);
            lootGameObjectModel.transform.SetParent(lootGameObject.transform);

            float force = 300f;
            Vector3 explosionPos = lootGameObject.transform.position - new Vector3(Random.Range(-.5f,.5f),0.5f,Random.Range(-.5f,.5f));
            lootGameObject.GetComponent<Rigidbody>().AddExplosionForce(force,explosionPos,2f);
        }
    }
}
 