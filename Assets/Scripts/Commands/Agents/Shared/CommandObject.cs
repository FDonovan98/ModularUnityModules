// Title: CommandObject.cs
// Author: Harry Donovan
// Date Last Edited: 07/05/2020
// Description: Base abstract command class all command objects are built upon.

using UnityEngine;

public abstract class CommandObject : ScriptableObject
{
    public abstract void RunCommandOnStart(AgentInputHandler agentInputHandler);
}