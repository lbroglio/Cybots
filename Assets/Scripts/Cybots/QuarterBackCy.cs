using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;



/// <summary>
/// Cybot which moves between the 4 corners of the map and always run
/// </summary>
public class QuarterBackCy : Cybot
{
    // The points this cybot will move between
    private MapGraph.UGraphNode[] patrolPoints;

    // Index of current point being moved to
    private int patrolIndex;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        // Set the patrol points to be the four corners of the map
        MapGraph.UGraphNode p1 = map.Maze.Maze[0, 0];
        MapGraph.UGraphNode p2 = map.Maze.Maze[0, map.MazeHeight - 1];
        MapGraph.UGraphNode p3 = map.Maze.Maze[map.MazeWidth - 1, 0];
        MapGraph.UGraphNode p4 = map.Maze.Maze[map.MazeWidth - 1, map.MazeHeight - 1];

        patrolPoints = new MapGraph.UGraphNode[]{ p1, p2, p3, p4};


        // Set behavior to run to first patrol point
        patrolIndex = 0;
        currentBehavior = new RunToPoint(map, patrolPoints[patrolIndex]);



    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        // Assess state change based on currentBehavior.DoAction return value (And current state)

        // If going to a point
        if (currentBehavior.GetType() == typeof(RunToPoint))
        {
            // Go to next point if indicated
            if (doActionRet == (int)PathfindToPoint.StateVals.AT_POINT)
            {
                patrolIndex = (patrolIndex + 1) % 4;
                currentBehavior = new RunToPoint(map, patrolPoints[patrolIndex]);
            }
            // If the player is in sight chase them
            else if (doActionRet == (int)PathfindToPoint.StateVals.SEES_PLAYER)
            {
                currentBehavior = new Chase();
            }
        }
        // If chasing the player
        else if (currentBehavior.GetType() == typeof(Chase))
        {
            // If the player is lost
            if (doActionRet == (int)Chase.StateVals.LOST_PLAYER)
            {
                // Continue moving to next point
                currentBehavior = new RunToPoint(map, patrolPoints[patrolIndex]);
            }
            // If at the player
            else if (doActionRet == (int)Chase.StateVals.AT_PLAYER)
            {
                currentBehavior = new CapturePlayer();
            }
        }
        // If capturring the player
        else if (currentBehavior.GetType() == typeof(CapturePlayer))
        {
            // If the player is out of range 
            if (doActionRet == (int)CapturePlayer.StateVals.PLAYER_GONE)
            {
                // If this cybot can see the player chase them
                if (canSeePlayer())
                {
                    currentBehavior = new Chase();
                }
                // Otherwise go to a random point to guide
                else
                {
                    // Continue moving to next point
                    currentBehavior = new RunToPoint(map, patrolPoints[patrolIndex]);
                }
            }
        }


    }
}


public class RunToPoint : PathfindToPoint
{

    internal RunToPoint(MapBuilder map, MapGraph.UGraphNode targetPoint) : base(map, targetPoint)
    {
        MoveType = CybotMoveTypes.RUN;
    }

    public override int DoAction(Cybot cybot)
    {
        return base.DoAction(cybot);
    }
}

