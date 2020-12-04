using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoleHeir.GenerationUtils;

namespace SoleHeir
{
    public class DoorGetter : MonoBehaviour
    {
        public GameObject normalDoor;
        public GameObject archedDoor;
        public GameObject wideDoor;
        public GameObject GetDoorType(PrototypeDoorway a)
        {
            if(a.room.roomType == RoomType.SMALL || a.other.room.roomType == RoomType.SMALL) return normalDoor;
            else if(a.room.roomType == RoomType.NORMAL && a.other.room.roomType == RoomType.NORMAL) return normalDoor;
            else if(a.room.roomType == RoomType.HALLWAY && a.other.room.roomType == RoomType.LARGE) return archedDoor;
            else if(a.room.roomType == RoomType.LARGE && a.other.room.roomType == RoomType.HALLWAY) return archedDoor;
            else return wideDoor;
        }
    }
}