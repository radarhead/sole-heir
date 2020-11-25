using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoleHeir.GenerationUtils;
using Mirror;

namespace SoleHeir
{
    public class RoomGenerator : NetworkBehaviour
    {
        [SyncVar]
        public Vector2 roomPosition;
        public int seed;
        public float roomWidth = 8.0f;
        public float roomHeight = 6.0f;
        public float roomSpacing = 2.0f;
        public float wallHeight = 3.0f;
        public GameObject floorPrefab;
        public GenerationGrid<SpaceType> roomGrid;
        public GameObject furniturePrefab;
        public GameObject spawnerPrefab;
        public Vector3 bottomLeft;
        public Vector3 topRight;
        private int gridSize = 2;
        private float xSize;
        private float ySize;
        public PrototypeRoom prototypeRoom;
        public GameObject lightSource;
        private int gridSpacing = 1;

        public Texture2D wallTexture;
        private bool isLocalRoom = true;
        private float alpha = 0.0f;
        public float velocity = 3.0f;



        // Start is called before the first frame update
       public void BuildRoom(PrototypeRoom prototypeRoom)
        {
            this.prototypeRoom = prototypeRoom;
            this.seed = prototypeRoom.GetSeed();
            roomGrid = new GenerationGrid<SpaceType>();
            xSize = prototypeRoom.GetSize().x*(roomWidth+roomSpacing)-roomSpacing;
            ySize = prototypeRoom.GetSize().y*(roomHeight+roomSpacing)-roomSpacing;
            UnityEngine.Random.InitState(prototypeRoom.GetSeed());

            transform.position = new Vector3(prototypeRoom.GetPosition().x*(roomWidth+roomSpacing),0,prototypeRoom.GetPosition().y*(roomHeight+roomSpacing));
            
            GameObject floor = transform.Find("Floor").gameObject;
            floor.GetComponent<FloorGenerator>().Initialize(prototypeRoom, roomWidth, roomHeight, roomSpacing);


            GameObject leftWall = transform.Find("LeftWall").gameObject;
            leftWall.GetComponent<WallGenerator>().Initialize(prototypeRoom.GetLeftDoorways(), roomHeight, wallHeight, roomSpacing);
            
            GameObject rightWall = transform.Find("RightWall").gameObject;
            rightWall.GetComponent<WallGenerator>().Initialize(prototypeRoom.GetRightDoorways(), roomHeight, wallHeight, roomSpacing);
            rightWall.transform.localPosition += new Vector3(xSize,0,0);
            rightWall.transform.localScale = new Vector3(1,-1,1);
            
            GameObject topWall = transform.Find("TopWall").gameObject;
            topWall.GetComponent<WallGenerator>().Initialize(prototypeRoom.GetTopDoorways(), roomWidth, wallHeight, roomSpacing);
            topWall.transform.localPosition += new Vector3(0,0,ySize);

            GameObject bottomWall = transform.Find("BottomWall").gameObject;
            bottomWall.GetComponent<WallGenerator>().Initialize(prototypeRoom.GetBottomDoorways(), roomWidth, wallHeight, roomSpacing);
            bottomWall.transform.localScale = new Vector3(1,-1,1);

            //Make bottom wall invisible
            foreach(var item in bottomWall.GetComponentsInChildren<MeshRenderer>())
            {
                item.enabled = false;
            }


            GameObject top = transform.Find("Top").gameObject;
            CreateMesh topMesh = top.GetComponent(typeof(CreateMesh)) as CreateMesh;
            topMesh.AddMesh(new Vector2(-roomSpacing/2,-roomSpacing/2), new Vector2(0, ySize+roomSpacing/2));
            topMesh.AddMesh(new Vector2(0,ySize), new Vector2(xSize, ySize+roomSpacing/2));
            topMesh.AddMesh(new Vector2(xSize,-roomSpacing/2), new Vector2(xSize+roomSpacing/2, ySize+roomSpacing/2));
            top.transform.localPosition = new Vector3(0,wallHeight,0);

            GameObject front = transform.Find("Front").gameObject;
            CreateMesh frontMesh = front.GetComponent(typeof(CreateMesh)) as CreateMesh;
            frontMesh.AddMesh(new Vector2(-roomSpacing/2,-roomSpacing/2), new Vector2(0,wallHeight));
            frontMesh.AddMesh(new Vector2(0,-roomSpacing/2), new Vector2(xSize,0));
            frontMesh.AddMesh(new Vector2(xSize,-roomSpacing/2), new Vector2(xSize+roomSpacing/2,wallHeight));

            GameObject darkLeft = transform.Find("DarkWallLeft").gameObject;
            CreateMesh darkLeftMesh = darkLeft.GetComponent(typeof(CreateMesh)) as CreateMesh;
            darkLeftMesh.AddMesh(new Vector2(-roomSpacing/2,0), new Vector2(ySize, wallHeight));
            darkLeftMesh.DisableCollisions();
            darkLeft.transform.localPosition = new Vector3(-roomSpacing/2, wallHeight, 0);

            GameObject darkRight = transform.Find("DarkWallRight").gameObject;
            CreateMesh darkRightMesh = darkRight.GetComponent(typeof(CreateMesh)) as CreateMesh;
            darkRightMesh.AddMesh(new Vector2(-roomSpacing/2,0), new Vector2(ySize, wallHeight));
            darkRightMesh.DisableCollisions();
            darkRight.transform.localPosition = new Vector3(xSize+roomSpacing/2, 0, 0);

            GameObject topFront = transform.Find("TopFront").gameObject;
            CreateMesh topFrontMesh = topFront.GetComponent(typeof(CreateMesh)) as CreateMesh;
            topFrontMesh.AddMesh(new Vector2(0,-roomSpacing/2), new Vector2(xSize, 0));
            topFrontMesh.transform.localPosition = new Vector3(0,wallHeight,-0.2f);

            GameObject topFront2 = transform.Find("TopFront2").gameObject;
            CreateMesh topFront2Mesh = topFront2.GetComponent(typeof(CreateMesh)) as CreateMesh;
            topFront2Mesh.AddMesh(new Vector2(0,-roomSpacing/2), new Vector2(xSize, 0));
            topFront2Mesh.transform.localPosition = new Vector3(0,wallHeight,0);


            GameObject topFull = transform.Find("TopFull").gameObject;
            CreateMesh topFullMesh = topFull.GetComponent(typeof(CreateMesh)) as CreateMesh;
            topFullMesh.AddMesh(new Vector2(0,-roomSpacing/2), new Vector2(xSize, ySize));
            topFullMesh.transform.localPosition = new Vector3(0,wallHeight,0);

            
            GameObject frontFront = transform.Find("FrontFront").gameObject;
            CreateMesh frontFrontMesh = frontFront.GetComponent(typeof(CreateMesh)) as CreateMesh;
            frontFrontMesh.AddMesh(new Vector2(-roomSpacing/2,0), new Vector2(xSize+roomSpacing/2, wallHeight));
            frontFrontMesh.DisableCollisions();
            frontFront.transform.localRotation = Quaternion.Euler(-90,0,0);
            frontFront.transform.localPosition = new Vector3(0,0,-roomSpacing/2);

            

            bottomLeft = transform.position;
            topRight = bottomLeft + new Vector3(xSize,0,ySize);

            UnityEngine.Object[] wallTextures = Resources.LoadAll("Textures/Wallpapers");
            Texture2D thisWallTexture = wallTextures[UnityEngine.Random.Range(0, wallTextures.Length)] as Texture2D;
            SetTextureByName(thisWallTexture, "Wall");

            UnityEngine.Object[] wallPanelingTextures = Resources.LoadAll("Textures/WallPaneling");
            Texture2D thisWallPanelingTexture = wallPanelingTextures[UnityEngine.Random.Range(0, wallPanelingTextures.Length)] as Texture2D;
            SetTextureByName(thisWallPanelingTexture, "WallPaneling");

            UnityEngine.Object[] floorTextures = Resources.LoadAll("Textures/Flooring");
            Texture2D thisFloorTexture = floorTextures[UnityEngine.Random.Range(0, floorTextures.Length)] as Texture2D;
            SetTextureByName(thisFloorTexture, "Floor");

            CreateLighting();
        }

