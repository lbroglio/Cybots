using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Playables;

public class Cybot : MonoBehaviour
{
    public static float LineWidth = 0.05f;

    public float WalkSpeed = 5;
    public float RunSpeed = 8f;

    // Track the location of this cybots eyes
    Transform leftEye;
    Transform rightEye;


    // The current behavior this cybot is executing
    protected CybotBehavior currentBehavior;

    // The map 
    protected MapBuilder map;

    // Store the return value of the last do action call
    protected int doActionRet;

    // The behavior this cybot had last frame
    private CybotBehavior lastBehavior = new DefaultBehavior();

    // Start is called before the first frame update
    protected virtual void Start()
    {
        // Get and set map
        map = GameObject.Find("MapBuilder").GetComponent<MapBuilder>();

        // Spawn at maze center 
        System.Drawing.Point center = new System.Drawing.Point(map.MazeWidth / 2, map.MazeHeight / 2);

        transform.position = map.SpaceLocToCoords(center);

        currentBehavior = new DefaultBehavior();

        // Get eyes and add LineRenderers
        leftEye = transform.Find("mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:Neck/mixamorig:Head/mixamorig:HeadTop_End/LeftEye");
        rightEye = transform.Find("mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:Neck/mixamorig:Head/mixamorig:HeadTop_End/RightEye");

        //Add and configure line renders
        Material lineMtl = Resources.Load("Materials/IceBlue") as Material;
        LineRenderer leftLR = leftEye.AddComponent<LineRenderer>();
        leftLR.material = lineMtl;
        leftLR.startWidth = LineWidth;
        leftLR.startWidth = LineWidth;

        LineRenderer rightLR = rightEye.AddComponent<LineRenderer>();
        rightLR.material = lineMtl;
        rightLR.startWidth = LineWidth;
        rightLR.startWidth = LineWidth;
        //rightEye.AddComponent<LineRenderer>();

    }

    // Update is called once per frame
    protected virtual void Update()
    {
        // If the behavior last frame wasn't chase (or capture) but it is now
        if ((currentBehavior.GetType() == typeof(Chase) || currentBehavior.GetType().IsSubclassOf(typeof(Chase)) || currentBehavior.GetType() == typeof(CapturePlayer)) 
            && (lastBehavior.GetType() != typeof(Chase) && lastBehavior.GetType() != typeof(CapturePlayer) && !lastBehavior.GetType().IsSubclassOf(typeof(Chase))))
        {
            // Set that this cybot is in a chase
            GameController gameCon = GameObject.Find("GameController").GetComponent<GameController>();
            gameCon.amChasing();
        }

        // If the behavior last frame was a chase but it is now
        if ((currentBehavior.GetType() != typeof(Chase) && currentBehavior.GetType() != typeof(CapturePlayer) && !currentBehavior.GetType().IsSubclassOf(typeof(Chase))) 
            && (lastBehavior.GetType() == typeof(Chase) || lastBehavior.GetType() == typeof(CapturePlayer) || lastBehavior.GetType().IsSubclassOf(typeof(Chase))))
        {
            // Set that this cybot is in a chase
            GameController gameCon = GameObject.Find("GameController").GetComponent<GameController>();
            gameCon.stopChasing();
        }

        // Control Animations
        if (currentBehavior.MoveType == CybotBehavior.CybotMoveTypes.WALK)
        {
            GetComponent<Animator>().SetFloat("MoveSpeed", WalkSpeed);
            GetComponent<Animator>().SetBool("isCapturing", false);

            //gameObject.transform.position += transform.rotation * Vector3.forward * WalkSpeed * Time.deltaTime;
        }
        else if(currentBehavior.MoveType == CybotBehavior.CybotMoveTypes.RUN)
        {
            GetComponent<Animator>().SetFloat("MoveSpeed", RunSpeed);
            GetComponent<Animator>().SetBool("isCapturing", false);


            //gameObject.transform.position += transform.rotation * Vector3.forward * RunSpeed * Time.deltaTime;
        }
        else if(currentBehavior.MoveType == CybotBehavior.CybotMoveTypes.CAPTURING)
        {
            GetComponent<Animator>().SetFloat("MoveSpeed", 0);
            GetComponent<Animator>().SetBool("isCapturing", true);

        }
        else
        {
            GetComponent<Animator>().SetFloat("MoveSpeed", 0);
            GetComponent<Animator>().SetBool("isCapturing", false);

        }


        // Cast eye beams
        

        // Cast rays from eyes
        Ray leftRay = new Ray(leftEye.position, leftEye.forward);
        RaycastHit leftHitInfo;
        Physics.Raycast(leftRay, out leftHitInfo);

        Ray rightRay = new Ray(rightEye.position, rightEye.forward);
        RaycastHit rightHitInfo;
        Physics.Raycast(rightRay, out rightHitInfo);

        // Draw rays
        LineRenderer leftLR = leftEye.GetComponent<LineRenderer>();
        leftLR.positionCount = 2;
        leftLR.SetPosition(0, leftEye.position);
        leftLR.SetPosition(1, leftHitInfo.point);

        LineRenderer rightLR = rightEye.GetComponent<LineRenderer>();
        rightLR.positionCount = 2;
        rightLR.SetPosition(0, rightEye.position);
        rightLR.SetPosition(1, rightHitInfo.point);


        // Freezer player if hit

        // Get the player
        GameObject player = GameObject.Find("FirstPersonRig(Clone)");

        // Check if the player was hit 
                    
        if (leftHitInfo.collider == player.GetComponent<Collider>() || rightHitInfo.collider == player.GetComponent<Collider>())
        {
            player.GetComponent<Player>().Freeze();
        }


        // Set this frames behavior
        lastBehavior = currentBehavior;


        // Pass control to behavioor
        doActionRet = currentBehavior.DoAction(this);


    }


