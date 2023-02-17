using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;
using Sequence = DG.Tweening.Sequence;

namespace Emir
{
    public static class CommonTypes
    {
        //GENERICS
        public static int DEFAULT_FPS = 60;
        public static int DEFAULT_THREAD_SLEEP_MS = 100;

        //INTERFACES
        public static float UI_DEFAULT_FLY_CURRENCY_DURATION = 0.5F;

        //SOUNDS
        public static string SFX_CLICK = "CLICK";
        public static string SFX_CURRENCY_FLY = "CURRENCY_FLY";
        public static string SFX_WIN = "WIN";
        public static string SFX_LOSE = "LOSE";

        //DATA KEYS
        public static string PLAYER_DATA_KEY = "player_data";
        public static string LEVEL_ID_DATA_KEY = "level_data";
        public static string SOUND_STATE_KEY = "sound_state_data";
        public static string VIBRATION_STATE_KEY = "vibration_state_data";
        public static string CURRENCY_DATA_KEY = "Currency";

        public static string STORED_PICKAXE_KEY = "Pickaxes";

        //TAGS
        public static string GATE_TAG = "Gate";
        public static string GATE_SLOT_TAG = "GateSlot";
        public static string PICKAXES_TAG = "Pickaxe";
        public static string STAIR_COMPONENT = "Stair";

        //VALUES
        //buttonprice
        public static string ADD_AXE_PRICE = "AddAxe";
        public static string ADD_GATE_PRICE = "AddGate";
        public static string ADD_LAYER_PRICE = "AddLayer";
        public static string MERGE_AXE_PRICE = "MergeAxe";

        //gatevalue
        public static string GATE_2X = "Gate2X";
        public static string GATE_3X = "Gate3X";
        public static string GATE_4X = "Gate4X";

        //axe value
        public static string AXE_COUNT = "AxeCount";

        //layervalue
        public static string LAYER_COUNT = "LayerCount";

        public static string ISTUTORIAL = "Tutorial";

#if UNITY_EDITOR

        public static string EDITOR_LEVELS_PATH = "Levels/";
        public static string EDITOR_GAME_SETTINGS_PATH = "GameSettings";

#endif
    }

    public static class GameUtils
    {
        public static void SwitchCanvasGroup(CanvasGroup a, CanvasGroup b, float duration = 0.25F)
        {
            Sequence sequence = DOTween.Sequence();

            if (a != null)
                sequence.Join(a.DOFade(0, duration));
            if (b != null)
                sequence.Join(b.DOFade(1, duration));

            sequence.OnComplete(() =>
            {
                if (a != null)
                    a.blocksRaycasts = false;
                if (b != null)
                    b.blocksRaycasts = true;
            });

            sequence.Play();
        }

        public static Vector2 WorldToCanvasPosition(RectTransform canvas, Camera camera, Vector3 worldPosition)
        {
            Vector2 tempPosition = camera.WorldToViewportPoint(worldPosition);

            tempPosition.x *= canvas.sizeDelta.x;
            tempPosition.y *= canvas.sizeDelta.y;

            tempPosition.x -= canvas.sizeDelta.x * canvas.pivot.x;
            tempPosition.y -= canvas.sizeDelta.y * canvas.pivot.y;

            return tempPosition;
        }

        public static string ConvertMoney(this int num)
        {
            return num switch
            {
                >= 100000000 => (num / 1000000).ToString("$" + "#,0M"),
                >= 10000000 => (num / 1000000).ToString("$" + "0.#") + "M",
                >= 100000 => ((float)num / 1000f).ToString("$" + "#,0K"),
                >= 10000 => ((float)num / 1000f).ToString("$" + "0.##") + "K",
                >= 1000 => ((float)num / 1000f).ToString("$" + "#.##") + "K",
                _ => num.ToString("$" + "#,0")
            };
        }
    }
}