using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum METHOD
{
    BORDER,
    RANDOM,
    ROOMS_AND_CORRIDORS,
}

public class MapGenerator : MonoBehaviour
{
    public METHOD method = METHOD.BORDER;
    public GameObject wallPrefab;
    public GameObject floorPrefab;
    public int gridSize = 200;
    public float worldSize = 20;
    public int maxRooms = 10;

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
        var bitset = new BitArray(gridSize * gridSize);

        switch (method)
        {
            case METHOD.ROOMS_AND_CORRIDORS:
                {
                    var r1 = new Rect(20, 15, 10, 15);
                    var r2 = new Rect(35, 15, 10, 15);
                    ApplyRoomToMap(r1, ref bitset);
                    ApplyRoomToMap(r2, ref bitset);
                }
                break;
            default:
                break;
        }
        for (int z = 0; z < gridSize; z++, offset.z += step)
        {
            offset.x = -0.5f * worldSize;
            for (int x = 0; x < gridSize; x++, offset.x += step)
            {
                GameObject prefab;
                switch (method)
                {
                    case METHOD.BORDER:
                        prefab = (x < border || x >= gridSize - border || z < border || z >= gridSize - border) ? wallPrefab : floorPrefab;
                        break;
                    case METHOD.RANDOM:
                        prefab = Random.Range(0, 2) == 0 ? wallPrefab : floorPrefab;
                        break;
                    case METHOD.ROOMS_AND_CORRIDORS:
                        prefab = bitset.Get(z * gridSize + x) ? floorPrefab : wallPrefab;
                        break;
                    default:
                        prefab = floorPrefab;
                        break;
                }
                var obj = Instantiate(prefab, mapParent.transform.position + offset, Quaternion.identity, mapParent.transform);
                obj.transform.localScale = new Vector3(step, 1.0f, step);
                map[z, x] = obj;
            }
        }
    }

    void ApplyRoomToMap(in Rect r, ref BitArray m)
    {
        for (int z = (int)r.yMin; z < (int)r.yMax; z++)
        {
            for (int x = (int)r.xMin; x < (int)r.xMax; x++)
            {
                m.Set(z * gridSize + x, true);
            }
        }
    }
}
