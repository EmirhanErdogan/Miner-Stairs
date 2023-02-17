using UnityEngine;
using System.Collections.Generic;

namespace Emir
{
    [CreateAssetMenu(menuName = "Emir/Default/GameSettings", fileName = "GameSettings", order = 0)]
    public class GameSettings : ScriptableObject
    {
        [Header("Datas")] [ContextMenuItem("Update", "FindLevels")]
        public Level[] Levels;

        [Header("Data")] public int ActiveLayerCount;
        public List<PickaxeComponent> Pickaxes;

        [Header("Prefabs")] public List<PickaxeComponent> PickaxesPrefab;
        public GateComponent GatePrefab;
        public ChestComponent ChestPrefab;
        public List<Color> GateColor;

        [Header("Buttons")] public int AddPickaxeButton;
        public int AddGateButton;
        public int MergePickaxeButton;
        public int AddLayerButton;

        [Header("Economy")] public List<int> AxeAddPrice;
        public List<int> GateAddPrice;
        public List<int> AxeMergePrice;
        public List<int> LayerAddPrice;

        [Header("Color")] public Color DefaultButtonColor;
        public Color DeactiveButtonColor;

        public float ChestDestroyTimer;

#if UNITY_EDITOR

        /// <summary>
        /// This function helper for update levels list.
        /// </summary>
        public void FindLevels()
        {
            Levels = null;

            List<Level> foundLevels = new List<Level>();
            Object[] objects = Resources.LoadAll(CommonTypes.EDITOR_LEVELS_PATH);

            foreach (Object targetObject in objects)
            {
                if (targetObject is not Level)
                    continue;

                foundLevels.Add(targetObject as Level);
            }

            Levels = foundLevels.ToArray();
        }

#endif
    }
}