using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace SoleHeir
{
    public class NetworkParentWithAttributes : NetworkBehaviour
    {
        [SyncVar] private int bools=0;
        readonly SyncDictionary<short, int> ints = new SyncDictionary<short, int>();
        readonly SyncDictionary<short, float> floats = new SyncDictionary<short, float>();
        readonly SyncDictionary<short, string> strings = new SyncDictionary<short, string>();

        public bool GetBool(short idx)
        {
            return (bools & 1 << (idx)) != 0;
        }

        public void SetBool(short idx, bool value)
        {
            if(!isServer) return;
            if(value) bools |= (1 << idx);
            else bools &= ~(1 << idx);
        }

        public float GetFloat(short idx)
        {
            return floats[idx];
        }

        public void SetFloat(short idx, float value)
        {
            if(!isServer) return;
            floats[idx] = value;
        }

        public int GetInt(short idx)
        {
            return ints[idx];
        }

        public void SetInt(short idx, int value)
        {
            if(!isServer) return;
            ints[idx] = value;
        }

        public string GetString(short idx)
        {
            return strings[idx];
        }

        public void SetString(short idx, string value)
        {
            if(!isServer) return;
            strings[idx] = value;
        }
    }

    
}