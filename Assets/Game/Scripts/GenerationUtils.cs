using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SoleHeir.GenerationUtils
{
    public class PrototypeHouse
    {
        public int seed;

        public int houseSize;

        public GenerationGrid<RoomType> statusGrid;

        public List<PrototypeRoom> rooms;

        public PrototypeHouse() {}

        public PrototypeHouse(int seed, int houseSize)
        {
            this.houseSize = houseSize;
            this.seed = seed;
            statusGrid = new GenerationGrid<RoomType>();
            rooms = new List<PrototypeRoom>();
            UnityEngine.Random.InitState (seed);

            statusGrid.Set(0, 0, RoomType.NO_ROOM);

            while (statusGrid.CountValue(RoomType.NO_ROOM) < houseSize)
            {
                List<Vector2Int> edges = new List<Vector2Int>();

                // Collect all of the edges
                for (
                    int x = statusGrid.GetMinX();
                    x <= statusGrid.GetMaxX();
                    x++
                )
                {
                    for (
                        int y = statusGrid.GetMinY();
                        y <= statusGrid.GetMaxY();
                        y++
                    )
                    {
                        if (statusGrid.Get(x, y).Equals(RoomType.NO_ROOM))
                        {
                            bool isEdge = false;
                            for (int xMod = -1; xMod <= 1; xMod++)
                            {
                                for (int yMod = -1; yMod <= 1; yMod++)
                                {
                                    if (
                                        statusGrid.Get(x + xMod, y + yMod) !=
                                        RoomType.NO_ROOM
                                    )
                                    {
                                        isEdge = true;
                                    }
                                }
                            }
                            if (isEdge)
                            {
                                edges.Add(new Vector2Int(x, y));
                            }
                        }
                    }
                }

                //Pick a random edge and determine size.
                Vector2Int edge = edges[UnityEngine.Random.Range(0, edges.Count)];
                int minRange = 1;
                int maxRange = 2 + houseSize / 100;
                int rectLeft = UnityEngine.Random.Range(minRange, maxRange);
                int rectRight = UnityEngine.Random.Range(minRange, maxRange);
                int rectUp = UnityEngine.Random.Range(minRange, maxRange);
                int rectDown = UnityEngine.Random.Range(minRange, maxRange);

                for (int x = edge.x - rectLeft; x <= edge.x + rectRight; x++)
                {
                    for (int y = edge.y - rectUp; y <= edge.y + rectDown; y++)
                    {
                        statusGrid.Set(x, y, RoomType.NO_ROOM);
                    }
                }
            }

            PrototypeRoom firstRoom = MakeRoom(new Vector2Int(0, 0));
            AddRoomToHouse (firstRoom);
            RecursivelyAddRooms (firstRoom);
        }

        public PrototypeRoom GetRoomByVector(Vector2Int position)
        {
            foreach (PrototypeRoom room in rooms)
            {
                if (
                    position.x >= room.GetPosition().x &&
                    position.x < room.GetPosition().x + room.GetSize().x &&
                    position.y >= room.GetPosition().y &&
                    position.y < room.GetPosition().y + room.GetSize().y
                )
                {
                    return room;
                }
            }
            return null;
        }

        public List<PrototypeRoom> GetRooms()
        {
            return rooms;
        }

        public void AddRoomToHouse(PrototypeRoom room)
        {
            /*Debug
                .Log(String
                    .Format("Adding Room: X: {0}, Y:{1}, XPOS: {2}, YPOS: {3}",
                    room.GetSize().x,
                    room.GetSize().y,
                    room.GetPosition().x,
                    room.GetPosition().y));*/
            for (
                int x = room.GetPosition().x;
                x < room.GetPosition().x + room.GetSize().x;
                x++
            )
            {
                for (
                    int y = room.GetPosition().y;
                    y < room.GetPosition().y + room.GetSize().y;
                    y++
                )
                {
                    statusGrid.Set(x, y, room.GetRoomType());
                }
            }

            rooms.Add (room);
        }

        public void MaybeConnectDoorways(Vector2Int pos1, Vector2Int pos2)
        {
            if ( GetRoomByVector(pos1).GetRoomType() == RoomType.SMALL || GetRoomByVector(pos2).GetRoomType() == RoomType.SMALL)
            {
                return;
            }
            else if ( GetRoomByVector(pos1).GetRoomType() == RoomType.HALLWAY || GetRoomByVector(pos2).GetRoomType() == RoomType.HALLWAY)
            {
                if (UnityEngine.Random.Range(0, 10) > 3)
                {
                    ConnectDoorways (pos1, pos2);
                }
            }
            else
            {
                if (UnityEngine.Random.Range(0, 10) > 7)
                {
                    ConnectDoorways (pos1, pos2);
                }
            }
        }

        public void ConnectDoorways(Vector2Int pos1, Vector2Int pos2)
        {
            Vector2Int minPos = new Vector2Int(Math.Min(pos1.x,pos2.x), Math.Min(pos1.y,pos2.y));
            Vector2Int maxPos = new Vector2Int(Math.Max(pos1.x,pos2.x), Math.Max(pos1.y,pos2.y));

            PrototypeRoom minRoom = GetRoomByVector(minPos);
            PrototypeRoom maxRoom = GetRoomByVector(maxPos);
            PrototypeDoorway minDoorway = null;
            PrototypeDoorway maxDoorway = null;

            if(minPos.x != maxPos.x)
            {
                foreach (PrototypeDoorway item in minRoom.GetRightDoorways())
                {
                    if(minPos == item.GetPosition())
                    {
                        minDoorway = item;
                    }

                    // Dont connect the doors if there is already a connection.
                    if(item.other!=null && item.other.GetRoom() == maxRoom)
                    {
                        return;
                    }
                }

                foreach (PrototypeDoorway item in maxRoom.GetLeftDoorways())
                {
                    if(maxPos == item.GetPosition())
                    {
                        maxDoorway = item;
                    }
                }
            }

            if(minPos.y != maxPos.y)
            {
                foreach (var item in minRoom.GetTopDoorways())
                {
                    if(minPos == item.GetPosition())
                    {
                        minDoorway = item;
                    }

                    // Dont connect the doors if there is already a connection.
                    if(item.other!=null && item.other.GetRoom() == maxRoom)
                    {
                        return;
                    }
                }

                foreach (var item in maxRoom.GetBottomDoorways())
                {
                    if(maxPos == item.GetPosition())
                    {
                        maxDoorway = item;
                    }
                }
            }


            if(minDoorway != null && maxDoorway != null)
            {
                minDoorway.other = maxDoorway;
                maxDoorway.other = minDoorway;
            }


        }

        public void RecursivelyAddRooms(PrototypeRoom room)
        {
            //Left edge / right edge
            Vector2Int newPos;
            PrototypeRoom newRoom;
            for (int i = 0; i < room.GetSize().y; i++)
            {
                // Left
                newPos =
                    new Vector2Int(room.GetPosition().x - 1,
                        room.GetPosition().y + i);
                if (statusGrid.Get(newPos) == RoomType.NO_ROOM)
                {
                    newRoom = MakeRoom(newPos);
                    AddRoomToHouse (newRoom);
                    ConnectDoorways(newPos, newPos + new Vector2Int(1, 0));
                    RecursivelyAddRooms (newRoom);
                }
                else if (statusGrid.Get(newPos) != RoomType.EMPTY)
                {
                    MaybeConnectDoorways(newPos, newPos + new Vector2Int(1, 0));
                }

                // Right
                newPos =
                    new Vector2Int(room.GetPosition().x + room.GetSize().x,
                        room.GetPosition().y + i);
                if (statusGrid.Get(newPos) == RoomType.NO_ROOM)
                {
                    newRoom = MakeRoom(newPos);
                    AddRoomToHouse (newRoom);
                    ConnectDoorways(newPos, newPos + new Vector2Int(-1, 0));
                    RecursivelyAddRooms (newRoom);
                }
                else if (statusGrid.Get(newPos) != RoomType.EMPTY)
                {
                    MaybeConnectDoorways(newPos, newPos + new Vector2Int(-1, 0));
                }
            }
            for (int i = 0; i < room.GetSize().x; i++)
            {
                // Top
                newPos =
                    new Vector2Int(room.GetPosition().x + i,
                        room.GetPosition().y + room.GetSize().y);
                if (statusGrid.Get(newPos) == RoomType.NO_ROOM)
                {
                    newRoom = MakeRoom(newPos);
                    AddRoomToHouse (newRoom);
                    ConnectDoorways(newPos, newPos + new Vector2Int(0, -1));
                    RecursivelyAddRooms (newRoom);
                }
                else if (statusGrid.Get(newPos) != RoomType.EMPTY)
                {
                    MaybeConnectDoorways(newPos, newPos + new Vector2Int(0, -1));
                }

                // Bottom
                newPos =
                    new Vector2Int(room.GetPosition().x + i,
                        room.GetPosition().y - 1);
                if (statusGrid.Get(newPos) == RoomType.NO_ROOM)
                {
                    newRoom = MakeRoom(newPos);
                    AddRoomToHouse (newRoom);
                    ConnectDoorways(newPos, newPos + new Vector2Int(0, 1));
                    RecursivelyAddRooms (newRoom);
                }
                else if (statusGrid.Get(newPos) != RoomType.EMPTY)
                {
                    MaybeConnectDoorways(newPos, newPos + new Vector2Int(0, 1));
                }
            }
        }

        public GenerationGrid<RoomType> GetStatusGrid()
        {
            return statusGrid;
        }

        public PrototypeRoom MakeRoom(Vector2Int location)
        {
            List<Vector2Int> list = new List<Vector2Int>();
            PrototypeRoom room;

            // Attempt to add a hallway
            for (int i = 2; i < 7; i++)
            {
                list
                    .AddRange(CheckRoomVector(location,
                    new Vector2Int(0, i),
                    RoomType.HALLWAY));
                list
                    .AddRange(CheckRoomVector(location,
                    new Vector2Int(i, 0),
                    RoomType.HALLWAY));
            }

            if (list.Count > 0)
            {
                Vector2Int vector =
                    list[UnityEngine.Random.Range(0, list.Count)];
                room =
                    new PrototypeRoom(RoomType.HALLWAY, location, vector, this);
                return room;
            }

            // Attempt to add a large room
            list
                .AddRange(CheckRoomVector(location,
                new Vector2Int(1, 2),
                RoomType.LARGE));
            list
                .AddRange(CheckRoomVector(location,
                new Vector2Int(2, 1),
                RoomType.LARGE));
            if (list.Count > 0)
            {
                Vector2Int vector =
                    list[UnityEngine.Random.Range(0, list.Count)];
                room =
                    new PrototypeRoom(RoomType.LARGE, location, vector, this);
                return room;
            }

            //Attempt a medium room
            list
                .AddRange(CheckRoomVector(location,
                new Vector2Int(1, 1),
                RoomType.NORMAL));
            if (list.Count > 0)
            {
                Vector2Int vector =
                    list[UnityEngine.Random.Range(0, list.Count)];
                room =
                    new PrototypeRoom(RoomType.NORMAL, location, vector, this);
                return room;
            }

            //Attempt a medium room
            list
                .AddRange(CheckRoomVector(location,
                new Vector2Int(0, 1),
                RoomType.LONG));
            list
                .AddRange(CheckRoomVector(location,
                new Vector2Int(1, 0),
                RoomType.LONG));
            if (list.Count > 0)
            {
                Vector2Int vector =
                    list[UnityEngine.Random.Range(0, list.Count)];
                room = new PrototypeRoom(RoomType.LONG, location, vector, this);
                return room;
            }

            //Give up
            room = new PrototypeRoom(RoomType.SMALL, location, location, this);
            return room;
        }

        private List<Vector2Int>
        CheckRoomVector(Vector2Int location, Vector2Int vector, RoomType type)
        {
            List<Vector2Int> roomVectors = new List<Vector2Int>();
            Vector2Int v1 = new Vector2Int(vector.x, vector.y);
            Vector2Int v2 = new Vector2Int(-vector.x, vector.y);
            Vector2Int v3 = new Vector2Int(vector.x, -vector.y);
            Vector2Int v4 = new Vector2Int(-vector.x, -vector.y);

            foreach (var thisVector in new List<Vector2Int> { v1, v2, v3, v4 })
            {
                Vector2Int location2 = location + thisVector;

                // Check if the space is empty
                bool isEmpty = true;
                for (
                    int x = Math.Min(location.x, location2.x);
                    x <= Math.Max(location.x, location2.x);
                    x++
                )
                {
                    for (
                        int y = Math.Min(location.y, location2.y);
                        y <= Math.Max(location.y, location2.y);
                        y++
                    )
                    {
                        if (statusGrid.Get(x, y) != RoomType.NO_ROOM)
                        {
                            isEmpty = false;
                        }
                    }
                }

                bool isValid = true;

                // Check surrounding rooms
                if (type == RoomType.HALLWAY || type == RoomType.LARGE)
                {
                    int radius = 1;
                    if (type == RoomType.LARGE) radius = 5;
                    for (
                        int x = Math.Min(location.x, location2.x) - radius;
                        x <= Math.Max(location.x, location2.x) + radius;
                        x++
                    )
                    {
                        for (
                            int y = Math.Min(location.y, location2.y) - radius;
                            y <= Math.Max(location.y, location2.y) + radius;
                            y++
                        )
                        {
                            if (statusGrid.Get(x, y) == type)
                            {
                                isValid = false;
                            }
                        }
                    }
                }

                if (isEmpty && isValid)
                {
                    roomVectors.Add (location2);
                }
            }

            return roomVectors;
        }
    }

    public class PrototypeRoom
    {
        public RoomType roomType;

        public Vector2Int position;

        public Vector2Int size;

        public int seed;

        public PrototypeHouse house;

        public List<PrototypeDoorway> leftDoorways;

        public List<PrototypeDoorway> topDoorways;

        public List<PrototypeDoorway> rightDoorways;

        public List<PrototypeDoorway> bottomDoorways;

        public PrototypeRoom() {}

        public PrototypeRoom(
            RoomType roomType,
            Vector2Int vector1,
            Vector2Int vector2,
            PrototypeHouse house
        )
        {
            this.roomType = roomType;

            leftDoorways = new List<PrototypeDoorway>();
            topDoorways = new List<PrototypeDoorway>();
            rightDoorways = new List<PrototypeDoorway>();
            bottomDoorways = new List<PrototypeDoorway>();

            position =
                new Vector2Int(Math.Min(vector1.x, vector2.x),
                    Math.Min(vector1.y, vector2.y));
            size =
                new Vector2Int(Math.Max(vector1.x, vector2.x),
                    Math.Max(vector1.y, vector2.y));
            size = size - position + new Vector2Int(1, 1);
            this.seed = (int)UnityEngine.Random.Range(0,999999999);

            for (int i = position.x; i < position.x + size.x; i++)
            {
                topDoorways
                    .Add(new PrototypeDoorway(this,
                        new Vector2Int(i, position.y + size.y - 1)));
                bottomDoorways
                    .Add(new PrototypeDoorway(this,
                        new Vector2Int(i, position.y)));
            }

            for (int i = position.y; i < position.y + size.y; i++)
            {
                leftDoorways
                    .Add(new PrototypeDoorway(this,
                        new Vector2Int(position.x, i)));
                rightDoorways
                    .Add(new PrototypeDoorway(this,
                        new Vector2Int(position.x + size.x - 1, i)));
            }

            this.house = house;
        }

        public List<PrototypeDoorway> GetLeftDoorways() {return leftDoorways;}
        public List<PrototypeDoorway> GetRightDoorways() {return rightDoorways;}
        public List<PrototypeDoorway> GetTopDoorways() {return topDoorways;}
        public List<PrototypeDoorway> GetBottomDoorways() {return bottomDoorways;}

        public RoomType GetRoomType()
        {
            return roomType;
        }

        public Vector2Int GetPosition()
        {
            return position;
        }

        public Vector2Int GetSize()
        {
            return size;
        }

        public int GetSeed()
        {
            return seed;
        }
    }

    public class PrototypeDoorway
    {
        public PrototypeRoom room;

        public Vector2Int position;

        public PrototypeDoorway other = null;

        public PrototypeDoorway(PrototypeRoom room, Vector2Int position)
        {
            this.room = room;
            this.position = position;
        }

        public PrototypeDoorway() {}

        public PrototypeRoom GetRoom()
        {
            return room;
        }

        public Vector2Int GetPosition()
        {
            return position;
        }
    }

    public class GenerationGrid<T>
    {
        public GenerationPlane<GenerationPlane<T>> grid;

        public int minX;

        public int maxX;

        public int minY;

        public int maxY;

        public GenerationGrid()
        {
            minX = 0;
            maxX = 0;
            minY = 0;
            maxY = 0;
            grid = new GenerationPlane<GenerationPlane<T>>();
        }

        public void SetRange(Vector2Int bottomLeft, Vector2Int topRight, T value)
        {
            for(int x = bottomLeft.x; x<=topRight.x; x++)
            {
                for(int y = bottomLeft.y; y<=topRight.y; y++)
                {
                    Set(x,y,value);
                }
            }
        }

        public int CountRange(Vector2Int bottomLeft, Vector2Int topRight, T value)
        {
            int count = 0;
            for(int x = bottomLeft.x; x<=topRight.x; x++)
            {
                for(int y = bottomLeft.y; y<=topRight.y; y++)
                {
                    if(Get(x,y).Equals(value))
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        public T Get(Vector2Int vector)
        {
            return Get(vector.x, vector.y);
        }

        public T Get(int x, int y)
        {
            GenerationPlane<T> plane = grid.Get(x);
            if (plane == null)
            {
                plane = new GenerationPlane<T>();
                grid.Set (x, plane);
            }

            return plane.Get(y);
        }

        public void Set(Vector2Int vector, T value)
        {
            Set(vector.x, vector.y, value);
        }

        public void Set(int x, int y, T value)
        {
            if (x > maxX) maxX = x;
            if (x < minX) minX = x;
            if (y > maxY) maxY = y;
            if (y < minY) minY = y;

            GenerationPlane<T> plane = grid.Get(x);
            if (plane == null)
            {
                plane = new GenerationPlane<T>();
                grid.Set (x, plane);
            }
            plane.Set (y, value);
        }

        public void PrintToConsole()
        {
            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                }
            }
        }

        public int CountValue(T value)
        {
            int count = 0;
            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    if (Get(x, y).Equals(value)) count++;
                }
            }
            return count;
        }

        public int GetMinX()
        {
            return minX;
        }

        public int GetMaxX()
        {
            return maxX;
        }

        public int GetMinY()
        {
            return minY;
        }

        public int GetMaxY()
        {
            return maxY;
        }
    }

    public class GenerationPlane<T>
    {
        public T[] array;

        public int min;

        public int max;

        public GenerationPlane()
        {
            min = 0;
            max = 0;
            array = new T[1];
        }



        public void Set(int index, T value)
        {
            if (index < min)
            {
                int offset = min - index;
                T[] newArray = new T[array.Length + offset];
                for (int i = 0; i < array.Length; i++)
                {
                    newArray[i + offset] = array[i];
                }
                array = newArray;
                min = index;
            }
            else if (index > max)
            {
                int offset = index - max;
                T[] newArray = new T[array.Length + offset];
                for (int i = 0; i < array.Length; i++)
                {
                    newArray[i] = array[i];
                }
                array = newArray;
                max = index;
            }

            array[index - min] = value;
        }

        public T Get(int index)
        {
            if (index <= max && index >= min)
            {
                return array[index - min];
            }
            return default(T);
        }
    }

    public enum RoomType
    {
        EMPTY,
        NO_ROOM,
        LARGE,
        NORMAL,
        LONG,
        SMALL,
        HALLWAY
    }

    public enum SpaceType
    {
        ROOM_FILLED,
        ROOM_OPEN,
        ROOM_WALL_LEFT,
        ROOM_WALL_RIGHT,
        ROOM_WALL_TOP,
        ROOM_CORNER_LEFT,
        ROOM_CORNER_RIGHT
    }
}
