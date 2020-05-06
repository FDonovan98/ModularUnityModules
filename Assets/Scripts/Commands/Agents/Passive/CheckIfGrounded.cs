// Title: CheckIfGrounded.cs
// Author: Harry Donovan
// Date Last Edited: 05/05/2020
// Description: Runs through the agents contact points, checking to see if any contact point normals are within agentValues.slopeLimitAngle degrees of -agentInputHandler.gravityDirection, returning the contact point closest to zero degrees. If none are within the angle then an average of all the contact points are taken, and this normal is checked against agentInputHandler.gravityDirection.

using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "DefaultCheckIfGrounded", menuName = "Commands/Passive/Check If Grounded")]
public class CheckIfGrounded : PassiveCommandObject
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
        bool foundGround = false;
        ContactPoint currentGround = new ContactPoint();
        float currentGroundTheta = float.MaxValue;

        foreach (ContactPoint element in allCPs)
        {
            float cosTheta = Vector3.Dot(element.normal, agentInputHandler.gravityDirection);
            float theta = Mathf.Abs(Mathf.Acos(cosTheta) * Mathf.Rad2Deg - 180);
            
            // Catches bug cause when cosTheta == -1.
            if (float.IsNaN(theta))
            {
                theta = 0.0f;
            }

            if (theta < agentValues.slopeLimitAngle && theta < currentGroundTheta)
            {
                foundGround = true;
                currentGround = element;
                currentGroundTheta = theta;
            }
        }

        if (!foundGround)
        {
            Vector3 averageNormal = Vector3.zero;

            foreach (ContactPoint element in allCPs)
            {
                averageNormal += element.normal;
            }

            averageNormal = averageNormal.normalized;

            float cosTheta = Vector3.Dot(averageNormal, agentInputHandler.gravityDirection);
            float theta = Mathf.Abs(Mathf.Acos(cosTheta) * Mathf.Rad2Deg - 180);
            
            // Catches bug cause when cosTheta == -1.
            if (float.IsNaN(theta))
            {
                theta = 0.0f;
            }

            if (theta < agentValues.slopeLimitAngle && theta < currentGroundTheta)
            {
                foundGround = true;
                currentGround = default(ContactPoint);
                currentGroundTheta = theta;
            }
        }

        if (!foundGround && agentInputHandler.currentLeapCharge > 0)
        {
            agentInputHandler.currentLeapCharge = 0.0f;
        }
        
        agentInputHandler.isGrounded = foundGround;
        agentInputHandler.groundContactPoint = currentGround;
        allCPs.Clear();
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