// Title: RecoilWeapon.cs
// Author: Harry Donovan
// Date Last Edited: 07/05/2020
// Description: Kicks the camera up when the weapon is fired and will recenter it. This rate of recentering and recoil is controllable through animation curves and Weapon.recoilForce on the Weapon.cs object.

using UnityEngine;

[CreateAssetMenu(fileName = "DefaultRecoilWeapon", menuName = "Commands/Passive/RecoilWeapon", order = 0)]
public class RecoilWeapon : PassiveCommandObject
{
    public override void RunCommandOnStart(AgentInputHandler agentInputHandler)
    {
        agentInputHandler.runCommandOnWeaponFired += RunCommandOnWeaponFired;
        agentInputHandler.runCommandOnUpdate += RunCommandOnUpdate;
    }

    void RunCommandOnWeaponFired(AgentInputHandler agentInputHandler)
    {
        HandleRecoil(agentInputHandler, agentInputHandler.weapons.currentWeapon.upForceStep);
    }

    void RunCommandOnUpdate(GameObject agent, AgentInputHandler agentInputHandler, AgentValues agentValues)
    {
        float timeDelta;
        timeDelta = -Time.deltaTime / agentInputHandler.weapons.currentWeapon.downForceDuration;

        HandleRecoil(agentInputHandler, timeDelta);
    }

    void HandleRecoil(AgentInputHandler agentInputHandler, float timeDelta)
    {

        AnimationCurve weaponRecoilCurveUp = agentInputHandler.weapons.currentWeapon.recoilCurveUp;
        AnimationCurve weaponRecoilCurveDown = agentInputHandler.weapons.currentWeapon.recoilCurveDown;

        float valueDelta;

        if (timeDelta > 0)
        {
            valueDelta = weaponRecoilCurveUp.Evaluate(agentInputHandler.weapons.currentRecoilValue + timeDelta) - weaponRecoilCurveUp.Evaluate(agentInputHandler.weapons.currentRecoilValue);
        }
        else
        {
            valueDelta = weaponRecoilCurveDown.Evaluate(agentInputHandler.weapons.currentRecoilValue + timeDelta) - weaponRecoilCurveDown.Evaluate(agentInputHandler.weapons.currentRecoilValue);
        }


        valueDelta *= -agentInputHandler.weapons.currentWeapon.recoilForce;

        agentInputHandler.camera.agentCamera.transform.Rotate(valueDelta, 0.0f, 0.0f);

        // Prevents index errors.
        agentInputHandler.weapons.currentRecoilValue += timeDelta;
        agentInputHandler.weapons.currentRecoilValue = Mathf.Clamp(agentInputHandler.weapons.currentRecoilValue, 0.0f, 1.0f - agentInputHandler.weapons.currentWeapon.upForceStep);
    }
}