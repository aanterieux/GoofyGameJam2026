using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private Item currentItem = null;

    public Item CurrentItem
    {
        get => currentItem;
    }

    public void SetCurrentItem(Item _newItem)
    {
        currentItem = _newItem;
    }
}
