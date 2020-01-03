using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace MapGenerator
{
    public class MapGenerator : MonoBehaviour
    {
        public bool useECS = false;

        [Header("Map")]
        public int gridSize = 200;
        public int maxRooms = 30;
        public int minRoomSize = 6;
        public int maxRoomSize = 10;

        [Header("World")]
        public float worldSize = 20;
        public GameObject wallPrefab;
        public GameObject floorPrefab;

        private Map map;

        private EntityManager manager;
        private Entity wallEntityPrefab;
        private Entity floorEntityPrefab;

        private GameObject mapParent;

        private GameObject[,] mapObjs;
        private NativeArray<Entity> walls;
        private NativeArray<Entity> floors;

        void Start()
        {
            if (useECS)
            {
                manager = World.Active.EntityManager;
                wallEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(wallPrefab, World.Active);
                floorEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(floorPrefab, World.Active);
            }
            InitializeMap();
        }

        void InitializeMap()
        {
            if (!useECS)
            {
                Destroy(mapParent);
                mapParent = new GameObject("Map");
                mapParent.transform.parent = transform;
                mapObjs = new GameObject[gridSize, gridSize];
            }
            GenerateMap();
        }

        void GenerateMap()
        {
            Vector3 offset = new Vector3(-0.5f * worldSize, 0.0f, -0.5f * worldSize);
            float step = worldSize / gridSize;
            Vector3 newScale = new Vector3(step, step, step);

            map = Map.BuildRandom(gridSize, maxRooms, minRoomSize, maxRoomSize);

            int nWalls = map.CountWalls();
            walls = new NativeArray<Entity>(nWalls, Allocator.TempJob);
            floors = new NativeArray<Entity>(gridSize * gridSize - nWalls, Allocator.TempJob);
            manager.Instantiate(wallEntityPrefab, walls);
            manager.Instantiate(floorEntityPrefab, floors);

            int iWalls = -1;
            int iFloors = -1;
            for (int z = 0; z < gridSize; z++, offset.z += step)
            {
                offset.x = -0.5f * worldSize;
                for (int x = 0; x < gridSize; x++, offset.x += step)
                {
                    GameObject prefab;
                    Entity ent;
                    switch (map.tiles[z, x])
                    {
                        case TileType.WALL:
                            iWalls++;
                            ent = walls[iWalls];
                            prefab = wallPrefab;
                            break;
                        default:
                            iFloors++;
                            ent = floors[iFloors];
                            prefab = floorPrefab;
                            break;
                    }
                    if (useECS)
                    {
                        manager.SetComponentData(ent, new Translation { Value = transform.position + offset });
                        manager.SetComponentData(ent, new Rotation { Value = prefab.transform.rotation });
                        manager.AddComponentData(ent, new NonUniformScale { Value = newScale });
                    }
                    else
                    {
                        var obj = Instantiate(prefab, mapParent.transform.position + offset, prefab.transform.rotation, mapParent.transform);
                        obj.transform.localScale = newScale;
                        mapObjs[z, x] = obj;
                    }
                }
            }
            walls.Dispose();
            floors.Dispose();
        }
    }
}
