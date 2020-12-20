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
