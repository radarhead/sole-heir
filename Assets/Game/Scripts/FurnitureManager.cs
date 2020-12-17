
using UnityEngine;

namespace SoleHeir
{

    public class FurnitureManager : MonoBehaviour
    {
        public static FurnitureManager instance = null;

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
    }
}
