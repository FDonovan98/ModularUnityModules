// Title: AgentController.cs
// Author: Harry Donovan
// Date Last Edited: 07/05/2020
// Description: Stores current variables for the agent that change at run time. Contains functions to change these variables, as well as a delegate to enable UI updates.

using UnityEngine;

public enum ResourceType
{
    None,
    Health,
    MagazineAmmo,
    ExtraAmmo,
    Oxygen,
    WallClimbing,
    EmergencyRegen,
    LowOxygen,
    OxygenRegen,
    AlienVision
}

[System.Serializable]
public class EmergencyRegenStrut
{
    public AudioClip emergencyRegenAudio;
    public GameObject emergencyRegenParticleSystem;
    public GameObject emergencyRegenParticleSystems;
}

public class OxygenStruct
{
    public AudioClip oxygenWarningAudio = null;
    public float oxygenWarningDingStartRate = 2.0f;
    public float timeInLowOxygen = float.MaxValue;
    [HideInInspector]
    public float currentOxygen = 0.0f;
    [Range(0, 100)]
    public int oxygenWarningAmount = 30;
}

public class AgentController : AgentInputHandler
{
    public EmergencyRegenStrut emergencyRegen;

    public OxygenStruct oxygen;

    [Header("Current Stats")]
    [HideInInspector]
    public float currentHealth = 0.0f;
    [HideInInspector]
    public bool lowOxygen = false;
    [HideInInspector]
    public int currentExtraAmmo = 0;
    [HideInInspector]
    public int currentBulletsInMag = 0;
    [HideInInspector]
    public bool emergencyRegenActive = false;
    [HideInInspector]
    public int emergencyRegenUsesRemaining = 0;
    [HideInInspector]
    public bool isWallClimbing = false;
    [HideInInspector]
    public bool oxygenIsRegenerating = false;
    [HideInInspector]
    public bool alienVisionIsActive = false;

    public delegate void UpdateUI(ResourceType resourceType = ResourceType.None);
    public UpdateUI updateUI;

    private void Awake()
    {
        agentController = this;
        updateUI += TriggerEffectsOnStatChange;

        if (agentValues != null)
        {
            oxygen.currentOxygen = agentValues.maxOxygen;
            currentHealth = agentValues.maxHealth;
            emergencyRegenUsesRemaining = agentValues.emergencyRegenUses;
        }

        if (weapons.currentWeapon != null)
        {
            currentBulletsInMag = weapons.currentWeapon.bulletsInCurrentMag;
            currentExtraAmmo = weapons.currentWeapon.magSize * 2;
            weapons.timeSinceLastShot = weapons.currentWeapon.fireRate;
        }

        if (updateUI != null)
        {
            updateUI();
        }
    }

    void TriggerEffectsOnStatChange(ResourceType resourceType)
    {
        switch (resourceType)
        {
            case ResourceType.Oxygen:
                if (lowOxygen && oxygen.oxygenWarningAudio != null)
                {
                    oxygen.timeInLowOxygen += Time.deltaTime;
                    float boundryTime = oxygen.currentOxygen / agentValues.maxOxygen;
                    boundryTime /= (oxygen.oxygenWarningAmount / agentValues.maxOxygen);
                    boundryTime *= oxygen.oxygenWarningDingStartRate + oxygen.oxygenWarningAudio.length;

                    if (oxygen.timeInLowOxygen > boundryTime)
                    {
                        mainAudioSource.PlayOneShot(oxygen.oxygenWarningAudio);
                        oxygen.timeInLowOxygen = 0.0f;
                    }
                }
                break;

            default:
                break;
        }
    }

    public override void ChangeWeapon(Weapon weapon)
    {
        base.ChangeWeapon(weapon);

        currentBulletsInMag = weapons.currentWeapon.bulletsInCurrentMag;
        currentExtraAmmo = weapons.currentWeapon.magSize * 2;

        if (updateUI != null)
        {
            updateUI(ResourceType.ExtraAmmo);
        }

    }

    public void ChangeStat(ResourceType resourceType, bool value)
    {
        if (resourceType == ResourceType.WallClimbing)
        {
            isWallClimbing = value;

            if (updateUI != null)
            {
                updateUI(ResourceType.WallClimbing);
            }
        }

        if (resourceType == ResourceType.EmergencyRegen)
        {
            emergencyRegenActive = value;

            if (updateUI != null)
            {
                updateUI(ResourceType.EmergencyRegen);
            }
        }

        if (resourceType == ResourceType.AlienVision)
        {
            alienVisionIsActive = value;
            updateUI(ResourceType.AlienVision);
        }
    }

    public void ChangeStat(ResourceType resourceType, int value)
    {
        if (resourceType == ResourceType.MagazineAmmo)
        {
            currentBulletsInMag = (int)Mathf.Clamp(currentBulletsInMag + value, 0.0f, weapons.currentWeapon.magSize);

            if (updateUI != null)
            {
                updateUI(ResourceType.ExtraAmmo);
            }
        }

        if (resourceType == ResourceType.ExtraAmmo)
        {
            currentExtraAmmo = (int)Mathf.Max(currentExtraAmmo + value, 0.0f);

            if (updateUI != null)
            {
                updateUI(ResourceType.ExtraAmmo);
            }
        }
    }

    public void ChangeStat(ResourceType resourceType, float value)
    {
        if (resourceType == ResourceType.Health)
        {
            currentHealth += value * equippedArmour.damageMultiplier;

            if (updateUI != null)
            {
                updateUI(ResourceType.Health);
            }
        }
        else if (resourceType == ResourceType.Oxygen)
        {
            oxygen.currentOxygen = Mathf.Clamp(oxygen.currentOxygen + value, 0.0f, agentValues.maxOxygen);

            if (value > 0)
            {
                if (lowOxygen && oxygen.currentOxygen > oxygen.oxygenWarningAmount)
                {
                    lowOxygen = false;
                    oxygen.timeInLowOxygen = float.MaxValue;

                    if (updateUI != null)
                    {
                        updateUI(ResourceType.LowOxygen);
                    }
                }
            }
            else
            {
                if (oxygen.currentOxygen == 0.0f)
                {
                    ChangeStat(ResourceType.Health, -(agentValues.suffocationDamage * Time.deltaTime));
                }
                else if (!lowOxygen && oxygen.currentOxygen <= oxygen.oxygenWarningAmount)
                {
                    lowOxygen = true;

                    if (updateUI != null)
                    {
                        updateUI(ResourceType.LowOxygen);
                    }
                }
            }

            if (updateUI != null)
            {
                updateUI(ResourceType.Oxygen);
            }
        }
    }
}