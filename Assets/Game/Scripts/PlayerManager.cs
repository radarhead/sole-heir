using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoleHeir
{

    public class PlayerManager : MonoBehaviour
    {
        public static PlayerManager instance = null;

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Destroy (gameObject);
            }
        }


        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}
