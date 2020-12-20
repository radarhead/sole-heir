using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace SoleHeir {
    public class GunController : NetworkBehaviour, ICarryableShoot
    {
        public float damage;
        public int count = 1;
        public float cooldownTime;
        public float reloadTime;
        public bool automatic;
        public int clipSize;
        public float inaccuracy;
        public BulletController bullet;

        [HideInInspector] public Carryable carryable;
        [HideInInspector] public PlayerController player {get {return carryable?.inventory?.GetComponent<PlayerController>();}}

        [SyncVar] private float reloadTimer=0f;
        [SyncVar] private float cooldownTimer=0f;
        [SyncVar]private int bulletsInClip;

        
        void Awake()
        {
            carryable = GetComponent<Carryable>();
            bulletsInClip = clipSize;
        }

        void Update()
        {
            cooldownTimer = Mathf.Max(0, cooldownTimer - Time.deltaTime);

            if(bulletsInClip == 0)
            {
                reloadTimer = Mathf.Max(0, reloadTimer - Time.deltaTime);
                if(reloadTimer == 0)
                {
                    bulletsInClip = clipSize;
                }
            }
            
            
            if(player != null && player.HeldItem()?.gameObject == this.gameObject)
            {
                if(player.holdingFire)
                {
                    if(automatic)
                    {
                        TryShootBullet();
                    }
                }
            }
            
        }

        void TryShootBullet()
        {

            if(cooldownTimer == 0 && bulletsInClip > 0)
            {
                for(int i=0; i<count; i++)
                {
                    Vector3 pos = player.transform.position + new Vector3(0,1,0);
                    BulletController bc = Instantiate(bullet, pos, Quaternion.identity);
                    bc.player = player;
                    bc.damage = damage;

                    if(player.aimTarget.y != 0)
                    {
                        bc.body.velocity = (player.aimTarget - pos).normalized*100;
                    }
                    else
                    {
                        bc.body.velocity = HelperMethods.GetPosition2D(player.aimTarget - pos).normalized*100;
                    }

                    bc.body.velocity += new Vector3(Random.Range(-inaccuracy, inaccuracy), Random.Range(-inaccuracy, inaccuracy)/10, Random.Range(-inaccuracy, inaccuracy));

                    NetworkServer.Spawn(bc.gameObject);
                }
                
                cooldownTimer = cooldownTime;
                bulletsInClip = Mathf.Max(0, bulletsInClip-1);
                if(bulletsInClip==0)
                {
                    reloadTimer=reloadTime;
                }
            }
        }

        public void Shoot()
        {
            if(!automatic) 
            {
                TryShootBullet();
            }
        }

    }
}

