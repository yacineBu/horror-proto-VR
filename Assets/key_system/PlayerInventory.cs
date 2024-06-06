using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
   [SerializeField]  private bool hasKey = false;

    public bool HasKey()
    {
        return hasKey;
    }

    public void PickupKey()
    {
        hasKey = true;
    }
}
