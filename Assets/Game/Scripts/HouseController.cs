using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
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
        public NavMeshSurface surface;
        public RoomGenerator playerRoom;

        public static HouseController instance = null;

        void Awake()
        {
            if(instance == null)
            {
                instance = this;
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            this.surface = GetComponent<NavMeshSurface>();

            foreach(Furniture furniture in Resources.LoadAll<Furniture>("Furniture"))
            {
                ClientScene.RegisterPrefab(furniture.gameObject);
            }

            foreach(Carryable carryable in Resources.LoadAll<Carryable>("Carryable"))
            {
                ClientScene.RegisterPrefab(carryable.gameObject);
            }

            if(isServer)
            {
                UnityEngine.Random.InitState (seed);
                prototypeHouse = new PrototypeHouse(seed, houseSize);
                foreach (PrototypeRoom prototypeRoom in prototypeHouse.GetRooms())
                {
                    GameObject room = (GameObject) GameObject.Instantiate(roomPrefab, transform);
                    room.GetComponent<RoomGenerator>().SetPrototype(prototypeRoom);
                    room.GetComponent<RoomGenerator>().Initialize();
                    NetworkServer.Spawn(room);
                    Physics.SyncTransforms();
                    room.GetComponent<RoomGenerator>().Furnish();
                }
                GameObject npc = Instantiate(npcPrefab, Vector3.zero, Quaternion.identity);
                NetworkServer.Spawn(npc);

                SpawnItems();

                surface.BuildNavMesh();

            }
            
        }

        void SpawnItems()
        {
            Carryable[] carryables = Resources.LoadAll<Carryable>("Carryables");

            Inventory[] inventories = FurnitureManager.instance.GetComponentsInChildren<Inventory>();

            Random.InitState(System.DateTime.Now.Millisecond);

            while(inventories.Length > 1)
            {
                float sum = carryables.Sum(c => c.rarity);
                float rarityValue = UnityEngine.Random.Range(0f, sum);
                Carryable carryable = null;

                foreach(var c in carryables) {
                    rarityValue-=c.rarity;
                    if(rarityValue<=0)
                    {
                        carryable = c;
                        break;
                    }
                }

                Carryable coolGuy = Instantiate(carryable);
                NetworkServer.Spawn(coolGuy.gameObject);

                Inventory inventory = inventories[Random.Range(0, inventories.Length)];
                inventory.Insert(coolGuy);
                inventories = inventories.Where(c => !c.IsFull()).ToArray();
            }
        }


        // Update is called once per frame
        void Update()
        {

            if(ClientScene.localPlayer != null)
            {
                PlayerController pc = ClientScene.localPlayer.GetComponent<PlayerController>();
                RoomGenerator currentRoom = HelperMethods.FindCurrentRoom(pc);

                foreach(RoomGenerator rg in GetComponentsInChildren<RoomGenerator>())
                {
                    rg.SetEnabled(rg==currentRoom);
                }
                if(pc!=null &&currentRoom != null)
                {
                    Shader.SetGlobalColor("_PaletteDarkColor",
                        Color.Lerp(
                            Shader.GetGlobalColor("_PaletteDarkColor"),
                            currentRoom.colorPalette.Dark(),
                            Time.deltaTime*5
                        )
                    );
                    Shader.SetGlobalColor("_PaletteDarkAccentColor",
                        Color.Lerp(
                            Shader.GetGlobalColor("_PaletteDarkAccentColor"),
                            currentRoom.colorPalette.DarkAccent(),
                            Time.deltaTime*5
                        )
                    );
                    Shader.SetGlobalColor("_PalettePrimaryColor",
                        Color.Lerp(
                            Shader.GetGlobalColor("_PalettePrimaryColor"),
                            currentRoom.colorPalette.Primary(),
                            Time.deltaTime*5
                        )
                    );
                    Shader.SetGlobalColor("_PaletteLightAccentColor",
                        Color.Lerp(
                            Shader.GetGlobalColor("_PaletteLightAccentColor"),
                            currentRoom.colorPalette.LightAccent(),
                            Time.deltaTime*5
                        )
                    );
                    Shader.SetGlobalColor("_PaletteLightColor",
                        Color.Lerp(
                            Shader.GetGlobalColor("_PaletteLightColor"),
                            currentRoom.colorPalette.Light(),
                            Time.deltaTime*5
                        )
                    );

                    Shader.SetGlobalVector("_TopRight", currentRoom.topRight);
                    Shader.SetGlobalVector("_BottomLeft", currentRoom.bottomLeft);
                    Shader.SetGlobalInt("_LightsOut", currentRoom.lights ? 0 : 1);
                    Shader.SetGlobalVector("_PlayerPosition", pc.transform.position);
                    
                }
            }
            
        }
    }
}
