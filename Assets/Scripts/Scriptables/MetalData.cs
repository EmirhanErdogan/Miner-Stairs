using UnityEngine;

namespace Emir
{
    [CreateAssetMenu(menuName = "Emir/Default/Metal", fileName = "Metal", order = 2)]
    public class MetalData : ScriptableObject
    {
        [Header("General")] public EGemType Type;
        public int PoolCount;
        public int LayerCount;
        public int Speed;
    }
}