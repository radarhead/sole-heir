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

        // Start is called before the first frame update
        void Awake()
        {
            this.body = GetComponent<Rigidbody>();
            this.bullet = transform.Find("Bullet").gameObject;
            this.hole = transform.Find("Hole").gameObject;
        }
        void Start()
        {
            transform.parent = BulletManager.instance.transform;
            angle = body.velocity;
            if(!isServer)
            {
                body.detectCollisions = false;
            }
        }

        void OnCollisionEnter(Collision collision) {
            if(isServer)
            {
                Killable killable = null;
                IShootable shootable = null;

                foreach(var contact in collision.contacts)
                {
                    killable = killable ?? contact.otherCollider?.attachedRigidbody?.GetComponent<Killable>();
                    shootable = shootable ?? contact.otherCollider?.attachedRigidbody?.GetComponent<IShootable>();
                }
                
                killable?.DealDamage(damage, body.velocity, player.identity);
                shootable?.Shoot(this);

                body.detectCollisions = false;
                transform.rotation = Quaternion.LookRotation(collision.contacts[0].normal, Vector3.up);
                RpcCollide();
            }
        }
        
        [ClientRpc]
        void RpcCollide()
        {
            body.isKinematic=true;
            hole.SetActive(true);
            bullet.SetActive(false);
        }
    }

}
