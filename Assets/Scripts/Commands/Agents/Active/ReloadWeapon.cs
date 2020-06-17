// Title: ReloadWeapon.cs
// Author: Harry Donovan
// Date Last Edited: 05/05/2020
// Description: Reduces the total extra ammo by the ammo missing in the magazine. Refills the magazine to its max capacity and locks the agent into reloading for the agentInputHandler.currentWeapon.reloadDuration

using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "DefaultReload", menuName = "Commands/Active/ReloadWeapon", order = 0)]
public class ReloadWeapon : ActiveCommandObject
{
    [SerializeField]
    KeyCode reloadKey = KeyCode.R;

    protected override void OnEnable()
    {
        keyTable.Add("Reload", reloadKey);
    }

    public override void RunCommandOnStart(AgentInputHandler agentInputHandler)
    {
        if (agentInputHandler.isLocalAgent)
        {
            agentInputHandler.runCommandOnUpdate += RunCommandOnUpdate;
        }
    }

    private void RunCommandOnUpdate(GameObject agent, AgentInputHandler agentInputHandler, AgentValues agentValues)
    {
        if (Input.GetKeyDown(reloadKey))
        {
            AgentController agentController = (AgentController)agentInputHandler;

            if (CanReload(agentController))
            {
                agentInputHandler.weapons.weaponAudioSource.PlayOneShot(agentInputHandler.weapons.currentWeapon.reloadSound);

                agentInputHandler.StartCoroutine(Reload(agentInputHandler.weapons.currentWeapon.reloadDuration, agentController));

                agentInputHandler.isReloading = true;
            }
        }
    }

    private bool CanReload(AgentController agentController)
    {
        if (agentController.currentExtraAmmo > 0)
        {
            if (agentController.currentBulletsInMag < agentController.weapons.currentWeapon.magSize)
            {
                return true;
            }
        }

        return false;
    }

    private IEnumerator Reload(float reloadTime, AgentController agentController)
    {
        yield return new WaitForSeconds(reloadTime);
        int bulletsUsed;

        bulletsUsed = agentController.weapons.currentWeapon.magSize - agentController.currentBulletsInMag;

        if (bulletsUsed > agentController.currentExtraAmmo)
        {
            bulletsUsed = agentController.currentExtraAmmo;
        }

        agentController.ChangeStat(ResourceType.MagazineAmmo, bulletsUsed);
        agentController.ChangeStat(ResourceType.ExtraAmmo, -bulletsUsed);

        agentController.isReloading = false;
    }
}