using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum METHOD
{
    BORDER,
    RANDOM,
}

public class MapGenerator : MonoBehaviour
{
    public METHOD method = METHOD.BORDER;
    public GameObject wallPrefab;
    public int gridSize = 20;
    public float worldSize = 20;

    private GameObject[,] map;
    private GameObject mapParent;

    void Start()
    {
        InitializeMap(false);
    }

    void InitializeMap(bool isValidate)
    {
        if (isValidate)
        {
            DestroyImmediate(mapParent);
        }
        else
        {
            Destroy(mapParent);
        }
        mapParent = new GameObject("Map");
        mapParent.transform.parent = transform;
        map = new GameObject[gridSize, gridSize];
        GenerateMap();
    }

    void GenerateMap()
    {
        int border = gridSize > 5 ? 5 : gridSize >= 2 ? gridSize / 2 - 1 : 0;
        Vector3 offset = new Vector3(-0.5f * worldSize, 0.0f, -0.5f * worldSize);
        float step = worldSize / gridSize;
        for (int z = 0; z < gridSize; z++, offset.z += step)
        {
            offset.x = -0.5f * worldSize;
            for (int x = 0; x < gridSize; x++, offset.x += step)
            {
                switch (method)
                {
                    case METHOD.BORDER:
                        if (x < border || x >= gridSize - border || z < border || z >= gridSize - border)
                        {
                            var obj = Instantiate(wallPrefab, mapParent.transform.position + offset, Quaternion.identity, mapParent.transform);
                            obj.transform.localScale = new Vector3(step, 1.0f, step);
                            map[z, x] = obj;
                        }
                        break;
                    case METHOD.RANDOM:
                        if (Random.Range(0, 2) == 0)
                        {
                            var obj = Instantiate(wallPrefab, mapParent.transform.position + offset, Quaternion.identity, mapParent.transform);
                            obj.transform.localScale = new Vector3(step, 1.0f, step);
                            map[z, x] = obj;
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