        // Update is called once per frame
        void Update()
        {
            
            if(!isLocalRoom)
            {
                alpha += velocity * Time.deltaTime;
                if(alpha>1.0f) alpha = 1.0f;
            }
            else
            {
                alpha -= velocity * Time.deltaTime;
                if(alpha<0.0f) alpha = 0.0f;
            }


            SetTargetTransparent(transform.Find("TopFull").gameObject, (alpha*4)/5f);
            SetTargetTransparent(transform.Find("TopFront2").gameObject, alpha);
            foreach (var i in gameObject.GetComponentsInChildren<Light>())
            {
                Light l = i as Light;
                if(alpha == 1.0f)
                {
                    i.enabled = false;
                }
                else
                {
                    i.enabled = true;
                }
                l.intensity = (1-alpha);
            }
            //SetTargetTransparent(transform.Find("FrontFront").gameObject, alpha);
        }

        public void SetEnabled(bool enabled)
        {
            this.isLocalRoom = enabled;
        }

        public void InitializeGrid()
        {
            UnityEngine.Random.InitState (prototypeRoom.GetSeed());
            
            transform.position = new Vector3(prototypeRoom.GetPosition().x*(roomWidth+roomSpacing),0,prototypeRoom.GetPosition().y*(roomHeight+roomSpacing));

            roomGrid = new GenerationGrid<SpaceType>();
            xSize = prototypeRoom.GetSize().x*(roomWidth+roomSpacing)-roomSpacing;
            ySize = prototypeRoom.GetSize().y*(roomHeight+roomSpacing)-roomSpacing;
            // Fill the grid
            roomGrid.SetRange(FloorVec(new Vector2(0,0)), FloorVec(new Vector2(xSize-1, ySize-1))*gridSize, SpaceType.ROOM_OPEN);

            // Here we would fill in the fake walls
            // TODO!

            // Add the walls and corners
            for(int x=0; x<xSize*gridSize; x++)
            {
                for(int y=0; y<ySize*gridSize; y++)
                {
                    if(!roomGrid.Get(x,y).Equals(SpaceType.ROOM_OPEN))
                    {
                        continue;
                    }

                    // Left wall
                    else if(roomGrid.Get(x-1,y).Equals(SpaceType.ROOM_FILLED))
                    {
                        roomGrid.Set(x,y,SpaceType.ROOM_WALL_LEFT);
                    }

                    // Right wall
                    else if(roomGrid.Get(x+1,y).Equals(SpaceType.ROOM_FILLED))
                    {
                        roomGrid.Set(x,y,SpaceType.ROOM_WALL_RIGHT);
                    }

                    // Top wall
                    else if(roomGrid.Get(x,y+1).Equals(SpaceType.ROOM_FILLED))
                    {
                        roomGrid.Set(x,y,SpaceType.ROOM_WALL_TOP);
                    }

                                        // Left corner
                    if(roomGrid.Get(x-1,y).Equals(SpaceType.ROOM_FILLED) && roomGrid.Get(x,y+1).Equals(SpaceType.ROOM_FILLED))
                    {
                        roomGrid.Set(x,y,SpaceType.ROOM_CORNER_LEFT);
                    }

                    // Right corner
                    if(roomGrid.Get(x+1,y).Equals(SpaceType.ROOM_FILLED) && roomGrid.Get(x,y+1).Equals(SpaceType.ROOM_FILLED))
                    {
                        roomGrid.Set(x,y,SpaceType.ROOM_CORNER_RIGHT);
                    }

                }
            }

            // Make space for doors
            float doorSpacing = 3;

            // Left
            for (int i = 0; i < prototypeRoom.GetLeftDoorways().Count; i++)
            {
                PrototypeDoorway doorway = prototypeRoom.GetLeftDoorways()[i];
                if(doorway.other != null)
                {
                    float doorWidth = 2;
                    float doorY = (i+1)*(roomSpacing+roomHeight)-roomSpacing-roomHeight/2;
                    roomGrid.SetRange(
                        EzVec(0, doorY-doorWidth/2),
                        EzVec(doorSpacing, doorY+doorWidth/2),
                        SpaceType.ROOM_FILLED);
                }

            }
            
            // Top
            for (int i = 0; i < prototypeRoom.GetTopDoorways().Count; i++)
            {
                PrototypeDoorway doorway = prototypeRoom.GetTopDoorways()[i];
                if(doorway.other != null)
                {
                    float doorWidth = 2;
                    float doorX = (i+1)*(roomSpacing+roomWidth)-roomSpacing-roomWidth/2;
                    
                    roomGrid.SetRange(
                        EzVec(doorX-doorWidth/2, ySize-doorSpacing),
                        EzVec(doorX+doorWidth/2, ySize),
                        SpaceType.ROOM_FILLED);
                }
            }

            // Right
            for (int i = 0; i < prototypeRoom.GetRightDoorways().Count; i++)
            {
                PrototypeDoorway doorway = prototypeRoom.GetRightDoorways()[i];
                if(doorway.other != null)
                {
                    float doorWidth = 2;
                    float doorY = (i+1)*(roomSpacing+roomHeight)-roomSpacing-roomHeight/2;
                    roomGrid.SetRange(
                        EzVec(xSize-doorSpacing, doorY-doorWidth/2),
                        EzVec(xSize, doorY+doorWidth/2),
                        SpaceType.ROOM_FILLED);
                }
            }
        }