    /// <summary>
    /// Returns true if this cybot can "see" the player
    /// </summary>
    /// <returns></returns>
    public bool canSeePlayer()
    {

        // Get the player
        GameObject player = GameObject.Find("FirstPersonRig(Clone)");

        // Cast a ray to the player to see if the cybot can see it
        Transform targeter = transform.Find("RayTargeter");
        targeter.LookAt(player.transform);
        RaycastHit hitInfo;
        Physics.Raycast(transform.position, targeter.forward, out hitInfo);

        // Calculate distance between player and cybot
        float dist = Vector3.Distance(player.transform.position, transform.position);

        // Return true if the player was hit or if close enough it doesn't matter
        if (hitInfo.collider == player.GetComponent<Collider>() || dist < 1)
        {
            return true;
        }

        // Otherwise return false
        return false;
    }

}

public abstract class CybotBehavior
{
    public enum CybotMoveTypes
    {
        IDLE,
        CAPTURING,
        WALK,
        RUN,
        NONE
    }

    public CybotMoveTypes MoveType;

    /// <summary>
    /// Have a behavior control the cybot and perform its action
    /// </summary>
    public abstract int DoAction(Cybot cybot);
}

/// <summary>
/// Cybot behavior which only idles.
/// </summary>
public class DefaultBehavior : CybotBehavior
{

    public DefaultBehavior()
    {
        MoveType = CybotMoveTypes.IDLE;
    }

    public override int DoAction(Cybot cybot)
    {
        return 0;
    }

}

/// <summary>
/// Behavior for a cybot which is chasing the player
/// </summary>
public class Chase : CybotBehavior
{
    // Possible return values of the Do action for this behavior. Determines state transition
    public enum StateVals
    {
        /// <summary>
        /// Stay in the chase state
        /// </summary>
        NO_CHANGE,
        /// <summary>
        /// Change state because the player is at of sight and the cybot is at its last known location
        /// </summary>
        LOST_PLAYER,
        /// <summary>
        /// Chnage state because this cybot is within capture distance of the player
        /// </summary>
        AT_PLAYER
    }


