using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoleHeir.GenerationUtils;

namespace SoleHeir
{
    public class HouseController : MonoBehaviour
    {
        public int seed = 0;
        public int houseSize = 30;
        private PrototypeHouse prototypeHouse;
        public GameObject roomPrefab;

        // Start is called before the first frame update
        void Start()
        {
            //seed = Random.Range(0,999999999);
            Initialize();
        }

        // Update is called once per frame
        void Update()
        {
        }

        public void Initialize()
        {

            prototypeHouse = new PrototypeHouse(seed, houseSize);

            foreach (PrototypeRoom prototypeRoom in prototypeHouse.GetRooms())
            {
                GameObject room = (GameObject) GameObject.Instantiate(roomPrefab, Vector3.zero, Quaternion.identity);
                room.GetComponent<RoomGenerator>().Initialize(prototypeRoom);
            }
        }
    }
}
