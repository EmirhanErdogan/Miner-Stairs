using System.Collections;
using System.Collections.Generic;
using Emir;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class TutorialComponent : Singleton<TutorialComponent>
{
    #region Serializable Fields

    [SerializeField] private CanvasGroup Group;
    [SerializeField] private VideoPlayer Video;
    [SerializeField] private GameObject RawImages;

    #endregion


    #region Getters

    public CanvasGroup GetGroup()
    {
        return Group;
    }

    public void OpenPanel()
    {
        GameUtils.SwitchCanvasGroup(null, Group);
    }

    public void ClosePanel()
    {
        GameUtils.SwitchCanvasGroup(Group, null);
    }

    public VideoPlayer GetVideo()
    {
        return Video;
    }

    public GameObject GetRawImages()
    {
        return RawImages;
    }

    #endregion
}