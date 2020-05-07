// Title: BaseObject.cs
// Author: Harry Donovan
// Date Last Edited: 07/05/2020
// Description: Base object for any equipable items, contains a settable item type.

using UnityEngine;

public class BaseObject : ScriptableObject
{
    public enum ItemType
    {
        Primary,
        Secondary,
        Armour,
        Material
    }

    public ItemType itemType;
}
