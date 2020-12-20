using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace SoleHeir
{

    public class BasicKit : NetworkBehaviour, IKitAction
    {
        public KitController controller;
        public Carryable carryable;
        [SyncVar] public int trait;

        void Start()
        {
            this.controller = GetComponent<KitController>();
            this.carryable = GetComponentInParent<Carryable>();

            if(carryable.isServer)
            {
                trait = Random.Range(0, PlayerTraitManager.instance.traits.Length);
            }
        }

        public bool CanUse(GameObject go)
        {
            return go?.GetComponent<CorpseController>()!=null;
        }

        public void PerformAction(PlayerController player, GameObject target, bool sabotaged)
        {
            CorpseController corpse = target.GetComponent<CorpseController>();
            PlayerIdentityStruct tVicIdentity;
            PlayerIdentityStruct tKiller;
            /*
            tVicIdentity = PlayerIdentitySystem.instance.GetPIS(tCarryable.GetInt(ParentInts.PlayerIdentity));

            bool outcome = tKiller.traits[trait] ^ sabotaged;
            
            string fullString = null;
            if(outcome) fullString = trait.outcome1.clue;
            else fullString = trait.outcome2.clue;
            foreach(var sub in PlayerTraitManager.instance.substitutions)
            {
                fullString = fullString.Replace(sub.tag, sub.values[Random.Range(0,sub.values.Length)]);
            }
            fullString = fullString.Replace("VICTIM", tVicIdentity.name);

            SetString(ParentStrings.OnScreenText, fullString);
            Debug.Log(fullString);*/
        }
    }
}