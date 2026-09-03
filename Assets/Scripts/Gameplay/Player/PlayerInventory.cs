using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private Item currentItem = null;

    public Item CurrentItem
    {
        get => currentItem;
    }
}
