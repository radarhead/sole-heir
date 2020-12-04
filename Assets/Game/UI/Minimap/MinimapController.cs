using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoleHeir.GenerationUtils;


namespace SoleHeir
{
    public class MinimapController : MonoBehaviour
    {
        public GameObject rooms;
        public GameObject minimapRoomPrefab;
        public int roomSize;

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            // Add rooms
            foreach(RoomGenerator room in rooms.GetComponentsInChildren<RoomGenerator>())
            {
                bool hasMinimap = false;
                foreach(MinimapRoom mmr in GetComponentsInChildren<MinimapRoom>())
                {
                    if(room == mmr.room) hasMinimap = true;
                }

                if(!hasMinimap)
                {
                    GameObject mmrgo = Instantiate(minimapRoomPrefab, transform);
                    MinimapRoom mmr = mmrgo.GetComponent<MinimapRoom>();
                    mmr.room = room;
                }
            }

            // Remove rooms
            foreach(MinimapRoom mmr in GetComponentsInChildren<MinimapRoom>())
            {
                bool roomExists = false;
                foreach(RoomGenerator room in rooms.GetComponentsInChildren<RoomGenerator>())
                {
                    if(room == mmr.room) roomExists = true;
                }

                if(!roomExists)
                {
                    GameObject.Destroy(mmr.gameObject);
                }
            }

            // Align the map
            PrototypeHouse hausu = rooms.GetComponent<HouseController>().prototypeHouse;
            if(rooms.GetComponent<HouseController>().prototypeHouse != null)
            {
                transform.localPosition = new Vector3(-(hausu.statusGrid.GetMaxX()+1)*roomSize, -(hausu.statusGrid.GetMinY())*roomSize, 0);
            }
        }
    }
}
