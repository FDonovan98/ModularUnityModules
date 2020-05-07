// Title: ActiveCommandObject.cs
// Author: Harry Donovan
// Date Last Edited: 07/05/2020
// Description: Abstract scriptable object used as the base class for any active commands. keyTable is for use when creating dynamic and rebindable key bind tables. Currently unused.

using UnityEngine;
using System.Collections.Generic;

public abstract class ActiveCommandObject : CommandObject
{
    // Can then store master dictionary in keybind menu with each dictionary and a refrence to its scriptable object.
    public Dictionary<string, KeyCode> keyTable = new Dictionary<string, KeyCode>();

    protected abstract void OnEnable();

    public virtual void ChangeKeycode(string newKeycode)
    {

    }

    // I am unsure whether this would be helpful or redundant. 
    // My current thought is redundant so it will remain commented out for now.
    // public virtual bool ExecuteCondition()
    // {
    //     return true;
    // }
}