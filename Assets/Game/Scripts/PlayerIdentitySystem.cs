using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace SoleHeir
{
    public class PlayerIdentitySystem : NetworkBehaviour
    {
        public readonly SyncList<PlayerIdentityStruct> identities = new SyncList<PlayerIdentityStruct>();
        public static PlayerIdentitySystem instance = null;
        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
        }

        void Start()
        {
            for (int i = 0; i < 10; i++)
            {
                NewPIS("Sure");
            }
        }

        public PlayerIdentityStruct GetPIS(int id)
        {
            foreach (PlayerIdentityStruct pis in identities)
            {
                if (pis.id == id)
                {
                    return pis;
                }
            }
            return new PlayerIdentityStruct
            {
                id = -1,
                name = "None",
                realName = "None",
                traits = new bool[PlayerTraitManager.instance.traits.Length]
            }; ;
        }
        public PlayerIdentityStruct NewPIS(string pisName)
        {
            if (!isServer) return new PlayerIdentityStruct();
            
            PlayerIdentityStruct pis = new PlayerIdentityStruct
            {
                id = identities.Count,
                realName = pisName,
                name = pisName,
                traits = new bool[PlayerTraitManager.instance.traits.Length]
            };

            identities.Add(pis);
            ShuffleTraits();
            ReassignNames();
            return pis;
        }

        public void ReassignNames()
        {
            for (int i = 0; i < identities.Count; i++)
            {
                var ident = identities[i];
                int nameCount = identities.FindAll(x => x.realName == ident.realName && ident.id > x.id).Count;
                if (nameCount != 0)
                {
                    ident.name = System.String.Format("{0} ({1})", ident.realName, nameCount);
                }
                identities[i] = ident;
            }
        }

        public void ShuffleTraits()
        {
            PlayerTraitManager tm = PlayerTraitManager.instance;
            Random.InitState(System.DateTime.Now.Millisecond);
            for (int i = 0; i < tm.traits.Length; i++)
            {

                bool baseValue = (Random.Range(0f, 1f) > 0.5f);

                List<int> newPlayerList = new List<int>();
                for (int j = 0; j < identities.Count; j++) newPlayerList.Add(j);
                var shuffledList = newPlayerList.OrderBy((a) => { return Random.Range(0, 100); });

                int count=0;
                foreach(int idx in shuffledList)
                {
                    var ident = this.identities[idx];
                    ident.traits[i] = baseValue ^ (count < newPlayerList.Count / 2);
                    this.identities[idx] = ident;
                    count++;
                }
            }
        }
    }

    public struct PlayerIdentityStruct
    {
        public int id;
        public string name;
        public bool[] traits;
        public string realName;
    }
}