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
                if (agentInputHandler.cameraList.aDSCamera.enabled == true)
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
        agentInputHandler.cameraList.isADS = toggle;

        if (toggle)
        {
            agentInputHandler.cameraList.agentCamera = agentInputHandler.cameraList.aDSCamera;
        }
        else
        {
            agentInputHandler.cameraList.agentCamera = agentInputHandler.cameraList.mainCamera;
        }

        agentInputHandler.hUD.HUDCanvas.worldCamera = agentInputHandler.cameraList.agentCamera;

        agentInputHandler.cameraList.mainCamera.enabled = !toggle;
        agentInputHandler.cameraList.aDSCamera.enabled = toggle;   
    }
}