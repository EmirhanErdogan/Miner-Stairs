using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateSlotComponent : MonoBehaviour
{
    #region Serializable Fields

    [SerializeField] private Transform Root;
    [SerializeField] private MeshRenderer MeshRenderer;

    [SerializeField] private bool IsEmpty;

    #endregion

    #region Getters

    public bool GetIsEmpty()
    {
        return IsEmpty;
    }

    public Transform GetRoot()
    {
        return Root;
    }

    public MeshRenderer GetMesh()
    {
        return MeshRenderer;
    }

    #endregion

    #region Setters

    public void SetIsEmpty(bool Value)
    {
        IsEmpty = Value;
    }

    #endregion
}