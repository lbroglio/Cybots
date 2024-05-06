using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEditor;
using UnityEngine;

public class MapBuilder : MonoBehaviour
{

    public int MazeWidth = 20;
    public int MazeHeight = 20;

    private MapGraph _maze;

    public MapGraph Maze
    {
        get { return _maze; }
    }

    void Awake()
    {
        int mapWidth = (MazeWidth * 8) + 2;
        int mapHeight = (MazeHeight * 8) + 2;

        // Create a floor Game object
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.transform.localScale = new Vector3(mapWidth, 1, mapHeight);
        floor.transform.position = Vector3.zero;
        floor.name = "Floor";
        floor.GetComponent<MeshRenderer>().material = (Resources.Load("Materials/FloorMTL") as Material);

        // Create Maze
        _maze = new MapGraph(MazeWidth, MazeHeight);

        if (!MapGraph.EvaluateMaze(_maze))
        {
            Debug.Log("Error: Bad Graph");
        }

        // Add walls
        int worldZ = 0 - (mapHeight / 2);


        int[,] mazeRep = _maze.GetNumRep();

        int exitPos = Random.Range(1, MazeHeight * 2);

        // Go through ever space in the maze
        for (int i = 0; i < (MazeWidth * 2) + 1; i++)
        {
            int worldX = mapWidth / 2;

            for (int j = 0; j < (MazeHeight * 2) + 1; j++)
            {

                // Add exit
                if(i == exitPos  && j == MazeWidth * 2)
                {
                    GameObject tmp = Resources.Load("Prefabs/EndTrigger") as GameObject;
                    tmp = Instantiate(tmp);
                    tmp.transform.localScale = new Vector3(4, 5, 4);
                    tmp.transform.position = new Vector3(worldX - 1, 3, worldZ + 1);
                    tmp.name = "Exit";

                }
                // If there is a wall here
                else if (mazeRep[i, j] == 1)
                {
                    GameObject tmp = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    tmp.transform.localScale = new Vector3(4, 5, 4);
                    tmp.transform.position = new Vector3(worldX - 1, 3, worldZ + 1);
                    // TODO ADD SETTING APPR MATERIAL
                    tmp.GetComponent<MeshRenderer>().material = (Resources.Load("Materials/WallMTL") as Material);
                    tmp.name = "MazeWall";
                }

                worldX -= 4;
                
            }
            worldZ += 4;
        }

    }

    // Update is called once per frame
    void Update()
    {

    }

    public Vector3 MazeLocToCoords(Point mazeLoc)
    {
        // Calculate maze edge locations
        int mapWidth = (MazeWidth * 8) + 2;
        int mapHeight = (MazeHeight * 8) + 2;


        int worldX = mapWidth / 2;
        int worldZ = 0 - (mapHeight / 2);

        // Shift according to the maze loc
        worldX -= (4 * mazeLoc.X);
        worldZ += (4 * mazeLoc.Y);

        return new Vector3(worldX, 0, worldZ);

    }

    /// <summary>
    /// Convert the index of a space in the maze (equivalent to a node in the backing graph) to coordiantes in the game world
    /// </summary>
    public Vector3 SpaceLocToCoords(Point spaceLoc)
    {
        // Convert the coords in the representation of the maze
        Point mazeLoc = new Point((spaceLoc.X * 2) + 1, (spaceLoc.Y * 2) + 1);


        // Calculate maze edge locations
        int mapWidth = (MazeWidth * 8) + 2;
        int mapHeight = (MazeHeight * 8) + 2;


        int worldX = mapWidth / 2;
        int worldZ = 0 - (mapHeight / 2);

        // Shift according to the maze loc
        worldX -= (4 * mazeLoc.X);
        worldZ += (4 * mazeLoc.Y);

        return new Vector3(worldX, 0, worldZ);

    }

    /// <summary>
    /// Choose a random point in the maze. Either (or both) argumennt(s) can be wildcarded by setting them to -1
    /// 
    /// 
    /// </summary>
    private Point chooseRandomMazePointBack(int row, int col)
    {
        int graphX;
        int graphY;


        if (row == -1)
        {
            graphX = Random.Range(0, Maze.MazeWidth);
        }
        else
        {
            graphX = row;
        }

        if (col == -1)
        {
            graphY = Random.Range(0, Maze.MazeHeight);
        }
        else
        {
            graphY = col;
        }

        // Convert these indexes to ones which index to text representation

        // Calulate the coordinates of this node and return
        return new Point((graphX * 2) + 1, (graphY * 2) + 1);


    }

    /// <summary>
    /// Choose a random point in the maze. Guranteed to be an open space
    /// </summary>
    /// <returns></returns>
    public Point ChooseRandomMazePoint()
    {
        return chooseRandomMazePointBack(-1, -1);
    }


    /// <summary>
    /// Choose a random point in the given row of the maze. Guranteed to be an open space
    /// </summary>
    public Point ChooseRandomMazePointInRow(int row)
    {
        return chooseRandomMazePointBack(row, -1);
    }

    /// <summary>
    /// Choose a random point in the given column of the maze. Guranteed to be an open space
    /// </summary>
    public Point ChooseRandomMazePointInCol(int col)
    {
        return chooseRandomMazePointBack(-1, col);
    }

    /// <summary>
    /// Convert a set of coordinates to the closest node in the graph to them
    /// </summary>
    internal MapGraph.UGraphNode CoordsToNode(Vector3 loc)
    {
        // Calculate map size
        int mapWidth = (MazeWidth * 8) + 2;
        int mapHeight = (MazeHeight * 8) + 2;

        // Convert the coords to be the closest index in the maze representation

        // Convert to ints (Integerr division  is needed)
        int worldX = System.Convert.ToInt32(loc.x);
        int worldZ = System.Convert.ToInt32(loc.z);

        // Shift to account for the top corner as 0 0
        worldX = (mapWidth / 2) - worldX;
        worldZ -= (0 - (mapHeight / 2));

        //Divide to get representation index
        worldX /= 4;
        worldZ /= 4;

        // Convert rep loc coords to the node represented by this space
        Point nodeLoc = new Point((worldX - 1) / 2, (worldZ - 1) / 2);

        // Return found node
        return _maze.Maze[nodeLoc.X, nodeLoc.Y];
    }

}

