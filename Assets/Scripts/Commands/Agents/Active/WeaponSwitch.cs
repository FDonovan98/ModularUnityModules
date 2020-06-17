// Title: WeaponSwitch.cs
// Author: Eugene Syricks
// Date Last Edited: 05/05/2020
// Description: Toggles which weapon is enabled for the agent.

using UnityEngine;

[CreateAssetMenu(fileName = "DefaultSwitchWeapon", menuName = "Commands/Active/SwitchWeapon", order = 0)]
public class WeaponSwitch : ActiveCommandObject
{
    [SerializeField]
    KeyCode primaryWeaponKey = KeyCode.Alpha1;
    [SerializeField]
    KeyCode secondaryWeaponKey = KeyCode.Alpha2;

    protected override void OnEnable()
    {
        keyTable.Add("Select Primary Weapon", primaryWeaponKey);
        keyTable.Add("Select Secondary Weapon", secondaryWeaponKey);
    }

    public override void RunCommandOnStart(AgentInputHandler agentInputHandler)
    {
        agentInputHandler.runCommandOnUpdate += RunCommandOnUpdate;
    }

    private void RunCommandOnUpdate(GameObject agent, AgentInputHandler agentInputHandler, AgentValues agentValues)
    {
        if (Input.GetKeyDown(primaryWeaponKey))
        {
            if (agentInputHandler.weapons.currentWeaponID != 0)
            {
                agentInputHandler.weapons.currentWeaponID = 0;
                SwitchWeapon(agentInputHandler);
            }
        }
        else if (Input.GetKeyDown(secondaryWeaponKey))
        {
            if (agentInputHandler.weapons.currentWeaponID != 1)
            {
                agentInputHandler.weapons.currentWeaponID = 1;
                SwitchWeapon(agentInputHandler);
            }
        }
        else if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            agentInputHandler.weapons.currentWeaponID += 1;
            if (agentInputHandler.weapons.currentWeaponID >= agentInputHandler.weapons.equippedWeapons.Length)
            {
                agentInputHandler.weapons.currentWeaponID = 0;
            }
            SwitchWeapon(agentInputHandler);
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            agentInputHandler.weapons.currentWeaponID -= 1;
            if (agentInputHandler.weapons.currentWeaponID < 0)
            {
                agentInputHandler.weapons.currentWeaponID = agentInputHandler.weapons.equippedWeapons.Length - 1;
            }
            SwitchWeapon(agentInputHandler);
        }
    }

    private void SwitchWeapon(AgentInputHandler agentInputHandler)
    {
        MeshFilter mesh;
        mesh = agentInputHandler.weapons.weaponObject.GetComponent<MeshFilter>();

        agentInputHandler.weapons.currentWeapon = agentInputHandler.weapons.equippedWeapons[agentInputHandler.weapons.currentWeaponID];
        if (mesh != null && agentInputHandler.weapons.currentWeapon.weaponMesh != null)
        {
            mesh.mesh = agentInputHandler.weapons.currentWeapon.weaponMesh;
        }
        else
        {
            Debug.LogWarning("CurrentWeapon scriptable object is missing it's mesh.");
        }
    }
}
