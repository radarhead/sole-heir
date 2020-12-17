using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using SoleHeir.GenerationUtils;
using Mirror;

namespace SoleHeir
{
    public class RoomGenerator : NetworkBehaviour
    {
        public int seed;
        public float roomWidth = 8.0f;
        public float roomHeight = 6.0f;
        public float roomSpacing = 2.0f;
        public float wallHeight = 3.0f;
        public GameObject floorPrefab;
        public GenerationGrid<SpaceType> roomGrid;
        public GameObject furniturePrefab;
        public GameObject doorGetter;
        public ColorPalette colorPalette;
        public GameObject spawnerPrefab;
        public Vector3 bottomLeft;
        public Vector3 topRight;
        private int gridSize = 2;
        private float xSize;
        private float ySize;
        [SyncVar] public PrototypeRoom prototypeRoom;
        [SyncVar] public bool lights = true;
        public bool built = false;

        public bool isLocalRoom = true;


        void Start()
        {
            if(!built)
            {
                BuildRoom();
            }
        }

        public void BuildRoom()
        {
            this.seed = prototypeRoom.GetSeed();
            roomGrid = new GenerationGrid<SpaceType>();
            xSize = prototypeRoom.GetSize().x * (roomWidth + roomSpacing) - roomSpacing;
            ySize = prototypeRoom.GetSize().y * (roomHeight + roomSpacing) - roomSpacing;
            UnityEngine.Random.InitState(prototypeRoom.GetSeed());

            transform.position = new Vector3(prototypeRoom.GetPosition().x * (roomWidth + roomSpacing), 0, prototypeRoom.GetPosition().y * (roomHeight + roomSpacing));

            GameObject floor = transform.Find("Floor").gameObject;
            floor.GetComponent<FloorGenerator>().Initialize(prototypeRoom, roomWidth, roomHeight, roomSpacing);


            GameObject leftWall = transform.Find("LeftWall").gameObject;
            leftWall.GetComponent<WallGenerator>().Initialize(prototypeRoom.GetLeftDoorways(), roomHeight, wallHeight, roomSpacing);

            GameObject rightWall = transform.Find("RightWall").gameObject;
            rightWall.GetComponent<WallGenerator>().Initialize(prototypeRoom.GetRightDoorways(), roomHeight, wallHeight, roomSpacing);
            rightWall.transform.localPosition += new Vector3(xSize, 0, 0);
            rightWall.transform.localScale = new Vector3(1, -1, 1);

            GameObject topWall = transform.Find("TopWall").gameObject;
            topWall.GetComponent<WallGenerator>().Initialize(prototypeRoom.GetTopDoorways(), roomWidth, wallHeight, roomSpacing);
            topWall.transform.localPosition += new Vector3(0, 0, ySize);

            GameObject bottomWall = transform.Find("BottomWall").gameObject;
            bottomWall.GetComponent<WallGenerator>().Initialize(prototypeRoom.GetBottomDoorways(), roomWidth, wallHeight, roomSpacing);
            bottomWall.transform.localScale = new Vector3(1, -1, 1);

            GameObject top = transform.Find("Top").gameObject;
            CreateMesh topMesh = top.GetComponent(typeof(CreateMesh)) as CreateMesh;
            topMesh.AddMesh(new Vector2(-roomSpacing / 2, -roomSpacing / 2), new Vector2(0, ySize + roomSpacing / 2));
            topMesh.AddMesh(new Vector2(0, ySize), new Vector2(xSize, ySize + roomSpacing / 2));
            topMesh.AddMesh(new Vector2(xSize, -roomSpacing / 2), new Vector2(xSize + roomSpacing / 2, ySize + roomSpacing / 2));
            top.transform.localPosition = new Vector3(0, wallHeight, 0);

            GameObject front = transform.Find("Front").gameObject;
            CreateMesh frontMesh = front.GetComponent(typeof(CreateMesh)) as CreateMesh;
            frontMesh.AddMesh(new Vector2(-roomSpacing / 2, -roomSpacing / 2), new Vector2(0, wallHeight));
            frontMesh.AddMesh(new Vector2(0, -roomSpacing / 2), new Vector2(xSize, 0));
            frontMesh.AddMesh(new Vector2(xSize, -roomSpacing / 2), new Vector2(xSize + roomSpacing / 2, wallHeight));

            GameObject darkLeft = transform.Find("DarkWallLeft").gameObject;
            CreateMesh darkLeftMesh = darkLeft.GetComponent(typeof(CreateMesh)) as CreateMesh;
            darkLeftMesh.AddMesh(new Vector2(-roomSpacing / 2, 0), new Vector2(ySize, wallHeight));
            darkLeftMesh.DisableCollisions();
            darkLeft.transform.localPosition = new Vector3(-roomSpacing / 2, wallHeight, 0);

            GameObject darkRight = transform.Find("DarkWallRight").gameObject;
            CreateMesh darkRightMesh = darkRight.GetComponent(typeof(CreateMesh)) as CreateMesh;
            darkRightMesh.AddMesh(new Vector2(-roomSpacing / 2, 0), new Vector2(ySize, wallHeight));
            darkRightMesh.DisableCollisions();
            darkRight.transform.localPosition = new Vector3(xSize + roomSpacing / 2, 0, 0);

            GameObject topFront = transform.Find("TopFront").gameObject;
            CreateMesh topFrontMesh = topFront.GetComponent(typeof(CreateMesh)) as CreateMesh;
            topFrontMesh.AddMesh(new Vector2(0, 0), new Vector2(xSize, roomSpacing / 2));
            topFrontMesh.transform.localPosition = new Vector3(0, wallHeight, -roomSpacing / 2 - 0.2f);

            GameObject topFront2 = transform.Find("TopFront2").gameObject;
            CreateMesh topFront2Mesh = topFront2.GetComponent(typeof(CreateMesh)) as CreateMesh;
            topFront2Mesh.AddMesh(new Vector2(0, -roomSpacing / 2), new Vector2(xSize, 0));
            topFront2Mesh.transform.localPosition = new Vector3(0, wallHeight, 0);

            GameObject frontFront = transform.Find("FrontFront").gameObject;
            CreateMesh frontFrontMesh = frontFront.GetComponent(typeof(CreateMesh)) as CreateMesh;
            frontFrontMesh.AddMesh(new Vector2(-roomSpacing / 2, 0), new Vector2(xSize + roomSpacing / 2, wallHeight));
            frontFrontMesh.DisableCollisions();
            frontFront.transform.localRotation = Quaternion.Euler(-90, 0, 0);
            frontFront.transform.localPosition = new Vector3(0, 0, -roomSpacing / 2);

            bottomLeft = transform.position;
            topRight = bottomLeft + new Vector3(xSize, 0, ySize);

            colorPalette = ColorManager.instance.RandomPalette();
            this.built = true;
        }

