using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WandererCy : Cybot
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        // Choose a random point for this sentry to guard
        int pointX = Random.Range(0, map.MazeWidth);
        int pointY = Random.Range(0, map.MazeHeight);
        MapGraph.UGraphNode goTo = map.Maze.Maze[pointX, pointY];

        // Set behavior to travel to the chosen point
        currentBehavior = new PathfindToPoint(map, goTo);



    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        // Assess state change based on currentBehavior.DoAction return value (And current state)

        // If pathfinding to a point
        if (currentBehavior.GetType() == typeof(PathfindToPoint))
        {
            // Switch behavior to guard point if indicated
            if (doActionRet == (int)PathfindToPoint.StateVals.AT_POINT)
            {
                // Go to a new random point

                int pointX = Random.Range(0, map.MazeWidth);
                int pointY = Random.Range(0, map.MazeHeight);
                MapGraph.UGraphNode goTo = map.Maze.Maze[pointX, pointY];

                currentBehavior = new PathfindToPoint(map, goTo);
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
                // Move to a random point to guard
                int pointX = Random.Range(0, map.MazeWidth);
                int pointY = Random.Range(0, map.MazeHeight);
                MapGraph.UGraphNode gotTo = map.Maze.Maze[pointX, pointY];

                // Set behavior to travel to the chosen point
                currentBehavior = new PathfindToPoint(map, gotTo);
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
                // Otherwise go to a random point to guard
                else
                {
                    // Move to a random point to guard
                    int pointX = Random.Range(0, map.MazeWidth);
                    int pointY = Random.Range(0, map.MazeHeight);
                    MapGraph.UGraphNode goTo = map.Maze.Maze[pointX, pointY];

                    // Set behavior to travel to the chosen point
                    currentBehavior = new PathfindToPoint(map, goTo);
                }
            }
        }


    }
}
