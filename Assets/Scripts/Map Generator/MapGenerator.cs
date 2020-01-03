using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapGenerator
{
    public class MapGenerator : MonoBehaviour
    {
        public GameObject wallPrefab;
        public GameObject floorPrefab;
        public int gridSize = 200;
        public float worldSize = 20;
        public int maxRooms = 30;
        public int minRoomSize = 6;
        public int maxRoomSize = 10;

        private Map map;
        private GameObject[,] mapObjs;
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
            mapObjs = new GameObject[gridSize, gridSize];
            GenerateMap();
        }

        void GenerateMap()
        {
            Vector3 offset = new Vector3(-0.5f * worldSize, 0.0f, -0.5f * worldSize);
            float step = worldSize / gridSize;
            Vector3 newScale = new Vector3(step, step, step);

            map = Map.BuildRandom(gridSize, maxRooms, minRoomSize, maxRoomSize);

            for (int z = 0; z < gridSize; z++, offset.z += step)
            {
                offset.x = -0.5f * worldSize;
                for (int x = 0; x < gridSize; x++, offset.x += step)
                {
                    GameObject prefab;
                    switch (map.tiles[z, x])
                    {
                        case TileType.WALL:
                            prefab = wallPrefab;
                            break;
                        default:
                            prefab = floorPrefab;
                            break;
                    }
                    var obj = Instantiate(prefab, mapParent.transform.position + offset, prefab.transform.rotation, mapParent.transform);
                    obj.transform.localScale = newScale;
                    mapObjs[z, x] = obj;
                }
            }
        }
    }
}
