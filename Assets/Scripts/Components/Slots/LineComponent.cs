using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class LineComponent : MonoBehaviour
{
    #region Serailizable Fields

    [SerializeField] private List<Transform> Slots = new List<Transform>();
    [SerializeField] private bool IsEmpty = false;
    [SerializeField] private int Order;

    #endregion


    #region Getters

    public int GetOrder()
    {
        return Order;
    }

    public List<Transform> GetSlots()
    {
        return Slots;
    }

    public bool GetIsEmpty()
    {
        return IsEmpty;
    }

    #endregion

    #region Setters

    public void SetIsEmpty(bool Value)
    {
        IsEmpty = Value;
    }

    #endregion
}