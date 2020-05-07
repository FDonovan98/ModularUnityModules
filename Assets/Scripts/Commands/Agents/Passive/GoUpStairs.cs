// Title: AgentGivesHitFeedback.cs
// Author: Harry Donovan
// Date Last Edited: 07/05/2020
// Code references: https://cobertos.com/blog/post/how-to-climb-stairs-unity3d/
// Description: Checks colliders to detect a stair. A contact point will flag as a stair if it is in the direction of movement, it is nearly entirely vertical, and the step height is below agentValues.maxStepHeight.
// Improvements: Allow stepping up contact points with a non-horizontal normal.

using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "DefaultGoUpStairs", menuName = "Commands/Passive/Go Up Stairs")]
public class GoUpStairs : PassiveCommandObject
{
    List<ContactPoint> allCPs = new List<ContactPoint>();
    public override void RunCommandOnStart(AgentInputHandler agentInputHandler)
    {
        agentInputHandler.runCommandOnCollisionEnter += RunCommandOnCollisionEnter;
        agentInputHandler.runCommandOnCollisionStay += RunCommandOnCollisionStay;
        agentInputHandler.runCommandOnFixedUpdate += RunCommandOnFixedUpdate;
    }

    void RunCommandOnFixedUpdate(GameObject agent, AgentInputHandler agentInputHandler, AgentValues agentValues)
    {
        Vector3 stepUpOffset;

        if (agentInputHandler.isGrounded)
        {
            if (FindStair(out stepUpOffset, agent, agentInputHandler.agentRigidbody, agentInputHandler, agentValues))
            {
                agentInputHandler.agentRigidbody.position += stepUpOffset;
                agentInputHandler.agentRigidbody.velocity = agentInputHandler.lastVelocity;
            }
        }

        agentInputHandler.lastVelocity = agentInputHandler.agentRigidbody.velocity;
        allCPs.Clear();
    }

    bool FindStair(out Vector3 stepUpOffset, GameObject agent, Rigidbody agentRigidbody, AgentInputHandler agentInputHandler, AgentValues agentValues)
    {
        stepUpOffset = Vector3.zero;

        Vector2 agentXZVel = new Vector2(agentRigidbody.velocity.x, agentRigidbody.velocity.z).normalized;

        if (agentXZVel.sqrMagnitude > 0.0f)
        {
            foreach (ContactPoint element in allCPs)
            {
                if (CheckForStair(out stepUpOffset, agent, element, agentValues, agentXZVel, agentInputHandler))
                {
                    return true;
                }
            }
            return false;
        }
        else
        {
            return false;
        }
    }

    bool CheckForStair(out Vector3 stepUpOffset, GameObject agent, ContactPoint contactPoint, AgentValues agentValues, Vector2 agentXZVel, AgentInputHandler agentInputHandler)
    {
        stepUpOffset = Vector3.zero;
        // Should be changed to check for angle between horizontal and normal.
        if (Mathf.Abs(contactPoint.normal.y) > 0.01f)
        {
            return false;
        }

        if (contactPoint.point.y - agentInputHandler.groundContactPoint.point.y > agentValues.maxStepHeight)
        {
            return false;
        }

        // Overcast is sent in direction of player movement.
        RaycastHit hit;
        float rayOriginHeight = agentInputHandler.groundContactPoint.point.y + agentValues.maxStepHeight;
        Vector3 overshootDirection = new Vector3(agentXZVel.x, 0.0f, agentXZVel.y);
        Vector3 rayOrigin = new Vector3(contactPoint.point.x, rayOriginHeight, contactPoint.point.z);
        rayOrigin += overshootDirection * agentValues.stepSearchOvershoot;

        Ray ray = new Ray(rayOrigin, agentInputHandler.gravityDirection);

        if (!(contactPoint.otherCollider.Raycast(ray, out hit, agentValues.maxStepHeight)))
        {
            return false;
        }

        stepUpOffset = new Vector3(0.0f, hit.point.y + 0.0001f - agentInputHandler.groundContactPoint.point.y, 0.0f);
        stepUpOffset += overshootDirection * agentValues.stepSearchOvershoot;

        return true;
    }

    void RunCommandOnCollisionEnter(GameObject agent, AgentInputHandler agentInputHandler, AgentValues agentValues, Collision other)
    {
        allCPs.AddRange(other.contacts);
    }

    void RunCommandOnCollisionStay(GameObject agent, AgentInputHandler agentInputHandler, AgentValues agentValues, Collision other)
    {
        allCPs.AddRange(other.contacts);
    }
}