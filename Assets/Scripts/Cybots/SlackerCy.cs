using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SlackerCy : Cybot
{

    private Player player;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        player = GameObject.Find("FirstPersonRig(Clone)").GetComponent<Player>();

        currentBehavior = new AwaitChase();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        // Waiting for a chase (or capture)
        if (currentBehavior.GetType() == typeof(AwaitChase))
        {
            // If a chase has started move to player
            if (doActionRet == (int)AwaitChase.StateVals.CHASE_STARTED)
            {
                currentBehavior = new GoToPlayer(player, map);
            }
        }
        // If moving to the player
        else if (currentBehavior.GetType() == typeof(GoToPlayer))
        {
            GameController gameCon = GameObject.Find("GameController").GetComponent<GameController>();
            // If the player is seen start chasing
            if (doActionRet == (int)GoToPlayer.StateVals.SEE_PLAYER)
            {
                currentBehavior = new Chase();
            }
            // Otherwise if the chase has stopped wait for a new one
            else if (!gameCon.chaseOccuring())
            {
                currentBehavior = new AwaitChase();
            }
        }
        // If chasing the player
        else if (currentBehavior.GetType() == typeof(Chase))
        {
            // If the player is lost
            if (doActionRet == (int)Chase.StateVals.LOST_PLAYER)
            {
                // Wait for a new chase
                currentBehavior = new AwaitChase();
            }
            // If at the player
            else if (doActionRet == (int)Chase.StateVals.AT_PLAYER)
            {
                currentBehavior = new CapturePlayer();
            }
        }
        // If capturing the player
        else if (currentBehavior.GetType() == typeof(CapturePlayer))
        {
            // If the player is out of range 
            if (doActionRet == (int)CapturePlayer.StateVals.PLAYER_GONE)
            {
                // If this cybot can see the player chase 
                if (canSeePlayer())
                {
                    currentBehavior = new Chase();
                }
                // Otherwise wait for a chase to start 
                else
                {
                    currentBehavior = new AwaitChase();
                }
            }
        }



     }
}




/// <summary>
/// Behavior for a cybot which is trying to go to the player
/// </summary>
public class GoToPlayer : CybotBehavior
{

    // Possible return values of the Do action for this behavior. Determines state transition
    public enum StateVals
    {
        /// <summary>
        /// Stay in this state
        /// </summary>
        NO_CHANGE,
        /// <summary>
        /// Change state because the player is seen
        /// </summary>
        SEE_PLAYER,
    }


    // Set the map this behavior pathfinding on
    MapBuilder map;

    private MapGraph.UGraphNode targetPoint;
    // The current point being moved to 
    private MapGraph.UGraphNode currentPoint;
    // Track the path to take to the target
    private Stack<MapGraph.UGraphNode> path;
    //The player to follow
    private Player toFollow;



    public GoToPlayer(Player player, MapBuilder map)
    {
        MoveType = CybotMoveTypes.RUN;
        toFollow = player;

        // Calculate target point
        MapGraph.UGraphNode playerNode = map.CoordsToNode(player.transform.position);

        this.targetPoint = playerNode;
        this.currentPoint = null;
        path = null;
        this.map = map;
    }

    public override int DoAction(Cybot cybot)
    {
        float moveSpeed = cybot.WalkSpeed;
        // Set how fast to move between points (Used by subclasses who run)
        if (MoveType == CybotMoveTypes.RUN)
        {
            moveSpeed = cybot.RunSpeed;
        }


        // If not in position move towards the position. Behavior knows its in position once the path stack is initialized
        if (path == null)
        {
            // Set current point if needed
            if (currentPoint == null)
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


            return (int)StateVals.NO_CHANGE;
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
                // Calculate target point
                MapGraph.UGraphNode playerNode = map.CoordsToNode(toFollow.transform.position);

                MapGraph.UGraphNode targetNode;
                targetNode = playerNode;

                // Recalculate path to mactch current player location
                path = Pathfinding.shortestPathToNode(currentPoint, targetNode);
                path.Pop();

                // If the path isn't empty
                if (path.Count > 0)
                {
                    // switch to next point
                    currentPoint = path.Pop();
                }



                // Get coords for new point
                currNodeCoords = map.SpaceLocToCoords(new System.Drawing.Point(currentPoint.xLoc, currentPoint.yLoc));
                // Shift coords to target center of square
                currNodeCoords += new Vector3(-1, 0, 1);

                // Look at next point
                cybot.transform.LookAt(currNodeCoords);

            }

            // Advance towards next point 
            cybot.transform.position += cybot.transform.rotation * Vector3.forward * moveSpeed * Time.deltaTime;


            // If this cybot can see the player return corresponding flag

            if (cybot.canSeePlayer())
            {
                return (int)StateVals.SEE_PLAYER;
            }

            // Otherwise return no change
            return (int)StateVals.NO_CHANGE;
        }
    }
}


/// <summary>
/// Behavior for a Cybot waiting for a chase to start
/// </summary>
public class AwaitChase : CybotBehavior
{
    // Possible return values of the Do action for this behavior. Determines state transition
    public enum StateVals
    {
        /// <summary>
        /// Stay in this state
        /// </summary>
        NO_CHANGE,
        /// <summary>
        /// Change state because the player is being chased
        /// </summary>
        CHASE_STARTED
    }

    public AwaitChase()
    {
        MoveType = CybotMoveTypes.IDLE;
    }

    public override int DoAction(Cybot cybot)
    {
        GameController gameCon = GameObject.Find("GameController").GetComponent<GameController>();

        // If a chase is occuring return flag to start towards players
        if (gameCon.chaseOccuring())
        {
            return (int) StateVals.CHASE_STARTED;
        }

        return (int) StateVals.NO_CHANGE;
    }
}
    
