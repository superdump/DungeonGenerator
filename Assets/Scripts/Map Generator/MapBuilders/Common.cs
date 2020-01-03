using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapGenerator
{
    interface IMapBuilder
    {
        Map build(int gridSize, int maxRooms, int minRoomSize, int maxRoomSize);
    }

    public class Common
    {
        public static void ApplyRoomToMap(in Rect r, ref TileType[,] m)
        {
            for (int z = (int)r.yMin; z < (int)r.yMax; z++)
            {
                for (int x = (int)r.xMin; x < (int)r.xMax; x++)
                {
                    m[z, x] = TileType.FLOOR;
                }
            }
        }

        public static void ApplyHorizontalTunnel(ref TileType[,] m, int xMin, int xMax, int z)
        {
            if (xMin > xMax)
            {
                int tmp = xMin;
                xMin = xMax;
                xMax = tmp;
            }
            for (int x = xMin; x <= xMax; x++)
            {
                m[z, x] = TileType.FLOOR;
            }
        }

        public static void ApplyVerticalTunnel(ref TileType[,] m, int zMin, int zMax, int x)
        {
            if (zMin > zMax)
            {
                int tmp = zMin;
                zMin = zMax;
                zMax = tmp;
            }
            for (int z = zMin; z <= zMax; z++)
            {
                m[z, x] = TileType.FLOOR;
            }
        }
    }
}
