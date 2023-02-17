using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestComponent : MonoBehaviour
{
    #region SerializableFields

    [SerializeField] private Transform ChestTransform;
    [SerializeField] private Transform ChestUpperTransform;

    #endregion

    #region Getters

    public Transform GetUpperChest()
    {
        return ChestUpperTransform;
    }

    public Transform GetChest()
    {
        return ChestTransform;
    }

    #endregion
}