using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class MapGenerator : MonoBehaviour
{
    public enum MapSettingValue
    {
        Width,
        Height,
        RoadIntervals,
        CameraZoom
    }

    private MapSettingValue currentValue;

    [Header("City Settings")]

    [Range(30, 100)]
    public int width = 50;

    [Range(30, 100)]
    public int height = 50;

    private int roadWidth = 2;

    [Range(10, 20)]
    public int roadInterval = 10; //For building plot size

    [Header("Prefabs")]
    public GameObject cube;
    private int[,] map; // 0 = road 1 = building plot

    [Header("Camera Settings")]
    [Range(.5f, 5f)]
    public float zoom = 1;
    public GameObject Camera;

    [Header("UI")]
    public TMP_InputField inputField;

    public TextMeshProUGUI widthText,heightText,intervalText,zoomText;

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
        inputField.onEndEdit.AddListener(InputField);
        GenerateCity();
        SetUIValues();
    }

    void Update()
    {
        if (Keyboard.current[Key.Space].wasPressedThisFrame)
        {
            GenerateCity();
        }
        SetCam();
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
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Making my borders roads
                if (x == roadWidth || y == roadWidth || x == (width - roadWidth) || y == (height - roadWidth))
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
    /// <summary>
    /// Destroys all the children from this object
    /// </summary>
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

    public void WidthSelected()
    {
        currentValue = MapSettingValue.Width;
    }
    public void HeightSelected()
    {
        currentValue = MapSettingValue.Height;
    }
    public void IntervalsSelected()
    {
        currentValue = MapSettingValue.RoadIntervals;
    }

    public void ZoomSelected()
    {
        currentValue = MapSettingValue.CameraZoom;
    }
    void SetUIValues()
    {
        widthText.text = $"Width: {width}";
        heightText.text = $"Height: {height}";
        intervalText.text = $"Build plot size: {roadInterval}";
        zoomText.text = $"Camera Zoom: {zoom}";
    }
    void InputField(string input)
    {
        if (float.TryParse(input, out float value))
        {
            switch (currentValue)
            {
                case MapSettingValue.Width:
                    width = (int)value;
                    break;
                case MapSettingValue.Height:
                    height = (int)value;
                    break;
                case MapSettingValue.RoadIntervals:
                    roadInterval = (int)value;
                    break;
                case MapSettingValue.CameraZoom:
                    zoom = value;
                    break;
            }
            SetUIValues();
            GenerateCity();
        }
    }
    void SetCam()
    {
        float xCenter = width / 2f;
        float yCenter = height / 2f;

        Vector3 center = new Vector3(xCenter, 0, yCenter);

        float distance = Mathf.Max(width, height) * zoom;

        Camera.transform.position = new Vector3(center.x, distance, center.z - distance);
        Camera.transform.LookAt(center);
    }
}