using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapGenerator
{
    public class SimpleMapBuilder : IMapBuilder
    {
        public Map build(int gridSize, int maxRooms, int minRoomSize, int maxRoomSize)
        {
            Map map = new Map(gridSize);
            RoomsAndCorridors(ref map, gridSize, maxRooms, minRoomSize, maxRoomSize);
            return map;
        }

        private void RoomsAndCorridors(ref Map map, int gridSize, int maxRooms, int minRoomSize, int maxRoomSize)
        {
            var rooms = new List<Rect>();

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
                    ok &= !r.Overlaps(room);
                }
                if (ok)
                {
                    Common.ApplyRoomToMap(r, ref map.tiles);
                    if (rooms.Count != 0)
                    {
                        var center = r.center;
                        var centerPrev = rooms[rooms.Count - 1].center;
                        if (Random.Range(0, 2) == 0)
                        {
                            Common.ApplyHorizontalTunnel(ref map.tiles, (int)centerPrev.x, (int)center.x, (int)centerPrev.y);
                            Common.ApplyVerticalTunnel(ref map.tiles, (int)centerPrev.y, (int)center.y, (int)center.x);
                        }
                        else
                        {
                            Common.ApplyVerticalTunnel(ref map.tiles, (int)centerPrev.y, (int)center.y, (int)centerPrev.x);
                            Common.ApplyHorizontalTunnel(ref map.tiles, (int)centerPrev.x, (int)center.x, (int)center.y);
                        }
                    }
                    rooms.Add(r);
                }
            }

            map.rooms = rooms;
        }
    }
}