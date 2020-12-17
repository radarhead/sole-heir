using UnityEngine;

namespace SoleHeir
{

    [System.Serializable]
    public class PlayerTraitOutcome
    {
        public string description;
        public string clue;
    }

    [System.Serializable]
    public class PlayerTraitSubstitution
    {
        public string tag;
        public string[] values;
    }

    [System.Serializable]
    public class PlayerTrait
    {
        public string name;
        public PlayerTraitOutcome outcome1;
        public PlayerTraitOutcome outcome2;
    }

    [System.Serializable]
    public class PlayerTraitCollection
    {
        public PlayerTrait[] traits;
        public PlayerTraitSubstitution[] substitutions;

    }

    public class PlayerTraitManager : MonoBehaviour
    {
        public static PlayerTraitManager instance = null;

        public PlayerTrait[] traits;
        public PlayerTraitSubstitution[] substitutions;

        void Awake()
        {
            if(instance == null)
            {
                var c = JsonUtility.FromJson<PlayerTraitCollection>(Resources.Load<TextAsset>("Traits").text);
                this.traits = c.traits;
                this.substitutions = c.substitutions;
                instance = this;
            }
        }
    }
}