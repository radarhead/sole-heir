using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Mirror;

namespace SoleHeir
{
    public class PlayerProfileController : MonoBehaviour
    {
        public PlayerIdentityStruct identity;
        public GameObject traitPrefab;
        public GameObject traits;
        public Text nameText;

        void Update()
        {
            if(nameText.text != identity.name)
                nameText.text = identity.name;
            float yOffset = 0;
            for (int i = 0; i < identity.traits.Length; i++)
            {
                PlayerProfileTraitController pptc = System.Array.Find(
                    GetComponentsInChildren<PlayerProfileTraitController>(),
                    x => {return x.trait.name == PlayerTraitManager.instance.traits[i].name;}
                );


                if(pptc == null)
                {
                    var go = Instantiate(traitPrefab, traits.transform);
                    pptc = go.GetComponent<PlayerProfileTraitController>();
                }

                var rt = pptc.GetComponent<RectTransform>();
                rt.localPosition = new Vector3(0,yOffset,0);
                yOffset -= rt.rect.height;
                pptc.trait = PlayerTraitManager.instance.traits[i];
                pptc.isTrue = identity.traits[i];
            }
        }
    }
}
