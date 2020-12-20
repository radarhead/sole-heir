using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

namespace SoleHeir
{
    public class GameplayUIController : NetworkBehaviour
    {
        public GameObject ppcPrefab;
        public ScrollRect ppcGroup;
        public static GameplayUIController instance = null;

        void Awake()
        {
            if(instance == null)
            {
                instance = this;
            }
        }
        
        void Update()
        {
            UpdateIdentities();

            if( ClientScene.localPlayer != null && 
                ClientScene.localPlayer.TryGetComponent<PlayerController>(out var pc)
            )
            {
                GetComponent<Animator>().SetBool("playerList", pc.status == PlayerStatus.PLAYER_LIST);
            }

        }

        void UpdateIdentities()
        {
            var idents = PlayerIdentitySystem.instance.identities;
            // Check for id
            bool updated = false;
            foreach (var ident in idents)
            {
                if (System.Array.Find(
                    GetComponentsInChildren<PlayerProfileController>(),
                    x => { return x.identity.id == ident.id; }
                ) == null)
                {
                    var bff = Instantiate(ppcPrefab, ppcGroup.content.transform).GetComponent<PlayerProfileController>();
                    bff.identity = ident;
                    updated = true;
                }
            }

            if(updated)
            {
                // Resize the window
                var pprt = ppcGroup.GetComponent<RectTransform>();
                var rt = GetComponentInChildren<PlayerProfileController>().GetComponent<RectTransform>();
                pprt.sizeDelta = new Vector2(pprt.rect.width, rt.rect.height+20);
                ppcGroup.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rt.rect.width * idents.Count);

                // Relocate each rect
                float xOffset = 0;
                foreach (var ppc in GetComponentsInChildren<PlayerProfileController>())
                {
                    var rt2 = ppc.GetComponent<RectTransform>();
                    rt2.localPosition = new Vector3(xOffset, 0, 0);
                    xOffset += rt2.rect.width;
                }
            }
        }
    }
}