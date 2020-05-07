// Title: Armour.cs
// Author: Harry Donovan
// Date Last Edited: 07/05/2020
// Description: Scriptable object for armour. Can be configured to effect the speed of the agent and damage the agent takes.

using UnityEngine;

[CreateAssetMenu(fileName = "New Armour", menuName = "Objects/Create New Armour")]

public class Armour : BaseObject
{
    [Tooltip("Will cause player to take x% of damage.")]
    public float damageMultiplier;
    [Tooltip("Acts as a multiplier for walking and sprinting speed.")]
    public float speedMultiplier;
}
