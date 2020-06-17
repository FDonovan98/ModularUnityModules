// Title: AgentGivesHitFeedback.cs
// Author: Harry Donovan
// Date Last Edited: 05/05/2020
// Description: Spawns a particle effect and plays an attached noise when the agent is hit.

using UnityEngine;

[CreateAssetMenu(fileName = "DefaultAgentGivesHitFeedback", menuName = "Commands/Passive/Agent Gives Hit Feedback", order = 0)]
public class AgentGivesHitFeedback : PassiveCommandObject
{
    public override void RunCommandOnStart(AgentInputHandler agentInputHandler)
    {
        agentInputHandler.runCommandOnAgentHasBeenHit += RunCommandOnAgentHasBeenHit;
    }

    private void RunCommandOnAgentHasBeenHit(AgentInputHandler agentInputHandler, Vector3 position, Vector3 normal, float value)
    {
        if (agentInputHandler.agentHitParticles != null)
        {
			GameObject hitEffect = Instantiate(agentInputHandler.agentHitParticles, position, Quaternion.Euler(normal));

            if (agentInputHandler.agentHitSound != null)
            {   
                agentInputHandler.mainAudioSource.PlayOneShot(agentInputHandler.agentHitSound);
            }
            else
            {
                Debug.LogAssertion(agentInputHandler.gameObject.name + " doesn't have a hit feedback sound");
            }

            Destroy(hitEffect, 5f);
        }
        else
        {
            Debug.LogAssertion(agentInputHandler.gameObject.name + " doesn't have a hit feedback particle effect");
        }
    }
}