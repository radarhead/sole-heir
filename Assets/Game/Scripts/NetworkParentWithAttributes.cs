using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace SoleHeir
{
    public class NetworkParentWithAttributes : NetworkBehaviour
    {
        [SyncVar] private int bools=0;
        public bool GetBool(int idx)
        {
            return (bools & 1 << (idx)) != 0;
        }

        public void SetBool(int idx, bool value)
        {
            if(value) bools |= (1 << idx);
            else bools &= ~(1 << idx);
        }
    }
}