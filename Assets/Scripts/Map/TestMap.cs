using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMap : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
     
        MapGraph m = new MapGraph(20,  20);

        Debug.Log(m.GetTextRep());

        if (MapGraph.EvaluateMaze(m))
        {
            Debug.Log("Bad Maze");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
