using UnityEngine;
using System.Collections.Generic;

public class PlayerSlotManager : MonoBehaviour {
    public int SelectedInventorySlot;

    private Dictionary<int, GameObject> slotDictionary;
    public GameObject GetSlot(int slot) {
        return slotDictionary[slot];
    }
    public GameObject GetSelectedSlot() {
        return slotDictionary[SelectedInventorySlot];
    }

    private const int INVENTORY_SLOT_COUNT = 7;

    void Start() {
        SelectedInventorySlot = 1;
        slotDictionary = new Dictionary<int, GameObject>();
        // TODO: Add initial items in inventory
        /*slotDictionary.Add(1, blockDatabaseReference.GetBlockPrefabByName("Dev"));
        slotDictionary.Add(2, blockDatabaseReference.GetBlockPrefabByName("Snow"));
        slotDictionary.Add(3, blockDatabaseReference.GetBlockPrefabByName("MossyStone"));
        slotDictionary.Add(4, blockDatabaseReference.GetBlockPrefabByName("Concrete"));
        slotDictionary.Add(5, blockDatabaseReference.GetBlockPrefabByName("Cobble"));
        slotDictionary.Add(6, blockDatabaseReference.GetBlockPrefabByName("Dirt"));
        slotDictionary.Add(7, blockDatabaseReference.GetBlockPrefabByName("Sand"));*/
    }
	
	void Update() {
        for (int i = 1; i <= INVENTORY_SLOT_COUNT; ++i) {
            if (Input.GetKeyDown(i.ToString())) {
                SelectedInventorySlot = i;
            }
        }
	}
}
