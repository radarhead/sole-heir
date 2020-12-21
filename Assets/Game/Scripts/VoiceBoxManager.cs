using UnityEngine;

namespace SoleHeir
{

    public class VoiceBoxManager : MonoBehaviour
    {
        public static VoiceBoxManager instance = null;

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
        }
    }
}
