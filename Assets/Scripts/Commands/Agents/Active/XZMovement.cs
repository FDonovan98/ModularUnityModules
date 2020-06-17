// Title: XZMovement.cs
// Author: Harry Donovan
// Date Last Edited: 05/05/2020
// Description: Handles agent movement in the XZ plane (local to the agent). Also handles velocity degradation, meaning the agent will come to a stop if no input is supplied. Triggers footsteps if the agent is moving, but will cancel them when the agent stops.

using UnityEngine;

[CreateAssetMenu(fileName = "DefaultXZMovement", menuName = "Commands/Active/XZ Movement")]
public class XZMovement : ActiveCommandObject
{
    [SerializeField]
    private KeyCode MoveForward = KeyCode.W;
    [SerializeField]
    private KeyCode MoveBack = KeyCode.S;
    [SerializeField]
    private KeyCode MoveLeft = KeyCode.A;
    [SerializeField]
    private KeyCode MoveRight = KeyCode.D;

    protected override void OnEnable()
    {
        keyTable.Add("Move Forward", MoveForward);
        keyTable.Add("Move Back", MoveBack);
        keyTable.Add("Move Left", MoveLeft);
        keyTable.Add("Move Right", MoveRight);
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
        Vector3 inputMovementVector = GetKeyInput(agent);

        if (agentInputHandler.allowInput)
        {
            inputMovementVector *= agentValues.moveAcceleration * Time.deltaTime * agentInputHandler.moveSpeedMultiplier;

            agentInputHandler.agentRigidbody.velocity += inputMovementVector * agentInputHandler.moveSpeedMultiplier;
            
            HandleFootsteps(agentInputHandler, agentValues);

        }
        else
        {
            inputMovementVector = Vector3.zero;
        }
        

        if (agentInputHandler.isGrounded)
        {
            VelocityDegradation(agentValues.velocityDegradationGrounded, inputMovementVector, agentInputHandler);
        }
        else if (agentValues.reduceVelocityInAir)
        {
            VelocityDegradation(agentValues.velocityDegradationInAir, inputMovementVector, agentInputHandler);
        }
    }

    Vector3 GetKeyInput(GameObject agent)
    {
        Vector3 inputMovementVector = Vector3.zero;

        if (Input.GetKey(MoveForward))
        {
            inputMovementVector += agent.transform.forward;
        }
        if (Input.GetKey(MoveBack))
        {
            inputMovementVector -= agent.transform.forward;
        }
        if (Input.GetKey(MoveLeft))
        {
            inputMovementVector -= agent.transform.right;
        }
        if (Input.GetKey(MoveRight))
        {
            inputMovementVector += agent.transform.right;
        }

        return inputMovementVector.normalized;
    }

    void VelocityDegradation(float velocityDegradationValue, Vector3 inputMovementVector, AgentInputHandler agentInputHandler)
    {
        if (!agentInputHandler.isJumping)
        {
            Vector3 localVel = agentInputHandler.agentRigidbody.transform.worldToLocalMatrix * agentInputHandler.agentRigidbody.velocity;
            float RelativeVelDeg = velocityDegradationValue * Time.deltaTime;
            inputMovementVector = agentInputHandler.agentRigidbody.transform.worldToLocalMatrix * inputMovementVector;

            float[] xzVel = 
            {
                localVel.x,
                localVel.z
            };

            float[] xzInput = 
            {
                inputMovementVector.x,
                inputMovementVector.z
            };

            for (int i = 0; i < 2; i++)
            {
                if (xzInput[i] == 0.0f)
                {
                    if (xzVel[i] > 0.0f)
                    {
                        xzVel[i] = Mathf.Clamp(xzVel[i] - RelativeVelDeg, 0.0f, xzVel[i]);
                    }
                    else if (xzVel[i] < 0.0f)
                    {
                        xzVel[i] = Mathf.Clamp(xzVel[i] + RelativeVelDeg, xzVel[i], 0.0f);
                    } 
                }
                else if (xzInput[i] > 0.0f && xzVel[i] < 0.0f)
                {
                    xzVel[i] = Mathf.Clamp(xzVel[i] + RelativeVelDeg, xzVel[i], 0.0f);
                }
                else if (xzInput[i] < 0.0f && xzVel[i] > 0.0f)
                {
                    xzVel[i] = Mathf.Clamp(xzVel[i] - RelativeVelDeg, 0.0f, xzVel[i]);
                }
            }

            localVel = new Vector3(xzVel[0], localVel.y, xzVel[1]);

            agentInputHandler.agentRigidbody.velocity = agentInputHandler.agentRigidbody.transform.localToWorldMatrix * localVel;
        }
    }

    void HandleFootsteps(AgentInputHandler agentInputHandler, AgentValues agentValues)
    {
        if (agentInputHandler.agentRigidbody.velocity.magnitude > 0)
            {
                if (agentInputHandler.footsteps.footstepSource != null && !agentInputHandler.footsteps.footstepSource.isPlaying)
                {
                    if (agentInputHandler.footsteps.timeSinceFootstep > agentValues.footstepDelay)
                    {
                        PlayFootstep(agentInputHandler);
                        agentInputHandler.footsteps.timeSinceFootstep = 0.0f;
                    }
                    else
                    {
                        agentInputHandler.footsteps.timeSinceFootstep += Time.deltaTime;
                    }
                }
            }
            else if (agentInputHandler.footsteps.footstepSource.clip != null && agentInputHandler.footsteps.footstepSource.isPlaying)
            {
                CancelFootstep(agentInputHandler);
            }
    }

    public void PlayFootstep(AgentInputHandler agentInputHandler)
    {
        agentInputHandler.footsteps.footstepSource.clip = agentInputHandler.GetRandomFootstepClip();
        agentInputHandler.footsteps.footstepSource.Play();
    }

    public void CancelFootstep(AgentInputHandler agentInputHandler)
    {
        agentInputHandler.footsteps.footstepSource.Stop();
        agentInputHandler.footsteps.footstepSource.clip = null;
    }
}
