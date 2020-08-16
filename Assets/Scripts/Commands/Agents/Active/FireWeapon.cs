// Title: FireWeapon.cs
// Author: Harry Donovan
// Date Last Edited: 05/05/2020
// Description: Checks if a weapon shot hit a game object, and activates the AgentInputHandler.runCommandOnAgentHasBeenHit on the hit object if it exists.

using UnityEngine;

[CreateAssetMenu(fileName = "DefaultFireWeapon", menuName = "Commands/Active/FireWeapon", order = 0)]
public class FireWeapon : ActiveCommandObject
{
    [SerializeField]
    private KeyCode primaryFire = KeyCode.Mouse0;
    protected override void OnEnable()
    {
        keyTable.Add("Primary Fire", primaryFire);
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
        agentInputHandler.weapons.timeSinceLastShot += Time.deltaTime;

        if (CanFire(agentInputHandler))
        {
            if (Input.GetKeyDown(primaryFire))
            {
                ActuallyFire(agent, agentInputHandler);
            }
            else if (agentInputHandler.weapons.currentWeapon.fireMode == Weapon.FireType.FullAuto && Input.GetKey(primaryFire))
            {
                ActuallyFire(agent, agentInputHandler);
            }
        }
    }

    bool CanFire(AgentInputHandler agentInputHandler)
    {
        AgentController agentController = (AgentController)agentInputHandler;

        if (agentInputHandler.allowInput && !agentController.isReloading)
        {
            if (agentInputHandler.weapons.timeSinceLastShot > agentInputHandler.weapons.currentWeapon.fireRate)
            {
                if (agentInputHandler.weapons.currentWeapon.fireMode == Weapon.FireType.Melee || agentController.currentBulletsInMag > 0)
                {
                    return true;
                }
            }
        }

        return false;
    }

    void ActuallyFire(GameObject agent, AgentInputHandler agentInputHandler)
    {
        if (agentInputHandler.runCommandOnWeaponFired != null)
        {
            agentInputHandler.runCommandOnWeaponFired(agentInputHandler);
        }

        AgentController agentController = (AgentController)agentInputHandler;

        agentInputHandler.weapons.timeSinceLastShot = 0.0f;

        agentController.ChangeStat(ResourceType.MagazineAmmo, -1);

        RaycastHit hit;
        if (Physics.Raycast(agentInputHandler.cameraList.agentCamera.transform.position, agentInputHandler.cameraList.agentCamera.transform.forward, out hit, agentInputHandler.weapons.currentWeapon.range))
        {       
            GameObjectWasHit(hit, agentInputHandler.weapons.currentWeapon);

        }
    }

    public void GameObjectWasHit(RaycastHit hit, Weapon weapon)
    {     
        AgentInputHandler targetAgentInputHandler = hit.transform.gameObject.GetComponent<AgentInputHandler>();
        
        if (targetAgentInputHandler != null)
        {
            if (targetAgentInputHandler.runCommandOnAgentHasBeenHit != null)
            {
                targetAgentInputHandler.runCommandOnAgentHasBeenHit(targetAgentInputHandler, hit.point, hit.normal, weapon.damage);
            }
        }
    }
}