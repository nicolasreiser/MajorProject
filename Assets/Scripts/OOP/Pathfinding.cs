using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;

public class Pathfinding : MonoBehaviour
{

    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;
    private struct PathNode
    {
        public int X;
        public int Y;

        public int Index;

        public int GCost;
        public int HCost;
        public int FCost;

        public bool IsWalkable;

        public int PreviousNodeIndex;

        public void CalculateFcost()
        {
            FCost = GCost + HCost; 
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        FindPath(new int2(0, 0), new int2(3, 1));
    }

    // Update is called once per frame
    void Update()
    {

    }

    

    private void FindPath(int2 startPosition, int2 endPosition)
    {
        int2 gridSize = new int2(4, 4);

        NativeArray<PathNode> pathNodeArray = new NativeArray<PathNode>(gridSize.x * gridSize.y, Allocator.Temp);


        for(int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                PathNode pathNode = new PathNode();
                pathNode.X = x;
                pathNode.Y = y;
                pathNode.Index = CalculateIndex(x, y, gridSize.x);

                pathNode.GCost = int.MaxValue;
                pathNode.HCost = CalculateDistanceCost(new int2(x, y), endPosition);
                pathNode.CalculateFcost();

                pathNode.IsWalkable = true;
                pathNode.PreviousNodeIndex = -1;

                pathNodeArray[pathNode.Index] = pathNode;
            }
        }

        NativeArray<int2> neighbourOffsetArray = new NativeArray<int2>(new int2[]{
            new int2(-1,0),
            new int2(+1,0),
            new int2(0,+1),
            new int2(0,-1),
            new int2(-1,-1),
            new int2(-1,+1),
            new int2(+1,-1),
            new int2(+1,+1),
        }, Allocator.Temp);
        int endNodeIndex = CalculateIndex(endPosition.x, endPosition.y, gridSize.x);

        PathNode startNode = pathNodeArray[CalculateIndex(startPosition.x, startPosition.y, gridSize.x)];
        startNode.GCost = 0;
        startNode.CalculateFcost();
        pathNodeArray[startNode.Index] = startNode;

        NativeList<int> openList = new NativeList<int>(Allocator.Temp);
        NativeList<int> closedList = new NativeList<int>(Allocator.Temp);

        openList.Add(startNode.Index);

        while( openList.Length > 0)
        {
            int currentNodeIndex = GetLowestCostFNodeIndex(openList,pathNodeArray);
            PathNode currentNode = pathNodeArray[currentNodeIndex];

            if(currentNodeIndex == endNodeIndex)
            {
                // reached destination
                break;
            }

            //remove current node from open list
            for(int i = 0; i < openList.Length; i++)
            {
                if(openList[i] == currentNodeIndex)
                {
                    openList.RemoveAtSwapBack(i);
                    break;
                }
            }

            closedList.Add(currentNodeIndex);

            for(int i = 0; i < neighbourOffsetArray.Length; i++)
            {
                int2 neighbourOffset = neighbourOffsetArray[i];
                int2 neighbourPosition = new int2(currentNode.X + neighbourOffset.x, currentNode.Y + neighbourOffset.y);

                if(!IsPositionInsideGrid(neighbourPosition,gridSize))
                {
                    // not in grid
                    continue;
                }

                int neighbourNodeIndex = CalculateIndex(neighbourPosition.x, neighbourPosition.y, gridSize.x);

                if(closedList.Contains(neighbourNodeIndex))
                {
                    //node already searched
                    continue;
                }

                PathNode neighbourNode = pathNodeArray[neighbourNodeIndex];
                if(!neighbourNode.IsWalkable)
                {
                    //not walkable
                    continue;
                }

                int2 currentNodePosition = new int2(currentNode.X, currentNode.Y);

                int tentativeGCost = currentNode.GCost + CalculateDistanceCost(currentNodePosition, neighbourPosition);
                if(tentativeGCost < neighbourNode.GCost)
                {
                    neighbourNode.PreviousNodeIndex = currentNodeIndex;
                    neighbourNode.GCost = tentativeGCost;
                    neighbourNode.CalculateFcost();
                    pathNodeArray[neighbourNodeIndex] = neighbourNode;

                    if(!openList.Contains(neighbourNode.Index))
                    {
                        openList.Add(neighbourNode.Index);
                    }
                }
            }
        }

        PathNode endNode = pathNodeArray[endNodeIndex];
        if(endNode.PreviousNodeIndex == -1)
        {
            // did not find a path
            Debug.Log("No Path Found");
        }
        else
        {
            //FOUND THE PATH
            NativeList<int2> path = CalculatePath(pathNodeArray, endNode);

            foreach(int2 pathPosition in path)
            {
                Debug.Log(pathPosition);
            }
            path.Dispose();
        }

        pathNodeArray.Dispose();
        neighbourOffsetArray.Dispose();
        openList.Dispose();
        closedList.Dispose();
    }

    private NativeList<int2> CalculatePath(NativeArray<PathNode> pathNodeArray, PathNode endNode)
    {
        if(endNode.PreviousNodeIndex == -1)
        {
            // no path
            return new NativeList<int2>(Allocator.Temp);
        }
        else
        {
            // path

            NativeList<int2> path = new NativeList<int2>(Allocator.Temp);
            path.Add(new int2(endNode.X, endNode.Y));

            PathNode currentNode = endNode;

            while(currentNode.PreviousNodeIndex != -1)
            {
                PathNode previousNode = pathNodeArray[currentNode.PreviousNodeIndex];
                path.Add(new int2(previousNode.X, previousNode.Y));
                currentNode = previousNode;
            }

            return path;
        }
    }

    private bool IsPositionInsideGrid(int2 gridPosition, int2 gridSize)
    {
        return gridPosition.x >= 0 &&
            gridPosition.y >= 0 &&
            gridPosition.x < gridSize.x &&
            gridPosition.y < gridSize.y;
    }

    private int CalculateIndex(int x, int y, int gridwidth)
    {
        return x + y * gridwidth;
    }

    private int CalculateDistanceCost(int2 aPosition, int2 bPosition)
    {
        int xDistance = math.abs(aPosition.x - bPosition.x);
        int yDistance = math.abs(aPosition.y - bPosition.y);
        int remaining = math.abs(xDistance - yDistance);
        return MOVE_DIAGONAL_COST * math.min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
    }

    private int GetLowestCostFNodeIndex(NativeList<int> openList, NativeArray<PathNode> pathNodeArray)
    {
        PathNode lowestCostPathNode = pathNodeArray[openList[0]];
        for (int i = 1; i < openList.Length; i++)
        {
            PathNode testPathNode = pathNodeArray[openList[i]];
            if(testPathNode.FCost < lowestCostPathNode.FCost)
            {
                lowestCostPathNode = testPathNode;
            }
        }
        return lowestCostPathNode.Index;
    }
    
}