    // Track the last known location of the player
    private Vector3 lastKnownLoc;

    public Chase()
    {
        MoveType = CybotMoveTypes.RUN;
    }

    public override int DoAction(Cybot cybot)
    {
        // Set speed to move at
        float moveSpeed = cybot.RunSpeed;

        if(MoveType == CybotMoveTypes.WALK)
        {
            moveSpeed = cybot.WalkSpeed;
        }


        // Check if the cybot can see the player

        // Get the player
        GameObject player = GameObject.Find("FirstPersonRig(Clone)");

        // If the cybot can see player
        if (cybot.canSeePlayer())
        {

            // Set last known location and pathfind to the player
            lastKnownLoc = new Vector3(player.transform.position.x, 0, player.transform.position.z);

            // Have the cybot face the player
            cybot.transform.LookAt(player.transform.position);

            // Move forward
            cybot.transform.position += cybot.transform.rotation * Vector3.forward * moveSpeed * Time.deltaTime;


            //Calculate distance to player
            float dist = Vector3.Distance(player.transform.position, cybot.transform.position);

            // If this cybot is within capture distance of the player return state change indicator
            if (dist < 1.5f && player.GetComponent<Player>().getFrozen())
            {
                Debug.Log("At Player");
                return ((int) StateVals.AT_PLAYER);
            }
            else
            {
                return (int) StateVals.NO_CHANGE;
            }

        }
        // If the cybot can't see the player and is at the last known location
        else if (Vector3.Distance(cybot.transform.position, lastKnownLoc) < 1.5f)
        {

            // Return state change for having lost player
            return ((int)StateVals.LOST_PLAYER);
        }
        // Otherwise move towards last known location
        else
        {
            // Have the cybot face the player
            cybot.transform.LookAt(lastKnownLoc);

            // Move forward
            cybot.transform.position += cybot.transform.rotation * Vector3.forward * moveSpeed * Time.deltaTime;

            // Return no state change
            return (int)StateVals.NO_CHANGE;
        }

    }


}



/// <summary>
/// Behavior for a cybot which is moving to a point on the map
/// </summary>
public class PathfindToPoint : CybotBehavior
{

    // Possible return values of the Do action for this behavior. Determines state transition
    public enum StateVals
    {
        /// <summary>
        /// Stay in the pathfinding state
        /// </summary>
        NO_CHANGE,
        /// <summary>
        /// Change state because the target point has been reached
        /// </summary>
        AT_POINT,
        /// <summary>
        /// Change state because the player is in sight
        /// </summary>
        SEES_PLAYER
    }


    // Set the map this behavior pathfinding on
    MapBuilder map;

    private MapGraph.UGraphNode targetPoint;
    // The current point being moved to 
    private MapGraph.UGraphNode currentPoint;
    // Track the path to take to the target
    private Stack<MapGraph.UGraphNode> path;



    internal PathfindToPoint(MapBuilder map, MapGraph.UGraphNode targetPoint)
    {
        //Debug.Log("Starting Pathfind");
        MoveType = CybotMoveTypes.WALK;
        this.targetPoint = targetPoint;
        this.currentPoint = null;
        path = null;
        this.map = map;
    }

