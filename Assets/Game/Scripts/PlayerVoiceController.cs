using UnityEngine;
using Mirror;
using Dissonance;

namespace SoleHeir
{
    public class PlayerVoiceController : MonoBehaviour, Dissonance.IDissonancePlayer
    {
        private BoxCollider collider;
        public PlayerController player;
        public string PlayerId {get{return player.dissonancePlayerName;}}

        public Vector3 Position {get {return player.transform.position;}}

        public Quaternion Rotation {get {return player.transform.rotation;}}
        public NetworkPlayerType Type {get {return player.isLocalPlayer ? NetworkPlayerType.Local : NetworkPlayerType.Remote;}}

        public bool IsTracking {get {return player.isActiveAndEnabled;}}

        void Awake()
        {
            collider = GetComponent<BoxCollider>();
            player = GetComponentInParent<PlayerController>();
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;
        }

        void Update()
        {
            RoomGenerator currentRoom = HelperMethods.FindCurrentRoom(player);
            if(currentRoom != null)
            {
                Vector3 bounds = (currentRoom.topRight-currentRoom.bottomLeft);
                transform.position = currentRoom.bottomLeft + bounds/2 + new Vector3(0,5,0);
                transform.rotation = Quaternion.identity;
                collider.size = new Vector3(bounds.x + currentRoom.roomSpacing, 10, bounds.z +  + currentRoom.roomSpacing);
            }
        }
    }
}