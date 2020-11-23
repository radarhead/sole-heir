using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoleHeir.GenerationUtils;
using Mirror;

namespace SoleHeir
{
    public class HouseController : NetworkBehaviour
    {
        [SyncVar]
        public int seed = 0;
        [SyncVar]
        public int houseSize = 30;
        private PrototypeHouse prototypeHouse;
        public GameObject roomPrefab;

        // Start is called before the first frame update
        void Start()
        {

        }

        public override void OnStartServer()
        {
            UnityEngine.Random.InitState (seed);
            //seed = Random.Range(0,999999999);
            prototypeHouse = new PrototypeHouse(seed, houseSize);
            foreach (PrototypeRoom prototypeRoom in prototypeHouse.GetRooms())
            {
                GameObject room = (GameObject) GameObject.Instantiate(roomPrefab, transform);
                room.GetComponent<RoomGenerator>().prototypeRoom = prototypeRoom;
                room.GetComponent<RoomGenerator>().roomPosition = prototypeRoom.GetPosition();
                NetworkServer.Spawn(room);

                room.GetComponent<RoomGenerator>().InitializeGrid();
                room.GetComponent<RoomGenerator>().AddSpawner();
                room.GetComponent<RoomGenerator>().Furnish();
                //room.GetComponent<RoomGenerator>().BuildRoom(prototypeRoom);
            }
        }

        public override void OnStartClient()
        {
            UnityEngine.Random.InitState (seed);
            prototypeHouse = new PrototypeHouse(seed, houseSize);
            foreach (PrototypeRoom prototypeRoom in prototypeHouse.GetRooms())
            {
                foreach(RoomGenerator roomGenerator in Object.FindObjectsOfType<RoomGenerator>())
                {
                    
                    if(roomGenerator.roomPosition == prototypeRoom.GetPosition())
                    {
                        roomGenerator.BuildRoom(prototypeRoom);
                    }
                }
            }

            foreach(RoomGenerator roomGenerator in Object.FindObjectsOfType<RoomGenerator>())
            {
                roomGenerator.transform.parent = transform;
            }
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}