    public override int DoAction(Cybot cybot)
    {
        float moveSpeed = cybot.WalkSpeed;
        // Set how fast to move between points (Used by subclasses who run)
        if(MoveType == CybotMoveTypes.RUN) {
            moveSpeed = cybot.RunSpeed;
        }


        // If not in position move towards the position. Behavior knows its in position once the path stack is initialized
        if (path == null)
        {
            // Set current point if needed
            if(currentPoint == null)
            {
                currentPoint = map.CoordsToNode(cybot.transform.position);
            }
     
            // Get the coords
            Vector3 coords = map.SpaceLocToCoords(new System.Drawing.Point(currentPoint.xLoc, currentPoint.yLoc));
            coords += new Vector3(-1, 0, 1);

            // If sufficiently close to the coords mark as in position and start pathfinding
            // Calculate dist between the cybot and coords
            Vector3 coords2D = new Vector3(coords.x, 0, coords.z);
            Vector3 cb2D = new Vector3(cybot.transform.position.x, 0, cybot.transform.position.z);

            if (Vector3.Distance(cb2D, coords2D) < 0.5f)
            {
                // Build path to dest
                path = Pathfinding.shortestPathToNode(currentPoint, targetPoint); 
                
                // Pop first because its already current point
                path.Pop();
            }
            else
            {
                // Face the coords 
                cybot.transform.LookAt(coords);

                //cybot.transform.rotation = Quaternion.Euler(new Vector3(0, cybot.transform.rotation.eulerAngles.y, cybot.transform.rotation.eulerAngles.z));

                // Move towards starting position
                cybot.transform.position += cybot.transform.rotation * Vector3.forward * moveSpeed * Time.deltaTime;
            }


            return (int) StateVals.NO_CHANGE;
        }
        else
        { 
            Vector3 currNodeCoords = map.SpaceLocToCoords(new System.Drawing.Point(currentPoint.xLoc, currentPoint.yLoc));
            // Shift coords to target center of square
            currNodeCoords += new Vector3(-1, 0, 1);

            // If within target distance of the current node
            Vector3 coords2D = new Vector3(currNodeCoords.x, 0, currNodeCoords.z);
            Vector3 cb2D = new Vector3(cybot.transform.position.x, 0, cybot.transform.position.z);
            if (Vector3.Distance(cb2D, coords2D) < 0.5f)
            {
                //Debug.Log("HERE");
     
                // If at target switch to sentry mode
                if(currentPoint == targetPoint)
                {
                    //Debug.Log("Ending PathFind");
                    return (int)StateVals.AT_POINT;
                }
                
                // Otherwise switch to next point
                currentPoint = path.Pop();

                // Get coords for new point
                currNodeCoords = map.SpaceLocToCoords(new System.Drawing.Point(currentPoint.xLoc, currentPoint.yLoc));
                // Shift coords to target center of square
                currNodeCoords += new Vector3(-1, 0, 1);

                //Debug.Log(currNodeCoords);



                // Look at next point
                cybot.transform.LookAt(currNodeCoords);

                /*
                Debug.Log(currNodeCoords);
                Debug.Log(cybot.transform.position);
                Debug.Log(currentPoint.xLoc + ", " + currentPoint.yLoc);
                Debug.Log(map.Maze.GetTextRep());
                */

            }

            // Advance towards next point 
            cybot.transform.position += cybot.transform.rotation * Vector3.forward * moveSpeed * Time.deltaTime;

            // If the cybot can see the player return a state change value
            if (cybot.canSeePlayer())
            {
                return (int)StateVals.SEES_PLAYER;

            }

            return (int)StateVals.NO_CHANGE;
        }
    }
}



public class CapturePlayer : CybotBehavior
{
    // Possible return values of the Do action for this behavior. Determines state transition
    public enum StateVals
    {
        /// <summary>
        /// Stay in this state
        /// </summary>
        NO_CHANGE,
        /// <summary>
        /// Change state because the player has moved away
        /// </summary>
        PLAYER_GONE
    }

    public CapturePlayer()
    {
        MoveType = CybotMoveTypes.CAPTURING;
    }

    public override int DoAction(Cybot cybot)
    {
        // If the player is out of range return a state change


        // Get the player
        GameObject player = GameObject.Find("FirstPersonRig(Clone)");

        // Calculate distance between player and cybot
        float dist = Vector3.Distance(player.transform.position, cybot.transform.position);

        if ( dist > 1.5)
        {
            return (int) StateVals.PLAYER_GONE;
        }

        // Otherwise return no change
        return (int) StateVals.NO_CHANGE;
    }
}
