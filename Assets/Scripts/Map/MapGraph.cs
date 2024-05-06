using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Class which generates and stores a maze as a graph. Generation is done with Wilson's algorithm
/// </summary>
public class MapGraph 
{


    /// <summary>
    /// Class which represents a Node in an Undirected Graph
    /// </summary>
    internal class UGraphNode
    {
        public int xLoc;
        public int yLoc;

        public List<UGraphNode> Edges;

        public UGraphNode(int xLoc, int yLoc)
        {
            Edges = new List<UGraphNode>();
            this.xLoc = xLoc;
            this.yLoc = yLoc;
        }
    }


    /// <summary>
    /// Graph representation of this map. Stores the nodes in a 2D array with each node occupying its spot on the map
    /// </summary>
    private UGraphNode[,] _maze;

    /// <summary>
    /// Number of cells wide the maze should be
    /// </summary>
    private int _mazeWidth;

    /// <summary>
    /// Number of cells high the maze should be
    /// </summary>
    private int _mazeHeight;

    // Set of unvisted nodes used for Wilson's algorithm
    private HashSet<UGraphNode> _unvisited;


    internal UGraphNode[,] Maze {
        get { return _maze; }
    }

    public int MazeWidth
    {
        get { return _mazeWidth; }
    }

    public int MazeHeight
    {
        get { return _mazeHeight; }
    }

    public MapGraph(int mazeWidth, int mazeHeight)
    {
        _mazeHeight = mazeHeight;
        _mazeWidth = mazeWidth;

        _maze = new UGraphNode[mazeWidth, mazeHeight];
        _unvisited = new HashSet<UGraphNode>();

        // Initialize maze
        for(int i =0; i < _mazeWidth; i++)
        {
            for(int j =0; j < _mazeHeight; j++) {
                UGraphNode tmp = new UGraphNode(i, j);
                _maze[i, j] = tmp;
                _unvisited.Add(tmp);
            }
        }

        // Create the maze
        generateMaze();
    }

    private void generateMaze()
    {

        // Choose random source
        int srcX = Random.Range(0, _mazeWidth);
        int srcY = Random.Range(0, _mazeHeight);

        _unvisited.Remove(_maze[srcX, srcY]);

        // While all cells are not filled
        while(_unvisited.Count > 0)
        {
            // Perform a random walk
             
            // Set to track nodes in this path
            List<UGraphNode> inWalk = new List<UGraphNode>();

            // Select random unvisted node
            int nodeIndex = Random.Range(0, _unvisited.Count);

            UGraphNode randStart = _unvisited.ElementAt(nodeIndex);

            inWalk.Add(randStart);


            while(inWalk.Count > 0)
            {
                UGraphNode lastNode = inWalk.ElementAt(inWalk.Count - 1);

                int xMoveMin = -1;
                int xMoveMax = 2;

                int yMoveMin = -1;
                int yMoveMax = 1;

                if (lastNode.xLoc == _mazeWidth - 1)
                {
                    xMoveMax -= 1;
                }
                else if(lastNode.xLoc == 0)
                {
                    xMoveMin += 1;
                }

                if (lastNode.yLoc == _mazeHeight - 1)
                {
                    yMoveMax = -1;
                }
                else if (lastNode.yLoc == 0)
                {
                    yMoveMin = 1;
                }


                int xMove = Random.Range(xMoveMin, xMoveMax);
                int yMove;
                if(xMove == 0)
                {
                    int[] choiceVals = { yMoveMin, yMoveMax };

                    yMove = choiceVals[Random.Range(0, 2)];
                }
                else
                {
                    yMove = 0;
                }

                UGraphNode chosenNode = _maze[lastNode.xLoc + xMove, lastNode.yLoc + yMove];

                if (inWalk.Contains(chosenNode)){
                    int indexOf =  inWalk.IndexOf(chosenNode);
                    inWalk.RemoveRange(indexOf + 1, inWalk.Count -  (indexOf + 1));
                }
                else if(_unvisited.Contains(chosenNode))
                {
                    inWalk.Add(chosenNode);
                }
                else
                {
                    for(int i =0; i < inWalk.Count; i++)
                    {
                        UGraphNode atI = inWalk[i];
                        if (i != inWalk.Count - 1)
                        {
                            atI.Edges.Add(inWalk[i + 1]);
                            inWalk[i + 1].Edges.Add(atI);

                        }
                        else
                        {
                            atI.Edges.Add(chosenNode);
                            chosenNode.Edges.Add(atI);
                        }

                        _unvisited.Remove(atI);
                    }
                    inWalk.Clear();
                }
            }
        }
    }