        public void AddSpawner()
        {

            // Place the spawner
            List<Vector2Int> availableSpaces = new List<Vector2Int>();
            for(int x=0; x<=roomGrid.GetMaxX(); x++)
            {
                for(int y=0; y<=roomGrid.GetMaxY(); y++)
                {
                    if(roomGrid.Get(x,y) == SpaceType.ROOM_OPEN)
                    {
                        availableSpaces.Add(new Vector2Int(x,y));
                    }
                }
            }

            if(availableSpaces.Count > 0)
            {
                Vector2Int randomPosition = availableSpaces[UnityEngine.Random.Range(0, availableSpaces.Count - 1)];
                GameObject spawner = Instantiate(spawnerPrefab, transform);
                spawner.transform.localPosition = new Vector3(randomPosition.x, 0,randomPosition.y+gridSize)/gridSize;
                NetworkServer.Spawn(spawner);
            }
        }


        public void Furnish()
        {
            List<Vector2Int> availableSpaces = new List<Vector2Int>();
            // Place the furniture
            UnityEngine.Object[] furniture = Resources.LoadAll("Furniture");
            for (int i=0; i<20; i++)
            {
                int index = UnityEngine.Random.Range(0, furniture.Length);
                GameObject cube = (GameObject)furniture[index];
                availableSpaces = new List<Vector2Int>();

                for(int x=0; x<=roomGrid.GetMaxX(); x++)
                {
                    for(int y=0; y<=roomGrid.GetMaxY(); y++)
                    {
                        if(CheckFurniturePlacement(cube, new Vector2Int(x,y)))
                        {
                            availableSpaces.Add(new Vector2Int(x,y));
                        }

                    }
                }

                if(availableSpaces.Count > 0)
                {
                    //GameObject epic = GameObject.Instantiate(cube, transform);

                    Vector2Int randomPosition = availableSpaces[UnityEngine.Random.Range(0, availableSpaces.Count - 1)];
                    //epic.transform.localPosition= new Vector3((float)randomPosition.x/gridSize,0,(float)randomPosition.y/gridSize);
                    PlaceFurniture(cube, randomPosition, index);
                }
            }
        }

