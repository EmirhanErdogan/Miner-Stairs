using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class TubeComponent : MonoBehaviour
{
    #region Serializable Fields

    [SerializeField] private Transform TubeMain;
    [SerializeField] private Transform TubeSecond;
    [SerializeField] private List<Transform> Roots = new List<Transform>();
    [SerializeField] private List<Transform> Path = new List<Transform>();

    #endregion

    private void Start()
    {
        TubeMain.transform.position = new Vector3(-0.77f, 0, -8.5f);
    }

    public void ResetTransform()
    {
        SetTubePos();
        SetSecondTubeScale();
    }

    private void SetTubePos()
    {
        LayerComponent TargetLayer = StairComponent.Instance.GetLayers().LastOrDefault(x => x.GetIsActive() == true);
        TubeMain.DOMoveY(TargetLayer.transform.position.y, 1f);
    }

    private void SetSecondTubeScale()
    {
        LayerComponent TargetLayer = StairComponent.Instance.GetLayers().LastOrDefault(x => x.GetIsActive() == true);

        float TargetScale = 1 + (1.3f * (TargetLayer.transform.position.y / -2));
        TubeSecond.DOScaleY(TargetScale, 1f);
    }

    #region Getters

    public List<Transform> GetRoots()
    {
        return Roots;
    }

    public List<Transform> GetPath()
    {
        return Path;
    }

    #endregion
}