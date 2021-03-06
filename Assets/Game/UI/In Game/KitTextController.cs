using UnityEngine;
using Mirror;

namespace SoleHeir
{
    public class KitTextController : MonoBehaviour
    {
        UnityEngine.UI.Text text;
        void Update()
        {
            PlayerController playerController;
            Carryable carryable;
            KitController kitController;
            if(
                ClientScene.localPlayer != null &&
                (playerController = ClientScene.localPlayer.GetComponent<PlayerController>()) != null &&
                (carryable = playerController.HeldItem()) != null &&
                (kitController = carryable.GetComponentInChildren<KitController>()) != null &&
                kitController.used
            )
        
            {
                //text.text = kitController.GetString(ParentStrings.OnScreenText);
            }
            else
            {
                text.text = "";
            }
        }

        void Start()
        {
            text = GetComponent<UnityEngine.UI.Text>();
        }
    }
}