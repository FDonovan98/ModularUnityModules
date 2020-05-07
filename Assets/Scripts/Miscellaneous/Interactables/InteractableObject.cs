// Title: InteractableObject.cs
// Author: Harry Donovan
// Date Last Edited: 07/05/2020
// Description: Abstract class intended for anything that can be interacted with by an agent. The agent must have an CanInteractWithObjects.cs component to be able to interact.

using UnityEngine;
using UnityEngine.UI;

public abstract class InteractableObject : MonoBehaviour
{
    public string interactionPrompt = "Press E to interact";
    // The time needed to interact with the object to activate/open it.
    [SerializeField] private float interactTime;
    // Should the debug messages be displayed.
    [SerializeField] protected bool debug = false; 
    // How long the player has been pressing the interact key.
    private float currentInteractionTime = 0f; 
    // Is the interaction complete?
    public bool interactionComplete = false;

    public void resetInteraction(AgentInputHandler agentInputHandler)
    {
        currentInteractionTime = 0.0f;
        UpdateProgressBar(agentInputHandler.progressBar, currentInteractionTime);
    }

    public void ChangeCurrentInteractionTime(AgentInputHandler agentInputHandler, float value)
    {
        currentInteractionTime += value;

        if (agentInputHandler.progressBar != null)
        {
            UpdateProgressBar(agentInputHandler.progressBar, currentInteractionTime);
        }

        if (!interactionComplete && currentInteractionTime >= interactTime)
        {
            interactionComplete = true;
            agentInputHandler.allowInput = true;
        }
    }

    public void UpdateProgressBar(Image progressBar, float value)
    {
        progressBar.fillAmount = (value / interactTime);
        if (value >= 100)
        {
            progressBar.fillAmount = 0;
        }
    }

    public virtual void LeftArea(AgentInputHandler agentInputHandler)
    {
        currentInteractionTime = 0.0f;
        interactionComplete = false;
        UpdateProgressBar(agentInputHandler.progressBar, currentInteractionTime);
    }

    protected abstract void InteractionComplete();
}