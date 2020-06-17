// Title: ToggleBehaviour.cs
// Author: Harry Donovan
// Date Last Edited: 05/05/2020
// Description: Toggles agentInputHandler.behaviourToToggle on keypress.

using UnityEngine;

[CreateAssetMenu(fileName = "DefaultToggleBehaviour", menuName = "Commands/Active/ToggleBehaviour", order = 0)]
public class ToggleBehaviour : ActiveCommandObject
{
    [SerializeField]
    KeyCode toggleBehaviour = KeyCode.F;
    
    protected override void OnEnable()
    {
        keyTable.Add("Toggle Item", toggleBehaviour);
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
        if (Input.GetKeyDown(toggleBehaviour))
        {
            agentInputHandler.toggleBehaviour.behaviourToToggle.enabled = !agentInputHandler.toggleBehaviour.behaviourToToggle.isActiveAndEnabled;
        }
    }
}