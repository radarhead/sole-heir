using UnityEngine;
using UnityEngine.UI;
using Mirror;

namespace SoleHeir
{
    public class PlayerProfileTraitController : MonoBehaviour
    {
        public Text valueText;
        public PlayerTrait trait;
        public bool isTrue;

        void Update()
        {
            if(isTrue)
            {
                valueText.text = trait.outcome1.description;
            }
            else
            {
                valueText.text = trait.outcome2.description;
            }
        }

    }
}
