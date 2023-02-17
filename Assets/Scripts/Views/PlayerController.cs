using System;
using UnityEngine;

namespace Emir
{
    public class PlayerController : MonoBehaviour
    {
        private int Currency;

        private void Awake()
        {
            Initialize();
        }

        public void Initialize()
        {
            Currency = GameManager.Instance.GetCurreny();
        }

        /// <summary>
        /// This function helper for increase currency.
        /// </summary>
        /// <param name="amount"></param>
        public void IncreaseCurrency(int amount)
        {
            Currency += amount;
        }

        /// <summary>
        /// This function helper for decrease currency.
        /// </summary>
        /// <param name="amount"></param>
        public void DecreaseCurrency(int amount)
        {
            Currency -= amount;
        }

        /// <summary>
        /// This function helper for check currency.
        /// </summary>
        /// <param name="targetAmount"></param>
        /// <returns></returns>
        public bool CheckCurrency(float targetAmount)
        {
            return Currency >= targetAmount;
        }
    }
}