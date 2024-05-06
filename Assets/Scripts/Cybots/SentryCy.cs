using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentryCy : Cybot
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        // Choose a random point for this sentry to guard
        int pointX = Random.Range(0, map.MazeWidth);
        int pointY = Random.Range(0, map.MazeHeight);
        MapGraph.UGraphNode toGuard = map.Maze.Maze[pointX, pointY];

        // Set behavior to travel to the chosen point
        currentBehavior = new PathfindToPoint(map,toGuard);



    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        // Assess state change based on currentBehavior.DoAction return value (And current state)

        // If pathfinding to a point
        if(currentBehavior.GetType() == typeof(PathfindToPoint))
        {
            // Switch behavior to guard point if indicated
            if(doActionRet == (int) PathfindToPoint.StateVals.AT_POINT)
            {
                currentBehavior = new GuardPoint();
            }
            // If the player is in sight chase them
            else if (doActionRet == (int)PathfindToPoint.StateVals.SEES_PLAYER)
            {
                currentBehavior = new Chase();
            }
        }
        // If guarding a point
        else if(currentBehavior.GetType() == typeof(GuardPoint))
        {
            // If the player is spotted
            if(doActionRet == (int) GuardPoint.StateVals.SEE_PLAYER)
            {
                currentBehavior = new Chase();
            }
        }
        // If chasing the player
        else if(currentBehavior.GetType() == typeof(Chase))
        {
            // If the player is lost
            if (doActionRet == (int) Chase.StateVals.LOST_PLAYER)
            {
                // Move to a random point to guard
                int pointX = Random.Range(0, map.MazeWidth);
                int pointY = Random.Range(0, map.MazeHeight);
                MapGraph.UGraphNode toGuard = map.Maze.Maze[pointX, pointY];

                // Set behavior to travel to the chosen point
                currentBehavior = new PathfindToPoint(map, toGuard);
            }
            // If at the player
            else if(doActionRet == (int) Chase.StateVals.AT_PLAYER)
            {
                currentBehavior = new CapturePlayer();
            }
        }
        // If capturring the player
        else if(currentBehavior.GetType() == typeof(CapturePlayer))
        {
            // If the player is out of range 
            if(doActionRet == (int)CapturePlayer.StateVals.PLAYER_GONE)
            {
                // If this cybot can see the player chase them
                if (canSeePlayer())
                {
                    currentBehavior = new Chase();
                }
                // Otherwise go to a random point to guard
                else
                {
                    // Move to a random point to guard
                    int pointX = Random.Range(0, map.MazeWidth);
                    int pointY = Random.Range(0, map.MazeHeight);
                    MapGraph.UGraphNode toGuard = map.Maze.Maze[pointX, pointY];

                    // Set behavior to travel to the chosen point
                    currentBehavior = new PathfindToPoint(map, toGuard);
                }
            }
        }


    }
}


class GuardPoint : CybotBehavior
{

    // Possible return values of the Do action for this behavior. Determines state transition
    public enum StateVals
    {
        /// <summary>
        /// Stay in the pathfinding state
        /// </summary>
        NO_CHANGE,
        /// <summary>
        /// Change state because player is in sight
        /// </summary>
        SEE_PLAYER
    }

    public GuardPoint()
    {
        MoveType = CybotMoveTypes.NONE;
    }

    public override int DoAction(Cybot cybot)
    {
        // Set cybot to turn
        cybot.GetComponent<Animator>().SetFloat("Rot", 1);

        //If the cybot can see the player return to switch to case
        if (cybot.canSeePlayer())
        {
            cybot.GetComponent<Animator>().SetFloat("Rot", 0);
            return (int) StateVals.SEE_PLAYER;
        }

        return (int)StateVals.NO_CHANGE;

    }
}