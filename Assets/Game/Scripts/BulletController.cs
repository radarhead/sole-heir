using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace SoleHeir
{
    public class BulletController : NetworkBehaviour
    {
        private NetworkIdentity playerIdentity;
        public PlayerController player {get {return playerIdentity?.GetComponent<PlayerController>();} set{playerIdentity = value.netIdentity;}}
        public float damage;

        private Vector3 angle;
        public GameObject hole;
        public GameObject bullet;
        public Rigidbody body;
        private Vector3 lastPosition;
        private bool dead = false;


        // Start is called before the first frame update
        void Awake()
        {
            this.body = GetComponent<Rigidbody>();
            this.bullet = transform.Find("Bullet").gameObject;
            this.hole = transform.Find("Hole").gameObject;
            hole.SetActive(false);
            bullet.SetActive(true);
        }
        void Start()
        {
            transform.parent = BulletManager.instance.transform;
            angle = body.velocity;
            if(!isServer)
            {
                body.detectCollisions = false;
            }
            lastPosition = transform.position;
        }

        void Update()
        {
            if(isServer && !dead)
            {
                if(Physics.Raycast(lastPosition, (transform.position - lastPosition).normalized, out var hit, Vector3.Distance(transform.position, lastPosition), LayerMask.GetMask("House", "Furniture", "Players")))
                {
                    dead=true;
                    hit.collider.attachedRigidbody?.GetComponent<Killable>()?.DealDamage(damage, body.velocity, player.identity);
                    hit.collider.attachedRigidbody?.GetComponent<IShootable>()?.Shoot(this);

                    transform.rotation = Quaternion.LookRotation(hit.normal, Vector3.up);
                    transform.position = hit.point + 0.01f * hit.normal;

                    if(LayerMask.GetMask("House") == (LayerMask.GetMask("House") | (1 << hit.collider.gameObject.layer)))
                    {
                        RpcCollide(CollideEffect.Hole);
                    }
                    else
                    {
                        RpcCollide(CollideEffect.None);
                    }

                }
                lastPosition = body.position;
            }
            
        }


        
        
        [ClientRpc]
        void RpcCollide(CollideEffect effect)
        {
            body.isKinematic=true;
            body.velocity = Vector3.zero;
            if(effect == CollideEffect.Hole)
            {
                hole.SetActive(true);
            }
            bullet.SetActive(false);
        }
    }

    public enum CollideEffect
    {
        None,
        Blood,
        Hole
    }

}
