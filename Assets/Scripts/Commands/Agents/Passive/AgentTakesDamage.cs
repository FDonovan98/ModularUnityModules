// Title: AgentTakesDamage.cs
// Author: Harry Donovan
// Date Last Edited: 05/05/2020
// Description: If the agent has health, decreases it by the amount of damage the hit did.

using UnityEngine;

[CreateAssetMenu(fileName = "DefaultAgentTakesDamage", menuName = "Commands/Passive/AgentTakesDamage", order = 0)]
public class AgentTakesDamage : PassiveCommandObject
{
    public override void RunCommandOnStart(AgentInputHandler agentInputHandler)
    {
        agentInputHandler.runCommandOnAgentHasBeenHit += RunCommandOnAgentHasBeenHit;
    }

    private void RunCommandOnAgentHasBeenHit(AgentInputHandler agentInputHandler, Vector3 position, Vector3 normal, float value)
    {
        AgentController agentController = (AgentController)agentInputHandler;

        agentController.ChangeStat(ResourceType.Health, -value);
    }
}