        private void PlaceFurniture(GameObject furnitureObject, Vector2Int position, int index)
        {
            SpaceType spaceType = roomGrid.Get(position);
            Vector2Int offset1 = new Vector2Int(-gridSpacing, -gridSpacing);
            Vector2Int offset2 = new Vector2Int(gridSpacing, gridSpacing);
            Furniture furniture = furnitureObject.GetComponent<Furniture>();
            GameObject furnitureParent = Instantiate(furniturePrefab, transform) as GameObject;
            furnitureParent.GetComponent<FurnitureController>().resourcesIndex = index;
            furnitureParent.GetComponent<FurnitureController>().room = netIdentity;
            NetworkServer.Spawn(furnitureParent);
            if(spaceType == SpaceType.ROOM_OPEN)
            {
                roomGrid.SetRange(
                    position + FloorVec(0,-GetModifiedHeight(furniture)) + offset1,
                    position + FloorVec(GetModifiedWidth(furniture), 0) + offset2,
                    SpaceType.ROOM_FILLED);
                furnitureParent.transform.localPosition = new Vector3(position.x, 0,position.y+gridSize)/gridSize;
            }
            else if( spaceType == SpaceType.ROOM_WALL_TOP)
            {
                roomGrid.SetRange(
                    position + FloorVec(0,-GetModifiedHeight(furniture)) + offset1,
                    position + FloorVec(GetModifiedWidth(furniture), 0) + offset2,
                    SpaceType.ROOM_FILLED);
                furnitureParent.transform.localPosition = new Vector3(position.x, 0,position.y+gridSize)/gridSize;
            }
            else if(spaceType == SpaceType.ROOM_WALL_LEFT)
            {
                roomGrid.SetRange(
                    position + offset1,
                    position + FloorVec(GetModifiedHeight(furniture), GetModifiedWidth(furniture)) + offset2,
                    SpaceType.ROOM_FILLED);
                furnitureParent.transform.localRotation = Quaternion.Euler(0,-90,0);
                furnitureParent.transform.localPosition = new Vector3(position.x, 0,position.y)/gridSize;
            }
            else if(spaceType == SpaceType.ROOM_WALL_RIGHT)
            {
                roomGrid.SetRange(
                    position + FloorVec(-GetModifiedHeight(furniture), -GetModifiedWidth(furniture)) + offset1,
                    position + offset2,
                    SpaceType.ROOM_FILLED);
                furnitureParent.transform.localRotation = Quaternion.Euler(0,90,0);
                furnitureParent.transform.localPosition = new Vector3(position.x+gridSize, 0,position.y+gridSize)/gridSize;

                
            }
            else if(spaceType == SpaceType.ROOM_CORNER_LEFT)
            {
                roomGrid.SetRange(
                    position + FloorVec(0,-GetModifiedHeight(furniture)) + offset1,
                    position + FloorVec(GetModifiedWidth(furniture), 0) + offset2,
                    SpaceType.ROOM_FILLED);
                furnitureParent.transform.localPosition = new Vector3(position.x, 0,position.y+gridSize)/gridSize;
            }
            else if(spaceType == SpaceType.ROOM_CORNER_RIGHT)
            {
                //roomGrid.SetRange(position+EzVec(-(furniture.width-1),-(furniture.height-1))+offset1, position+EzVec(0, 0)+offset2, SpaceType.ROOM_FILLED);
                roomGrid.SetRange(
                    position + FloorVec(-GetModifiedWidth(furniture),-GetModifiedHeight(furniture)) + offset1,
                    position + FloorVec(0, 0) + offset2,
                    SpaceType.ROOM_FILLED);
                furnitureParent.transform.localScale = new Vector3(-1,1,1);
                furnitureParent.transform.localPosition = new Vector3(position.x+gridSize, 0,position.y+gridSize)/gridSize;
            }

            
        }

