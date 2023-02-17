using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class LayerComponent : MonoBehaviour
{
    #region Serializable Fields

    [SerializeField] private List<CubeComponent> Cubes = new List<CubeComponent>();
    [SerializeField] private bool IsActive = false;

    #endregion

    #region Private Fields

    #endregion


    private void OnEnable()
    {
        _ = Initialize();
    }

    public async UniTask Initialize()
    {
        List<Vector3> CubeTransform = new List<Vector3>();
        foreach (var Cube in Cubes)
        {
            CubeTransform.Add(Cube.transform.position);
            Cube.transform.localPosition = Vector3.down * 100;
        }

        int counter = 0;
        while (true)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(.25f));

            Cubes[counter].transform.DOMove(CubeTransform[counter], 1f);
            counter++;
            if (counter >= Cubes.Count)
            {
                break;
            }
        }
    }

    #region Getters

    public List<CubeComponent> GetCubes()
    {
        return Cubes;
    }

    public bool GetIsActive()
    {
        return IsActive;
    }

    #endregion

    #region Setters

    public void SetIsActive(bool Value)
    {
        IsActive = Value;
    }


    #endregion
}