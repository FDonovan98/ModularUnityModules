using UnityEngine;

[CreateAssetMenu(fileName = "DefaultWeaponEffects", menuName = "Commands/Passive/Weapon Effects", order = 0)]
public class WeaponEffects : PassiveCommandObject
{
    public override void RunCommandOnStart(AgentInputHandler agentInputHandler)
    {
        agentInputHandler.runCommandOnWeaponFired += RunCommandOnWeaponFired;
    }

    private void RunCommandOnWeaponFired(AgentInputHandler agentInputHandler)
    {
        
    }
}