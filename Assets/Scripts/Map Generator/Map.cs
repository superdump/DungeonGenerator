using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapGenerator
{
    public enum TileType
    {
        WALL,
        FLOOR,
        GROUND,
        DOOR,
    }

    public class Map
    {
        public TileType[,] tiles;
        public List<Rect> rooms;

        public Map(int gridSize)
        {
            tiles = new TileType[gridSize, gridSize];
        }

        public static Map BuildRandom(int gridSize, int maxRooms, int minRoomSize, int maxRoomSize)
        {
            return new SimpleMapBuilder().build(gridSize, maxRooms, minRoomSize, maxRoomSize);
        }
    }
}