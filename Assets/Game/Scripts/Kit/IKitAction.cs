using UnityEngine;

namespace SoleHeir
{
    public interface IKitAction
    {
        GameObject CanUse(GameObject go);
        string GetResult();
    }
}