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
    public int maxRooms = 30;
    public int minRoomSize = 6;
    public int maxRoomSize = 10;

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
        var rooms = new ArrayList();

        switch (method)
        {
            case METHOD.ROOMS_AND_CORRIDORS:
                {
                    for (int i = 0; i < maxRooms; i++)
                    {
                        var w = Random.Range(minRoomSize, maxRoomSize);
                        var h = Random.Range(minRoomSize, maxRoomSize);
                        var x = Random.Range(0, gridSize - w);
                        var y = Random.Range(0, gridSize - h);
                        var r = new Rect(x, y, w, h);
                        var ok = true;
                        foreach (var room in rooms)
                        {
                            ok &= !r.Overlaps((Rect)room);
                        }
                        if (ok)
                        {
                            ApplyRoomToMap(r, ref bitset);
                            if (rooms.Count != 0)
                            {
                                var center = r.center;
                                var centerPrev = ((Rect)rooms[rooms.Count - 1]).center;
                                if (Random.Range(0, 2) == 0)
                                {
                                    ApplyHorizontalTunnel(ref bitset, (int)centerPrev.x, (int)center.x, (int)centerPrev.y);
                                    ApplyVerticalTunnel(ref bitset, (int)centerPrev.y, (int)center.y, (int)center.x);
                                }
                                else
                                {
                                    ApplyVerticalTunnel(ref bitset, (int)centerPrev.y, (int)center.y, (int)centerPrev.x);
                                    ApplyHorizontalTunnel(ref bitset, (int)centerPrev.x, (int)center.x, (int)center.y);
                                }
                            }
                            rooms.Add(r);
                        }
                    }
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
                var obj = Instantiate(prefab, mapParent.transform.position + offset, prefab.transform.rotation, mapParent.transform);
                obj.transform.localScale = new Vector3(step, step, step);
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

    void ApplyHorizontalTunnel(ref BitArray m, int xMin, int xMax, int z)
    {
        if (xMin > xMax)
        {
            int tmp = xMin;
            xMin = xMax;
            xMax = tmp;
        }
        for (int i = gridSize * z + xMin; i <= gridSize * z + xMax; i++)
        {
            m.Set(i, true);
        }
    }

    void ApplyVerticalTunnel(ref BitArray m, int zMin, int zMax, int x)
    {
        if (zMin > zMax)
        {
            int tmp = zMin;
            zMin = zMax;
            zMax = tmp;
        }
        for (int z = zMin; z <= zMax; z++)
        {
            m.Set(gridSize * z + x, true);
        }
    }
}
