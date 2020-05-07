// Title: Gravity.cs
// Author: Harry Donovan
// Date Last Edited: 07/05/2020
// Description: Applies a constant acceleration in the direction of agentInputHandler.gravityDirection, which by default is worlddown.

using UnityEngine;

[CreateAssetMenu(fileName = "DefaultGravity", menuName = "Commands/Passive/Gravity")]
public class Gravity : PassiveCommandObject
{
    public override void RunCommandOnStart(AgentInputHandler agentInputHandler)
    {
        agentInputHandler.runCommandOnFixedUpdate += RunCommandOnFixedUpdate;
    }
    void RunCommandOnFixedUpdate(GameObject agent, AgentInputHandler agentInputHandler, AgentValues agentValues)
    {
        agentInputHandler.agentRigidbody.velocity += agentInputHandler.gravityDirection.normalized * agentValues.gravityAcceleration * Time.fixedDeltaTime;
    }
}