    /// <summary>
    /// Returns a text based representation of the current maze
    /// </summary>
    /// <returns></returns>
    public string GetTextRep()
    {
        int repHeight = (_mazeHeight * 2);
        int repWidth = (_mazeWidth * 2);

        char[,] textRep = new char[repWidth, repHeight];

        for(int i =0; i < repWidth; i++)
        {
            for(int j =0; j < repHeight; j++)
            {
                if(i % 2 == 0 && j % 2 == 0)
                {
                    textRep[i, j] = ' ';
                }
                else
                {
                    textRep[i, j] = '#';
                }
            }
        }

        for(int i = 0; i < _mazeWidth; i++)
        {
            for(int j=0; j < _mazeWidth; j++)
            {
                UGraphNode currNode = _maze[i , j];
                
                foreach(UGraphNode e in currNode.Edges){
                    int xOffset = e.xLoc - currNode.xLoc;
                    int yOffset = e.yLoc - currNode.yLoc;

                    textRep[(currNode.yLoc * 2) + yOffset, (currNode.xLoc * 2) + xOffset] = ' ';
                }
            }
        }

        string rep = "";
        for(int i =0; i <  repWidth + 1; i++)
        {
            for(int j=0; j < repHeight + 1; j++)
            {
                if(i == 0 || j == 0)
                {
                    rep += "#";
                }
                else
                {
                    rep += textRep[(i - 1), (j - 1)];
                }
            }
            rep += "\n";
        }

        return rep;
    }


    /// <summary>
    /// Returns a representation of the maze using ones and zeroes
    /// 1 = wall
    /// 0 = open space
    /// </summary>
    /// <returns></returns>
    public int[,] GetNumRep()
    {
        int repHeight = (_mazeHeight * 2);
        int repWidth = (_mazeWidth * 2);

        int[,] numRep = new int[repWidth + 1, repHeight + 1];

        for (int i = 0; i < repWidth + 1; i++)
        {
            for (int j = 0; j < repHeight + 1; j++)
            {
                if ((i + 1) % 2 == 0 && (j + 1) % 2 == 0)
                {
                    numRep[i, j] = 0;
                }
                else
                {
                    numRep[i, j] = 1;
                }
            }
        }

        for (int i = 0; i < _mazeWidth; i++)
        {
            for (int j = 0; j < _mazeWidth; j++)
            {
                UGraphNode currNode = _maze[i, j];

                foreach (UGraphNode e in currNode.Edges)
                {
                    int xOffset = e.xLoc - currNode.xLoc;
                    int yOffset = e.yLoc - currNode.yLoc;

                    numRep[(currNode.yLoc * 2) + yOffset + 1, (currNode.xLoc * 2) + xOffset + 1] = 0;
                }
            }
        }

        return numRep;
    }

    /// <summary>
    /// Perform BFS on a maze to determine if its valid
    /// </summary>
    /// <param name="m">The maze to evaluate</param>
    public static bool EvaluateMaze(MapGraph m)
    {

        // Perform BFS
        List<UGraphNode> visited = new List<UGraphNode>();
        Queue<UGraphNode> q = new Queue<UGraphNode>();

        UGraphNode root = m.Maze[0, 0];

        visited.Add(root);

        q.Enqueue(root);

        while (q.Count > 0)
        {
            UGraphNode node = q.Dequeue();

            foreach(UGraphNode e in node.Edges)
            {
                if (!visited.Contains(e))
                {
                    visited.Add(e);
                    q.Enqueue(e);
                }
            }
        }

        // Verify all nodes have been visited
        for(int i = 0; i < m._mazeWidth; i++)
        {
            for(int j=0; j < m._mazeHeight; j++)
            {
                if (!visited.Contains(m.Maze[i, j]))
                {
                    Debug.Log("Missing Node: (" + m.Maze[i, j].xLoc + ", " + m.Maze[i, j].yLoc + ")");
                    return false;
                }
            }
        }

        return true;

    }

}
