using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BackgroundCity : MonoBehaviour
{
    public RectTransform container;

    public float nearSpeed = 35f;
    public float middleSpeed = 20f;
    public float farSpeed = 10f;

    public int buildingsPerLayer = 10;

    public Color nearBuildingColor = new Color(0.025f, 0.025f, 0.035f, 1f);
    public Color middleBuildingColor = new Color(0.045f, 0.045f, 0.06f, 1f);
    public Color farBuildingColor = new Color(0.075f, 0.075f, 0.09f, 1f);

    public Color windowOnColor = new Color(1f, 0.8f, 0.35f, 1f);
    public Color windowOffColor = new Color(0.015f, 0.015f, 0.02f, 1f);

    public Color roadColor = new Color(0.015f, 0.015f, 0.02f, 1f);

    private List<Building> nearBuildings = new List<Building>();
    private List<Building> middleBuildings = new List<Building>();
    private List<Building> farBuildings = new List<Building>();

    private List<RectTransform> cars = new List<RectTransform>();

    private float cityWidth;

    private class Building
    {
        public RectTransform root;
        public List<Image> windows = new List<Image>();
        public float speed;
        public float flickerTimer;
    }

    private void Start()
    {
        if (container == null)
            container = GetComponent<RectTransform>();

        cityWidth = Mathf.Max(container.rect.width, Screen.width);

        CreateLayer(nearBuildings, nearBuildingColor, nearSpeed, 0.2f, 0.9f);
        CreateLayer(middleBuildings, middleBuildingColor, middleSpeed, 0.5f, 0.65f);
        CreateLayer(farBuildings, farBuildingColor, farSpeed, 0.8f, 0.4f);

        CreateRoad();
        CreateCars();
    }

    private void Update()
    {
        MoveLayer(nearBuildings);
        MoveLayer(middleBuildings);
        MoveLayer(farBuildings);

        UpdateCars();
        UpdateWindows(nearBuildings);
        UpdateWindows(middleBuildings);
        UpdateWindows(farBuildings);
    }

    private void CreateLayer(
        List<Building> list,
        Color buildingColor,
        float speed,
        float verticalPosition,
        float brightness)
    {
        float spacing = cityWidth / buildingsPerLayer;

        for (int i = 0; i < buildingsPerLayer; i++)
        {
            float x = -cityWidth / 2f + spacing * i;

            float width = Random.Range(spacing * 0.65f, spacing * 1.05f);
            float height = Random.Range(
                container.rect.height * 0.18f * brightness,
                container.rect.height * 0.55f * brightness
            );

            float y = -container.rect.height / 2f + height / 2f;

            Building building = CreateBuilding(
                x,
                y,
                width,
                height,
                buildingColor,
                speed
            );

            list.Add(building);
        }
    }

    private Building CreateBuilding(
        float x,
        float y,
        float width,
        float height,
        Color buildingColor,
        float speed)
    {
        GameObject buildingObject = new GameObject(
            "Building",
            typeof(RectTransform),
            typeof(Image)
        );

        RectTransform buildingRect = buildingObject.GetComponent<RectTransform>();
        Image buildingImage = buildingObject.GetComponent<Image>();

        buildingRect.SetParent(container, false);
        buildingRect.sizeDelta = new Vector2(width, height);
        buildingRect.anchoredPosition = new Vector2(x, y);

        buildingImage.color = buildingColor;
        buildingImage.raycastTarget = false;

        Building building = new Building
        {
            root = buildingRect,
            speed = speed,
            flickerTimer = Random.Range(1f, 4f)
        };

        CreateWindows(building, width, height);

        return building;
    }

    private void CreateWindows(Building building, float width, float height)
    {
        float windowWidth = Mathf.Clamp(width * 0.07f, 5f, 10f);
        float windowHeight = Mathf.Clamp(height * 0.035f, 5f, 9f);

        float spacingX = windowWidth * 2.3f;
        float spacingY = windowHeight * 2.7f;

        int columns = Mathf.Max(1, Mathf.FloorToInt(width / spacingX));
        int rows = Mathf.Max(1, Mathf.FloorToInt(height / spacingY));

        float startX = -width / 2f + spacingX;
        float startY = -height / 2f + spacingY;

        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                if (Random.value > 0.5f)
                    continue;

                GameObject windowObject = new GameObject(
                    "Window",
                    typeof(RectTransform),
                    typeof(Image)
                );

                RectTransform windowRect = windowObject.GetComponent<RectTransform>();
                Image windowImage = windowObject.GetComponent<Image>();

                windowRect.SetParent(building.root, false);

                windowRect.sizeDelta = new Vector2(
                    windowWidth,
                    windowHeight
                );

                windowRect.anchoredPosition = new Vector2(
                    startX + column * spacingX,
                    startY + row * spacingY
                );

                windowImage.color =
                    Random.value < 0.6f
                    ? windowOnColor
                    : windowOffColor;

                windowImage.raycastTarget = false;

                building.windows.Add(windowImage);
            }
        }
    }

    private void MoveLayer(List<Building> buildings)
    {
        for (int i = 0; i < buildings.Count; i++)
        {
            Building building = buildings[i];

            building.root.anchoredPosition +=
                Vector2.left *
                building.speed *
                Time.deltaTime;

            if (building.root.anchoredPosition.x < -cityWidth / 2f - building.root.rect.width)
            {
                float rightmost = GetRightmostBuilding(buildings);

                building.root.anchoredPosition = new Vector2(
                    rightmost + Random.Range(10f, 50f),
                    building.root.anchoredPosition.y
                );
            }
        }
    }

    private float GetRightmostBuilding(List<Building> buildings)
    {
        float rightmost = -Mathf.Infinity;

        for (int i = 0; i < buildings.Count; i++)
        {
            float right =
                buildings[i].root.anchoredPosition.x +
                buildings[i].root.rect.width / 2f;

            if (right > rightmost)
                rightmost = right;
        }

        return rightmost;
    }

    private void UpdateWindows(List<Building> buildings)
    {
        for (int i = 0; i < buildings.Count; i++)
        {
            Building building = buildings[i];

            building.flickerTimer -= Time.deltaTime;

            if (building.flickerTimer > 0f)
                continue;

            building.flickerTimer = Random.Range(1f, 5f);

            if (building.windows.Count == 0)
                continue;

            Image window =
                building.windows[Random.Range(0, building.windows.Count)];

            if (window.color == windowOnColor)
                window.color = windowOffColor;
            else
                window.color = windowOnColor;
        }
    }

    private void CreateRoad()
    {
        GameObject roadObject = new GameObject(
            "Road",
            typeof(RectTransform),
            typeof(Image)
        );

        RectTransform road = roadObject.GetComponent<RectTransform>();
        Image roadImage = roadObject.GetComponent<Image>();

        road.SetParent(container, false);

        road.anchorMin = new Vector2(0f, 0f);
        road.anchorMax = new Vector2(1f, 0f);
        road.pivot = new Vector2(0.5f, 0f);

        road.sizeDelta = new Vector2(0f, container.rect.height * 0.12f);
        road.anchoredPosition = Vector2.zero;

        roadImage.color = roadColor;
        roadImage.raycastTarget = false;

        road.SetAsFirstSibling();
    }

    private void CreateCars()
    {
        for (int i = 0; i < 6; i++)
        {
            GameObject carObject = new GameObject(
                "Car",
                typeof(RectTransform),
                typeof(Image)
            );

            RectTransform car = carObject.GetComponent<RectTransform>();
            Image image = carObject.GetComponent<Image>();

            car.SetParent(container, false);

            float width = Random.Range(45f, 80f);
            float height = Random.Range(15f, 25f);

            car.sizeDelta = new Vector2(width, height);

            car.anchoredPosition = new Vector2(
                Random.Range(-cityWidth / 2f, cityWidth / 2f),
                -container.rect.height / 2f +
                container.rect.height * 0.14f
            );

            image.color = new Color(
                Random.Range(0.05f, 0.15f),
                Random.Range(0.05f, 0.15f),
                Random.Range(0.05f, 0.2f),
                1f
            );

            image.raycastTarget = false;

            cars.Add(car);
        }
    }

    private void UpdateCars()
    {
        for (int i = 0; i < cars.Count; i++)
        {
            RectTransform car = cars[i];

            car.anchoredPosition +=
                Vector2.left *
                (nearSpeed * 1.3f) *
                Time.deltaTime;

            if (car.anchoredPosition.x < -cityWidth / 2f - car.rect.width)
            {
                car.anchoredPosition = new Vector2(
                    cityWidth / 2f + Random.Range(50f, 200f),
                    car.anchoredPosition.y
                );
            }
        }
    }
}