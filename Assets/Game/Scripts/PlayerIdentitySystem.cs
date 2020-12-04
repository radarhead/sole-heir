using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace SoleHeir
{
    public class PlayerIdentitySystem : NetworkBehaviour
    {
        readonly SyncList<PlayerIdentityStruct> identities = new SyncList<PlayerIdentityStruct>();
        public PlayerIdentityStruct GetPIS(int id)
        {
            foreach(PlayerIdentityStruct pis in identities)
            {
                if(pis.id == id)
                {
                    return pis;
                }
            }
            return new PlayerIdentityStruct{
                id = -1,
                name = "None",
                color = new Color(0, 0, 0)
            };;
        }

        public PlayerIdentityStruct NewPIS(string pisName)
        {
            PlayerIdentityStruct pis = new PlayerIdentityStruct{
                id = identities.Count+1,
                name = pisName,
                color = new Color(Random.Range(0.5f,1f), Random.Range(0.5f,1f), Random.Range(0.5f,1f))
            };
            Debug.Log(identities.Count);

            identities.Add(pis);
            return pis;
        }
    }

    public struct PlayerIdentityStruct
    {
        public int id;
        public string name;
        public Color color;
    }
}