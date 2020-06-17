// Title: AgentUIController.cs
// Author: Harry Donovan
// Date Last Edited: 07/05/2020
// Description: Handles all of the UI updates for the agent. Hooks into the updateUI delegate in AgentController.cs for all of its calls.

using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class HealthUIStruct
{
    public TextMeshProUGUI healthUIText;
    public Image healthUIImage;
}

[System.Serializable]
public class OxygenUIStruct
{
    public TextMeshProUGUI oxygenUIText;
    public Image oxygenUIImage;
    public GameObject lowOxygenUIObject;
    public GameObject oxyIsRegeningObject;
}

[System.Serializable]
public class AbilitySymbolUI
{
    public GameObject ActiveSymbol;
    public GameObject InactiveSymbol;
}

public class AgentUIController : MonoBehaviour
{
    public AgentController agentController;

    public HealthUIStruct healthUI;

    public TextMeshProUGUI ammoUIText;

    public OxygenUIStruct oxygenUI;

    public AbilitySymbolUI wallClimbingUI;

    public AbilitySymbolUI emergencyRegenUI;

    public AbilitySymbolUI specialVision;

    private void OnEnable()
    {
        agentController.updateUI += UpdateUI;
    }

    private void UpdateUI(ResourceType resourceType)
    {
        switch (resourceType)
        {
            case ResourceType.None:
                UpdateUI();
                break;

            case ResourceType.MagazineAmmo:
            case ResourceType.ExtraAmmo:
                UpdateAmmoUI();
                break;

            case ResourceType.Health:
                UpdateHealthUI();
                break;

            case ResourceType.Oxygen:
                UpdateOxygenUI();
                break;

            case ResourceType.WallClimbing:
                UpdateWallClimbingUI();
                break;
            
            case ResourceType.LowOxygen:
                UpdateLowOxygenUI();
                break;

            case ResourceType.EmergencyRegen:
                UpdateEmergencyRegenUI();
                break;

            case ResourceType.OxygenRegen:
                UpdateOxygenRegenUI();
                break;

            case ResourceType.AlienVision:
                UpdateAlienVisionUI();
                break;

            default:
                Debug.LogWarning(gameObject.name + " tried to update UI of unrecognized type.");
                break;
        }
    }

    void UpdateUI()
    {
        UpdateAmmoUI();
        UpdateHealthUI();
        UpdateOxygenUI();
        UpdateWallClimbingUI();
        UpdateEmergencyRegenUI();
        UpdateLowOxygenUI();
        UpdateOxygenRegenUI();
        UpdateAlienVisionUI();
    }

    void UpdateAlienVisionUI()
    {
        if (specialVision.ActiveSymbol != null)
        {
            specialVision.ActiveSymbol.SetActive(agentController.alienVisionIsActive);
        }

        if (specialVision.InactiveSymbol != null)
        {
            specialVision.InactiveSymbol.SetActive(!agentController.alienVisionIsActive);
        }
    }

    void UpdateOxygenRegenUI()
    {
        oxygenUI.oxyIsRegeningObject.SetActive(agentController.oxygenIsRegenerating);
    }

    void UpdateLowOxygenUI()
    {
        oxygenUI.lowOxygenUIObject.SetActive(agentController.lowOxygen);
    }

    void UpdateEmergencyRegenUI()
    {
        if (emergencyRegenUI.ActiveSymbol != null && emergencyRegenUI.InactiveSymbol != null)
        {
            if (agentController.emergencyRegenUsesRemaining <= 0)
            {
                emergencyRegenUI.ActiveSymbol.SetActive(true);
                emergencyRegenUI.InactiveSymbol.SetActive(false);
            }
        }
    }

    private void UpdateAmmoUI()
    {
        if (ammoUIText != null)
        {
            ammoUIText.text = agentController.currentBulletsInMag + " / " + agentController.currentExtraAmmo;
        }
    }

	void UpdateWallClimbingUI()
    {
        if (wallClimbingUI.ActiveSymbol != null)
        {
            wallClimbingUI.InactiveSymbol.SetActive(agentController.isWallClimbing);
        }
        
        if (wallClimbingUI.ActiveSymbol != null)
        {
            wallClimbingUI.InactiveSymbol.SetActive(!agentController.isWallClimbing);
        }
    }

    private void UpdateHealthUI()
    {
        if (healthUI.healthUIText != null)
        {
            healthUI.healthUIText.text = Mathf.RoundToInt(agentController.currentHealth / agentController.agentValues.maxHealth * 100).ToString() + "%";
        }

        if (healthUI.healthUIImage != null)
        {
            healthUI.healthUIImage.fillAmount = agentController.currentHealth / agentController.agentValues.maxHealth;
        }
    }

    private void UpdateOxygenUI()
    {
        if (oxygenUI.oxygenUIText != null)
        {
            oxygenUI.oxygenUIText.text = Mathf.RoundToInt(agentController.oxygen.currentOxygen / agentController.agentValues.maxOxygen * 100).ToString() + "%";
        }

        if (oxygenUI.oxygenUIImage != null)
        {
            oxygenUI.oxygenUIImage.fillAmount = agentController.oxygen.currentOxygen / agentController.agentValues.maxOxygen;
        }
    }
}
