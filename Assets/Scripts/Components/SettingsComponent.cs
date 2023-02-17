using System.Collections;
using System.Collections.Generic;
using Emir;
using Lofelt.NiceVibrations;
using UnityEngine;
using UnityEngine.UI;

public class SettingsComponent : MonoBehaviour
{
    #region Serializable Fields

    [Header("Images")] [SerializeField] private Image m_soundOn;
    [SerializeField] private Image m_soundOff;
    [SerializeField] private Image m_vibrateOn;
    [SerializeField] private Image m_vibrateOff;

    #endregion

    #region Private Fields

    private bool IsSound = true;
    private bool IsHaptic = true;

    #endregion

    private void Start()
    {
        ChangeValues();
    }

    public void SoundButtonClick()
    {
        IsSound = !IsSound;
        //icon düzeltme
        if (IsSound is true)
        {
            m_soundOn.enabled = true;
            m_soundOff.enabled = false;
        }
        else if (IsSound is false)
        {
            m_soundOn.enabled = false;
            m_soundOff.enabled = true;
        }

        ChangeValues();
    }

    public void VibrationButtonClick()
    {
        IsHaptic = !IsHaptic;
        //icon düzeltme
        if (IsHaptic is true)
        {
            m_vibrateOn.enabled = true;
            m_vibrateOff.enabled = false;
        }
        else if (IsHaptic is false)
        {
            m_vibrateOn.enabled = false;
            m_vibrateOff.enabled = true;
        }

        ChangeValues();
    }

    public void ChangeValues()
    {
        SoundManager.Instance.SetMuteState(IsSound);
        HapticManager.Instance.ChangeHapticState(IsHaptic);
        SoundManager.Instance.Play("CLICK");
        HapticManager.Instance.PlayHaptic(HapticPatterns.PresetType.MediumImpact);
    }
}