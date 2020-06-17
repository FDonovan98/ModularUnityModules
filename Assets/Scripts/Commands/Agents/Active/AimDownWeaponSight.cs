// Title: AimDownWeaponSight.cs
// Author: Harry Donovan
// Date Last Edited: 05/05/2020
// Description: Switches between two available cameras

using UnityEngine;

[CreateAssetMenu(fileName = "AimDownWeaponSight", menuName = "Commands/Active/Aim Down Weapon Sight", order = 0)]
public class AimDownWeaponSight : ActiveCommandObject
{
    [SerializeField]
    KeyCode aimDownSight = KeyCode.Mouse1;

    protected override void OnEnable()
    {
        keyTable.Add("Aim Down Sight", aimDownSight);
    }

    public override void RunCommandOnStart(AgentInputHandler agentInputHandler)
    {
        if (agentInputHandler.isLocalAgent)
        {      
            agentInputHandler.runCommandOnUpdate += RunCommandOnUpdate;
        }
    }

    void RunCommandOnUpdate(GameObject agent, AgentInputHandler agentInputHandler, AgentValues agentValues)
    {
        if (agentValues.aDSIsAToggle)
        {
            if (Input.GetKeyDown(aimDownSight))
            {
                if (agentInputHandler.camera.aDSCamera.enabled == true)
                {
                    ToggleADS(agentInputHandler, false);
                }
                else
                {
                    ToggleADS(agentInputHandler, true);
                }
            }
        }
        else
        {
            if (Input.GetKeyDown(aimDownSight))
            {
                ToggleADS(agentInputHandler, true);
            }
            else if (Input.GetKeyUp(aimDownSight))
            {
                ToggleADS(agentInputHandler, false);
            }
        }
    }

    void ToggleADS(AgentInputHandler agentInputHandler, bool toggle)
    {
        agentInputHandler.camera.isADS = toggle;

        if (toggle)
        {
            agentInputHandler.camera.agentCamera = agentInputHandler.camera.aDSCamera;
        }
        else
        {
            agentInputHandler.camera.agentCamera = agentInputHandler.camera.mainCamera;
        }

        agentInputHandler.hUD.HUDCanvas.worldCamera = agentInputHandler.camera.agentCamera;

        agentInputHandler.camera.mainCamera.enabled = !toggle;
        agentInputHandler.camera.aDSCamera.enabled = toggle;   
    }
}