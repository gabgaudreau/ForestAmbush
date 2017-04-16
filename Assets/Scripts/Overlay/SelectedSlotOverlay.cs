using UnityEngine;
using System.Collections.Generic;

public class SelectedSlotOverlay : MonoBehaviour {
    [SerializeField]
    GameObject player;

    [SerializeField]
    List<Transform> slotReferences;

    private PlayerSlotManager cachedPlayerInventoryManager;
    private RectTransform cachedRectTransform;

    void Start() {
        cachedPlayerInventoryManager = player.GetComponent<PlayerSlotManager>();
        cachedRectTransform = GetComponent<RectTransform>();
    }

	void Update() {
        Transform slotTransform = slotReferences[cachedPlayerInventoryManager.SelectedInventorySlot - 1];
        transform.SetParent(slotTransform);
        cachedRectTransform.anchoredPosition = Vector3.zero;
    }
}
