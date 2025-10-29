using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    private int[,] map;

    public int width;
    public int height;

    List<intersection> intersections;
    List<road> roads;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        map = new int[width,height];
        intersections = new List<intersection>();
        roads = new List<road>();

        int rngX = Random.Range(0, width);
        int rngY = Random.Range(0, height);

        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                if (x == rngX && y == rngY)
                {
                    Debug.Log("Intersection found");
                    intersection section = new intersection(x, y);
                    intersections.Add(section);
                }
                
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    bool InMapRange(int x,int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    class road
    {
        int startPosition;
        int endPosition;

        int rX;
        int rY;
        public road(int x,int y)
        {
            rX = x;
            rY = y;
        }
    }

    class intersection
    {
        public int posX;
        public int posY;

        public road topRoad, leftRoad, bottomRoad, rightRoad;

        public intersection(int _posX,int _posY)
        {
            posX = _posX;
            posY = _posY;

            topRoad = new road(posX, posY + 1);
            leftRoad = new road(posX - 1, posY);
            bottomRoad = new road(posX, posY - 1);
            rightRoad = new road(posX + 1, posY);

        }
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying && map != null)
        {
            if (intersections != null)
            foreach (var item in intersections)
            {
                Gizmos.color = Color.red;
                var position = new Vector3(item.posX, 0, item.posY);
                Gizmos.DrawCube(position,Vector3.one * 5);
            }
        }
    }
}