        // Update is called once per frame
        void Update()
        {

            //Make bottom wall invisible
            foreach (var item in transform.Find("BottomWall").gameObject.GetComponentsInChildren<MeshRenderer>())
            {
                item.enabled = !isLocalRoom;
            }
        }

        public void SetEnabled(bool enabled)
        {
            this.isLocalRoom = enabled;
        }

        public void AddSpawner()
        {

            // Place the spawner
            List<Vector2Int> availableSpaces = new List<Vector2Int>();
            for (int x = 0; x <= roomGrid.GetMaxX(); x++)
            {
                for (int y = 0; y <= roomGrid.GetMaxY(); y++)
                {
                    if (roomGrid.Get(x, y) == SpaceType.ROOM_OPEN)
                    {
                        availableSpaces.Add(new Vector2Int(x, y));
                    }
                }
            }

            if (availableSpaces.Count > 0)
            {
                Vector2Int randomPosition = availableSpaces[UnityEngine.Random.Range(0, availableSpaces.Count - 1)];
                GameObject spawner = Instantiate(spawnerPrefab, transform);
                spawner.transform.localPosition = new Vector3(randomPosition.x, 0, randomPosition.y + gridSize) / gridSize;
                NetworkServer.Spawn(spawner);
            }
        }


        public void Furnish()
        {
            List<Vector2Int> availableSpaces = new List<Vector2Int>();
            // Place the furniture
            Furniture[] furnitureList = Resources.LoadAll<Furniture>("Furniture");
            for (int i = 0; i < 20; i++)
            {
                float sum = furnitureList.Sum(c => c.rarity);
                float rarityValue = UnityEngine.Random.Range(0f, sum);
                Furniture furniture = null;

                foreach(var f in furnitureList) {
                    rarityValue-=f.rarity;
                    if(rarityValue<=0)
                    {
                        furniture = f;
                        break;
                    }
                }

                float iterAmount = 0.25f;

                var possibleLocations = new List<TransformHelper>();
                Bounds bounds = HelperMethods.LocalBounds(furniture);
                Bounds expandedBounds = HelperMethods.LocalBounds(furniture);
                expandedBounds.Expand(2f);

                float extraOutlineDistance = 0.02f;

                // Center
                if (furniture.type == FurnitureType.CENTER)
                {
                    for (float x = bottomLeft.x; x <= topRight.x; x += iterAmount)
                    {
                        for (float z = bottomLeft.z; z <= topRight.z; z += iterAmount)
                        {
                            Vector3 testPosition = new Vector3(x, bounds.extents.y + extraOutlineDistance, z) + furniture.offset;
                            if (
                                !Physics.CheckBox(testPosition, bounds.extents, Quaternion.identity, LayerMask.GetMask("House")) &&
                                !Physics.CheckBox(testPosition, expandedBounds.extents, Quaternion.identity, LayerMask.GetMask("Furniture")) &&
                                !Physics.CheckBox(testPosition, bounds.extents, Quaternion.identity, LayerMask.GetMask("Generation Spacing")))
                            {
                                TransformHelper helper = new TransformHelper
                                {
                                    position = testPosition,
                                    rotation = Quaternion.LookRotation(Vector3.back, Vector3.up)
                                };
                                possibleLocations.Add(helper);
                            }
                        }
                    }
                }

                else if (furniture.type == FurnitureType.WALL)
                {

                    // Top Wall
                    for (float x = bottomLeft.x; x <= topRight.x; x += iterAmount)
                    {
                        Vector3 testPosition = new Vector3(x, bounds.extents.y + extraOutlineDistance, topRight.z - bounds.extents.z - extraOutlineDistance) + furniture.offset;
                        Quaternion rotation = Quaternion.LookRotation(Vector3.back, Vector3.up);
                        if (
                            !Physics.CheckBox(testPosition, bounds.extents, rotation, LayerMask.GetMask("House")) &&
                            !Physics.CheckBox(testPosition, bounds.extents, rotation, LayerMask.GetMask("Furniture")) &&
                            !Physics.CheckBox(testPosition, bounds.extents, rotation, LayerMask.GetMask("Generation Spacing"))
                            )
                        {
                            TransformHelper helper = new TransformHelper
                            {
                                position = testPosition,
                                rotation = Quaternion.LookRotation(Vector3.back, Vector3.up)
                            };
                            possibleLocations.Add(helper);
                        }
                    }

                    // Left Wall
                    for (float z = bottomLeft.z; z <= topRight.z; z += iterAmount)
                    {
                        float x = bottomLeft.x + bounds.extents.x + extraOutlineDistance;
                        Vector3 testPosition = new Vector3(x, bounds.extents.y + extraOutlineDistance, z) + furniture.offset;
                        Quaternion rotation = Quaternion.LookRotation(Vector3.right, Vector3.up);
                        if (
                            !Physics.CheckBox(testPosition, bounds.extents, Quaternion.identity, LayerMask.GetMask("House")) &&
                            !Physics.CheckBox(testPosition, bounds.extents, Quaternion.identity, LayerMask.GetMask("Furniture")) &&
                            !Physics.CheckBox(testPosition, bounds.extents, Quaternion.identity, LayerMask.GetMask("Generation Spacing"))
                            )
                        {
                            TransformHelper helper = new TransformHelper
                            {
                                position = testPosition,
                                rotation = rotation
                            };
                            possibleLocations.Add(helper);
                        }
                    }

                    // Right Wall
                    for (float z = bottomLeft.z; z <= topRight.z; z += iterAmount)
                    {
                        float x = topRight.x - bounds.extents.x - extraOutlineDistance;
                        Quaternion rotation = Quaternion.LookRotation(Vector3.left, Vector3.up);
                        Vector3 testPosition = new Vector3(x, bounds.extents.y + extraOutlineDistance, z) + furniture.offset;
                        if (
                            !Physics.CheckBox(testPosition, bounds.extents, rotation, LayerMask.GetMask("House")) &&
                            !Physics.CheckBox(testPosition, bounds.extents, rotation, LayerMask.GetMask("Furniture")) &&
                            !Physics.CheckBox(testPosition, bounds.extents, rotation, LayerMask.GetMask("Generation Spacing"))
                            )
                        {
                            TransformHelper helper = new TransformHelper
                            {
                                position = testPosition,
                                rotation = rotation
                            };
                            possibleLocations.Add(helper);
                        }
                    }
                }


                if (possibleLocations.Count > 0)
                {
                    var randomPosition = possibleLocations[UnityEngine.Random.Range(0, possibleLocations.Count)];
                    var spawnedFurniture = Instantiate(furniture, randomPosition.position, randomPosition.rotation);
                    

                    NetworkServer.Spawn(spawnedFurniture.gameObject);
                }

                if(furniture.onePerRoom)
                {
                    furnitureList = furnitureList.Where(e => e.name != furniture.name).ToArray();
                }
            }
        }

        

        private Vector2Int FloorVec(Vector2 vec)
        {
            return new Vector2Int((int)Math.Floor(vec.x), (int)Math.Floor(vec.y));
        }

    }

    public struct TransformHelper
    {
        public Vector3 position;
        public Quaternion rotation;
    }
}