        private bool CheckFurniturePlacement(GameObject furnitureObject, Vector2Int position)
        {
            SpaceType spaceType = roomGrid.Get(position);
            Vector2Int offset1 = new Vector2Int(-0, -0);
            Vector2Int offset2 = new Vector2Int(0, 0);
            Furniture furniture = furnitureObject.GetComponent<Furniture>();

            if(spaceType == SpaceType.ROOM_OPEN)
            {
                if(furniture.type != FurnitureType.CENTER) return false;
                return (roomGrid.CountRange(
                    position + FloorVec(0,-GetModifiedHeight(furniture)) + offset1 - FloorVec(2,2)*gridSize,
                    position + FloorVec(GetModifiedWidth(furniture), 0) + offset2 + FloorVec(2,2)*gridSize,
                    SpaceType.ROOM_FILLED) == 0);
            }
            else if(spaceType == SpaceType.ROOM_WALL_TOP)
            {

                if(furniture.type != FurnitureType.WALL) return false;
                return roomGrid.CountRange(
                    position + FloorVec(0,-GetModifiedHeight(furniture)) + offset1,
                    position + FloorVec(GetModifiedWidth(furniture), 0) + offset2,
                    SpaceType.ROOM_FILLED)==0;
            }
            else if(spaceType == SpaceType.ROOM_WALL_LEFT)
            {
                if(furniture.type != FurnitureType.WALL) return false;
                return roomGrid.CountRange(
                    position + offset1,
                    position + FloorVec(GetModifiedHeight(furniture), GetModifiedWidth(furniture)) + offset2,
                    SpaceType.ROOM_FILLED)==0;
                //return (roomGrid.CountRange(position, position+EzVec((furniture.width-1), (furniture.height-1)), SpaceType.ROOM_FILLED)==0);
            }
            else if(spaceType == SpaceType.ROOM_WALL_RIGHT)
            {
                if(furniture.type != FurnitureType.WALL) return false;
                return roomGrid.CountRange(
                    position + FloorVec(-GetModifiedHeight(furniture), -GetModifiedWidth(furniture)) + offset1,
                    position + offset2,
                    SpaceType.ROOM_FILLED)==0;
                //return (roomGrid.CountRange(position+EzVec(-(furniture.width-1), -(furniture.height-1)), position+EzVec(0, 0), SpaceType.ROOM_FILLED)==0);
            }
            
            else if(spaceType == SpaceType.ROOM_CORNER_LEFT)
            {
                if(furniture.type != FurnitureType.CORNER) return false;
                return roomGrid.CountRange(
                    position + FloorVec(0,-GetModifiedHeight(furniture)) + offset1,
                    position + FloorVec(GetModifiedWidth(furniture), 0) + offset2,
                    SpaceType.ROOM_FILLED)==0;
                //return (roomGrid.CountRange(position+EzVec(0,-(furniture.height-1)), position+EzVec((furniture.width-1), 0), SpaceType.ROOM_FILLED)==0);
            }

            else if(spaceType == SpaceType.ROOM_CORNER_RIGHT)
            {
                if(furniture.type != FurnitureType.CORNER) return false;
                return roomGrid.CountRange(
                    position + FloorVec(-GetModifiedWidth(furniture),-GetModifiedHeight(furniture)) + offset1,
                    position + FloorVec(0, 0) + offset2,
                    SpaceType.ROOM_FILLED)==0;
                //return (roomGrid.CountRange(position+EzVec(-(furniture.width-1),-(furniture.height-1)), position+EzVec(0, 0), SpaceType.ROOM_FILLED)==0);
            }
            return false;
        }

