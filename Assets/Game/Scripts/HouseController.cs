using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoleHeir.GenerationUtils;
using Mirror;

namespace SoleHeir
{
    public class HouseController : NetworkBehaviour
    {
        [SyncVar] public int seed = 0;
        [SyncVar] public int houseSize = 30;
        public Material darkness;
        public PrototypeHouse prototypeHouse;
        public GameObject npcPrefab;
        public GameObject roomPrefab;
        public RoomGenerator playerRoom;

        // Start is called before the first frame update
        void Start()
        {

        }

        public override void OnStartServer()
        {
            UnityEngine.Random.InitState (seed);
            prototypeHouse = new PrototypeHouse(seed, houseSize);
            foreach (PrototypeRoom prototypeRoom in prototypeHouse.GetRooms())
            {
                GameObject room = (GameObject) GameObject.Instantiate(roomPrefab, transform);
                room.GetComponent<RoomGenerator>().prototypeRoom = prototypeRoom;
                room.GetComponent<RoomGenerator>().roomPosition = prototypeRoom.GetPosition();
                NetworkServer.Spawn(room);

                room.GetComponent<RoomGenerator>().InitializeGrid();
                room.GetComponent<RoomGenerator>().Furnish();
                room.GetComponent<RoomGenerator>().AddSpawner();
                room.GetComponent<RoomGenerator>().BuildRoom(prototypeRoom);
                
            }
            GameObject npc = Instantiate(npcPrefab, Vector3.zero, Quaternion.identity);     
            NetworkServer.Spawn(npc);
        }

        public override void OnStartClient()
        {
            if(!isServer)
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
            
        }

        // Update is called once per frame
        void Update()
        {
            if(ClientScene.localPlayer != null)
            {
                PlayerController pc = ClientScene.localPlayer.GetComponent<PlayerController>();
                if(pc!=null &&pc.anonymousComponent.currentRoom != null)
                {
                    Shader.SetGlobalColor("_DarkColor",
                        Color.Lerp(
                            Shader.GetGlobalColor("_DarkColor"),
                            pc.anonymousComponent.currentRoom.colorPalette.Dark(),
                            Time.deltaTime*10
                        )
                    );
                    Shader.SetGlobalColor("_DarkAccentColor",
                        Color.Lerp(
                            Shader.GetGlobalColor("_DarkAccentColor"),
                            pc.anonymousComponent.currentRoom.colorPalette.DarkAccent(),
                            Time.deltaTime*10
                        )
                    );
                    Shader.SetGlobalColor("_PrimaryColor",
                        Color.Lerp(
                            Shader.GetGlobalColor("_PrimaryColor"),
                            pc.anonymousComponent.currentRoom.colorPalette.Primary(),
                            Time.deltaTime*10
                        )
                    );
                    Shader.SetGlobalColor("_LightAccentColor",
                        Color.Lerp(
                            Shader.GetGlobalColor("_LightAccentColor"),
                            pc.anonymousComponent.currentRoom.colorPalette.LightAccent(),
                            Time.deltaTime*10
                        )
                    );
                    Shader.SetGlobalColor("_LoghtColor1",
                        Color.Lerp(
                            Shader.GetGlobalColor("_LoghtColor1"),
                            pc.anonymousComponent.currentRoom.colorPalette.Light(),
                            Time.deltaTime*10
                        )
                    );
                    
                }
            }
        }
    }
}
