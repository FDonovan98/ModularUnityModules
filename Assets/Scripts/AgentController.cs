﻿using UnityEngine;
using TMPro;
using System.Collections;

using UnityEngine.UI;

public class AgentController : AgentInputHandler
{
    public GameObject deathScreen;
    public Color alienVision;
    public bool specialVision = false;

    [Header("UI")]
    public TextMeshProUGUI healthUIText;
    public TextMeshProUGUI ammoUIText;
    public GameObject oxygenDisplay;

    [Header("Current Stats")]
    [ReadOnly]
    public float currentHealth = 0.0f;
    [ReadOnly]
    public float currentOxygen = 0.0f;
    [ReadOnly]
    public int currentExtraAmmo = 0;
    [ReadOnly]
    public int currentBulletsInMag = 0;

    public enum ResourceType
    {
        Health,
        MagazineAmmo,
        ExtraAmmo,
        Oxygen
    }
    
    public GameObject[] gameObjectsToDisableForPhoton;
    public Behaviour[] componentsToDisableForPhoton;

    private void Awake()
    { 
        if (agentValues != null)
        {
            currentOxygen = agentValues.maxOxygen;
            currentHealth = agentValues.maxHealth;
        }

        if (currentWeapon != null)
        {
            currentBulletsInMag = currentWeapon.bulletsInCurrentMag;
            currentExtraAmmo = currentWeapon.magSize * 2;
            timeSinceLastShot = currentWeapon.fireRate;
        }

        UpdateUI();
    }

    public void ChangeResourceCount(ResourceType resourceType, int value)
    {
        if (resourceType == ResourceType.MagazineAmmo)
        {
            currentBulletsInMag = (int)Mathf.Clamp(currentBulletsInMag + value, 0.0f, currentWeapon.magSize);

            if (ammoUIText != null)
            {
                UpdateUI(ResourceType.MagazineAmmo);
            }
        }

        if (resourceType == ResourceType.ExtraAmmo)
        {
            currentExtraAmmo = (int)Mathf.Max(currentExtraAmmo + value, 0.0f);

            if (ammoUIText != null)
            {
                UpdateUI(ResourceType.MagazineAmmo);
            }
        }
    }

    public void ChangeResourceCount(ResourceType resourceType, float value)
    {
        if (resourceType == ResourceType.Health)
        {
            currentHealth = Mathf.Clamp(currentHealth + value, 0.0f, agentValues.maxHealth);
            if (currentHealth <= 0)
            {
                AgentHasDied();
            }

            if (healthUIText != null)
            {
                UpdateUI(ResourceType.Health);
            }
        }
        else if (resourceType == ResourceType.Oxygen)
        {
            currentOxygen = Mathf.Clamp(currentOxygen + value, 0.0f, agentValues.maxOxygen);

            if (currentOxygen == 0.0f)
            {
                ChangeResourceCount(AgentController.ResourceType.Health, -(agentValues.suffocationDamage * Time.deltaTime));
            }

            if (oxygenDisplay != null)
            {
                UpdateUI(ResourceType.Oxygen);
            }
        }
    }

    void UpdateUI()
    {
        UpdateAmmoUI();
        UpdateHealthUI();
        UpdateOxygenUI();
    }

    void UpdateUI(ResourceType resourceType)
    {
        switch (resourceType)
        {
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
            default:
                Debug.LogWarning(gameObject.name + " tried to update UI of unrecognized type.");
                break;
        }
    }

    void UpdateAmmoUI()
    {
        if (ammoUIText != null)
        {
            ammoUIText.text = "Ammo: " + currentBulletsInMag + " / " + currentExtraAmmo;
        }
    }

    void UpdateHealthUI()
    {
        if (healthUIText != null)
        {
            healthUIText.text = "Health: " + Mathf.RoundToInt(currentHealth / agentValues.maxHealth * 100);
        }
    }

    void UpdateOxygenUI()
    {
        if (oxygenDisplay != null)
        {
            Slider oxygenSlider = oxygenDisplay.GetComponentInChildren<Slider>();
            TextMeshProUGUI oxygenText = oxygenDisplay.GetComponentInChildren<TextMeshProUGUI>();
            oxygenSlider.value = currentOxygen / agentValues.maxOxygen * 100;
            oxygenText.text = (Mathf.Round(currentOxygen / agentValues.maxOxygen * 100)).ToString();
        }
    }

    /// <summary>
    /// Disables the player's input, enables rotations in the rigidbody, adds a random force to the
    /// rigidbody, and starts the 'Death' coroutine.
    /// </summary>
    public void AgentHasDied()
    {
        allowInput = false;
        agent.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        agent.GetComponent<Rigidbody>().AddForceAtPosition(RandomForce(150f), transform.position);
        StartCoroutine(Death(agent));
    }
    
    /// <summary>
    /// Returns a vector with all axes having a random value between 0 and the 'velocity' parameter.
    /// </summary>
    /// <param name="velocity">The maximum random force.</param>
    /// <returns>Returns a vector with all axes having a random value between 0 and the 'velocity' parameter.</returns>
    private Vector3 RandomForce(float velocity)
    {
        return new Vector3(Random.Range(0, velocity), Random.Range(0, velocity), Random.Range(0, velocity));
    }
    
    private IEnumerator Death(GameObject player)
    {
        yield return new WaitForSeconds(3f);
        
        deathScreen.SetActive(true);

        yield return new WaitForSeconds(3f);

        deathScreen.SetActive(false);
    }

}