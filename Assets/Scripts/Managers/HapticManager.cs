using System.Collections;
using System.Collections.Generic;
using Emir;
using Lofelt.NiceVibrations;
using UnityEngine;

public class HapticManager : Singleton<HapticManager>
{
    #region Private Fields

    private bool IsNoHaptic = true;

    #endregion


    public void PlayHaptic(HapticPatterns.PresetType Type)
    {
        if (!IsNoHaptic) return;
    
        HapticPatterns.PlayPreset(Type);
    }

    public void ChangeHapticState(bool value)
    {
        IsNoHaptic = value;
    }
}