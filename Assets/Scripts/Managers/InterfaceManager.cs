using TMPro;
using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Lofelt.NiceVibrations;
using UnityEngine;

namespace Emir
{
    public class InterfaceManager : Singleton<InterfaceManager>
    {
        #region Serialzable Fields

        [Header("Transforms")] [SerializeField]
        private RectTransform m_canvas;

        [SerializeField] private RectTransform m_currencySlot;

        [Header("Panels")]
        // [SerializeField] private UIWinPanel m_winPanel;
        // [SerializeField] private UILosePanel m_losePanel;
        [Header("Texts")]
        [SerializeField]
        private TMP_Text m_currencyText;

        [Header("Canvas Groups")] [SerializeField]
        private CanvasGroup SettingsGroup;

        [Header("Prefabs")] [SerializeField] private RectTransform m_currencyPrefab;

        #endregion


        private void Start()
        {
            _ = Initialize();
        }

        private async UniTask Initialize()
        {
            await UniTask.Delay(10);
            OnGameStateChanged(GameManager.Instance.GetGameState());
            OnPlayerCurrencyUpdated();
        }

        /// <summary>
        /// This function helper for fly currency animation to target currency icon.
        /// </summary>
        /// <param name="worldPosition"></param>
        public void FlyCurrencyFromWorld(Vector3 worldPosition)
        {
            Camera targetCamera = CameraManager.Instance.GetCamera();
            Vector3 screenPosition = GameUtils.WorldToCanvasPosition(m_canvas, targetCamera, worldPosition);
            Vector3 targetScreenPosition = m_canvas.InverseTransformPoint(m_currencySlot.position);

            RectTransform createdCurrency = Instantiate(m_currencyPrefab, m_canvas);
            createdCurrency.anchoredPosition = screenPosition;

            Sequence sequence = DOTween.Sequence();

            sequence.Join(createdCurrency.transform.DOLocalMove(targetScreenPosition, 0.75F));

            sequence.OnComplete(() => { Destroy(createdCurrency.gameObject); });

            sequence.Play();
        }

        /// <summary>
        /// This function helper for fly currency animation to target currency icon.
        /// </summary>
        /// <param name="screenPosition"></param>
        public void FlyCurrencyFromScreen(Vector3 screenPosition)
        {
            Vector3 targetScreenPosition = m_canvas.InverseTransformPoint(m_currencySlot.position);

            RectTransform createdCurrency = Instantiate(m_currencyPrefab, m_canvas);
            createdCurrency.position = screenPosition;

            Sequence sequence = DOTween.Sequence();

            sequence.Join(createdCurrency.transform.DOLocalMove(targetScreenPosition, 0.5F));

            sequence.OnComplete(() => { Destroy(createdCurrency.gameObject); });

            sequence.Play();
        }

        /// <summary>
        /// This function called when game state changed.
        /// </summary>
        /// <param name="e"></param>
        public void OnGameStateChanged(EGameState GameState)
        {
            switch (GameState)
            {
                case EGameState.STAND_BY:

                    break;
                case EGameState.STARTED:

                    break;
                case EGameState.WIN:

                    // m_winPanel.Initialize();

                    break;
                case EGameState.LOSE:

                    // m_losePanel.Initialize();

                    break;
            }
        }

        /// <summary>
        /// This function called when player currency updated.
        /// </summary>
        /// <param name="e"></param>
        public void OnPlayerCurrencyUpdated()
        {
            string currencyText = m_currencyText.text;

            currencyText = currencyText.Replace(".", String.Empty);
            currencyText = currencyText.Replace(",", String.Empty);

            int cachedCurrency = int.Parse(currencyText);

            Sequence sequence = DOTween.Sequence();

            sequence.Join(DOTween.To(() => cachedCurrency, x => cachedCurrency = x, GameManager.Instance.GetCurreny(),
                CommonTypes.UI_DEFAULT_FLY_CURRENCY_DURATION));

            sequence.OnUpdate(() =>
            {
                m_currencyText.text = $"{cachedCurrency.ToString("N0").Replace(",", String.Empty)}";
            });

            sequence.SetId(m_currencyText.GetInstanceID());
            sequence.Play();
        }

        /// <summary>
        /// This function helper for change settings panel state.
        /// </summary>
        /// <param name="state"></param>
        public void ChangeSettingsPanelState(bool state)
        {
            Debug.Log("dawdawd");
            if (DOTween.IsTweening(GetSettingsGroup().GetInstanceID()))
                return;

            Sequence sequence = DOTween.Sequence();

            sequence.Join(GetSettingsGroup().DOFade(state ? 1 : 0, 0.25F));
            sequence.OnStart(() =>
            {
                if (state)
                {
                    SoundManager.Instance.Play("CLICK");
                    HapticManager.Instance.PlayHaptic(HapticPatterns.PresetType.SoftImpact);
                }
            });
            sequence.OnComplete(() => { GetSettingsGroup().blocksRaycasts = state; });

            sequence.SetId(GetSettingsGroup().GetInstanceID());
            sequence.Play();
        }

        public CanvasGroup GetSettingsGroup()
        {
            return SettingsGroup;
        }
    }
}