        private Vector2Int FloorVec(Vector2 vec)
        {
            return new Vector2Int((int)Math.Floor(vec.x), (int)Math.Floor(vec.y));
        }

        private Vector2Int FloorVec(float x, float y)
        {
            return new Vector2Int((int)Math.Floor(x), (int)Math.Floor(y));
        }

        private float GetModifiedWidth(Furniture furniture)
        {
            return (furniture.width * gridSize) - 1;
        }

        private float GetModifiedHeight(Furniture furniture)
        {
            return (furniture.height * gridSize) - 1;
        }

        private Vector2Int EzVec(float width, float height)
        {
            return new Vector2Int((int)Math.Floor(width*gridSize), (int)Math.Floor(height*gridSize));
        }

        void SetTargetTransparent(GameObject target, float transparency)
        {
            Component[] a = target.GetComponentsInChildren(typeof(Renderer));
            foreach (Component b in a)
            {
                Renderer c = (Renderer)b;
                c.material.SetColor("_BaseColor", new Color(0,0,0, transparency));
            }
        }

        private void SetTextureByName(Texture2D texture, String name)
        {
            foreach(var i in gameObject.GetComponentsInChildren<MeshRenderer>())
            {
                MeshRenderer r = i as MeshRenderer;
                if(r.material.name == name || r.material.name == name+" (Instance)")
                {
                    r.material.SetTexture("_MainTex", texture);
                }
            }
        }

        private void CreateLighting()
        {
            for(int x = 0; x<prototypeRoom.GetSize().x; x++)
            {
                for(int y=0; y<prototypeRoom.GetSize().y; y++)
                {
                    GameObject light = GameObject.Instantiate(lightSource, transform.Find("Light"));
                    light.transform.localPosition = new Vector3((x+0.5f)*(roomWidth+roomSpacing)-roomSpacing/2,0,(y+0.5f)*(roomHeight+roomSpacing)-roomSpacing/2);
                }
            }
            transform.Find("Light").localPosition = new Vector3(0,wallHeight+1,0);
        }
    }
}
