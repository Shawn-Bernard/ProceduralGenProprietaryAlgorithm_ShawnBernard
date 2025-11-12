using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor;

public class MapGenerator : MonoBehaviour
{
    [Header("City Settings")]
    [Range(30,100)]
    public int width = 50;
    [Range(30, 100)]
    public int height = 50;
    private int roadWidth = 2;
    [Range(10, 20)]
    public int roadInterval = 10;
    [Header("Prefabs")]
    public GameObject cube;
    private int[,] map; // 0 = road 1 = building plot

    private int road 
    {  
        get => 0;
    }
    private int building
    {
        get => 1;
    }

    void Start()
    {
        GenerateCity();
    }

    void Update()
    {
        if (Keyboard.current[Key.Space].wasPressedThisFrame)
        {
            GenerateCity();
        }
    }

    public void GenerateCity()
    {
        DestroyCity();
        map = new int[width, height];

        FillMap();

        CreateGrid();

        AddCityObjects();
        
    }

    void AddCityObjects()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (map[x, y] == road)
                {
                    CreateRoad(x, y);
                }
                else if (map[x, y] == building)
                {
                    CreateBuilding(x, y);
                }
            }
        }
    }
    /// <summary>
    /// Places the border road and fills the rest with buildings
    /// </summary>
    void FillMap()
    {
        int halfRoadWidth = roadWidth / 2;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Making my borders roads
                if (x == halfRoadWidth || y == halfRoadWidth || x == (width - halfRoadWidth) || y == (height - halfRoadWidth))
                {
                    MarkRoad(x, y);
                }
                else
                {
                    map[x, y] = building;
                }
            }
        }
    }
    /// <summary>
    /// Marks roads at each road interval for horizontal and vertical
    /// </summary>
    void CreateGrid()
    {
        for (int x = 0; x < width; x += roadInterval)
        {
            for (int y = 0; y < height; y++)
            {
                MarkRoad(x, y);
            }
        }
        for (int y = 0; y < height; y += roadInterval)
        {
            for (int x = 0; x < width; x++)
            {
                MarkRoad(x, y);
            }
        }
    }
    /// <summary>
    /// Takes in the position and marks nearby position as road to be wider
    /// </summary>
    /// <param name="roadX"></param>
    /// <param name="roadY"></param>
    void MarkRoad(int roadX, int roadY)
    {
        int half = Mathf.CeilToInt(roadWidth / 2);

        for (int x = roadX - half; x <= roadX + half; x++)
        {
            for (int y = roadY - half; y <= roadY + half; y++)
            {
                if (InMapRange(x, y))
                {
                    map[x, y] = road;
                }
            }
        }
    }
    /// <summary>
    /// Crates a game object that picks a random height and chooses random color for building
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    void CreateBuilding(int x, int y)
    {
        Vector3 currentPosition = new Vector3(x, 0, y);
        Vector3 centerPosition = new Vector3(width / 2f, 0, height / 2f);
        float distance = Vector3.Distance(currentPosition, centerPosition);
        float maxDistance = Vector3.Distance(Vector3.zero, centerPosition);
        int buildHeight;

        float heightChance = 1f - (distance / maxDistance);

        if (heightChance > 0.7f)
        {
            buildHeight = Random.Range(9, 13);
        }
        else if (heightChance > 0.4f)
        {
            buildHeight = Random.Range(4, 9);
        }
        else
        {
            buildHeight = Random.Range(2, 5);
        }

        GameObject building = Instantiate(cube, transform);
        building.transform.localPosition = new Vector3(x, buildHeight/2, y);
        building.transform.localScale = new Vector3(1, buildHeight, 1);
        building.name = "Building";

        Renderer renderer = building.GetComponent<Renderer>();
        renderer.material.color = Random.ColorHSV();
    }

    void CreateRoad(int x, int y)
    {
        GameObject road = Instantiate(cube, transform);
        road.transform.localPosition = new Vector3(x, 0, y);
        road.transform.localScale = Vector3.one;
        road.name = "Road";
    }

    void DestroyCity()
    {
        if (transform.childCount > 0)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }
    }

    bool InMapRange(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }
}