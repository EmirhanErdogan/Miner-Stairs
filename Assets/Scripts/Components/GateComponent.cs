using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Emir;
using TMPro;
using UnityEngine;

public class GateComponent : MonoBehaviour
{
    #region SerializableFields

    [SerializeField] private GateSlotComponent LastSlot;
    [SerializeField] private GateSlotComponent TargetSlot;
    [SerializeField] public int Multiplier;
    [SerializeField] private TextMeshProUGUI Text;
    [SerializeField] private ParticleSystem Particle;
    [SerializeField] private MeshRenderer Mesh;

    #endregion

    #region Private Fields

    private Ray ray;
    private RaycastHit hit;
    private GateSlotComponent LastGateSlot;
    private GateComponent MergedGate;
    private bool IsUpgrade = false;
    private Sequence transformupsequence;
    private Sequence transformMoveSequence;

    #endregion

    private void OnEnable()
    {
        ResetText();
        transformupsequence = DOTween.Sequence();
        transformMoveSequence = DOTween.Sequence();
    }

    public void ResetText()
    {
        Text.text = string.Format("X{0}", Multiplier);
    }

    public void ResetColor()
    {
        Mesh.material.color = GameManager.Instance.GetGameSettings().GateColor[Multiplier];
    }

    public void TransformUp()
    {
        transformMoveSequence.Kill();
        transformupsequence.Join(transform.DOMoveY(transform.position.y + 1f, 0.375f));
        transformupsequence.Play();
    }

    public void TargetTransformMove(Vector3 TargetPos)
    {
        print("down");
        transformupsequence.Kill();
        transformMoveSequence.Join(transform.DOMove(TargetPos, 0.376f));
        transformMoveSequence.Play();
    }

    public void GroundControl()
    {
        ray = new Ray(transform.position, Vector3.down);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            if (hit.collider.gameObject.CompareTag(CommonTypes.GATE_TAG))
            {
                var gate = hit.collider.gameObject.GetComponent<GateComponent>();
                if (Multiplier == gate.Multiplier)
                {
                    MergedGate = gate;
                    TargetSlot = MergedGate.GetLastSlot();
                    LastGateSlot.GetMesh().enabled = false;
                    LastGateSlot = gate.LastGateSlot;
                    LastGateSlot.GetMesh().enabled = true;
                }
            }
            else if (hit.collider.gameObject.CompareTag(CommonTypes.GATE_SLOT_TAG))
            {
                if (LastGateSlot is not null) LastGateSlot.GetMesh().enabled = false;
                MergedGate = null;
                LastGateSlot = StairComponent.Instance.GetGateSlots()
                    .FirstOrDefault(x => x.gameObject == hit.collider.gameObject);
                LastGateSlot.GetMesh().enabled = true;
                TargetSlot = LastGateSlot;
            }

            else
            {
                MergedGate = null;
                if (LastGateSlot is null) return;
                LastGateSlot.GetMesh().enabled = false;
                // LastGateSlot = null;
                // TargetSlot = null;
            }
        }
    }

    public async UniTask MergeGate()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(0.376f));
        if (Multiplier == 4)
        {
            //geri döndür
            return;
        }

        Destroy(MergedGate.gameObject);

        Multiplier += 1;
        Mesh.material.color = GameManager.Instance.GetGameSettings().GateColor[Multiplier];
        ResetText();
        MergedGate = null;
        Particle.Play();
        if (Multiplier == 3)
        {
            PlayerPrefs.SetInt(CommonTypes.GATE_3X, PlayerPrefs.GetInt(CommonTypes.GATE_3X) + 1);
            PlayerPrefs.SetInt(CommonTypes.GATE_2X, PlayerPrefs.GetInt(CommonTypes.GATE_2X) - 2);
            Debug.Log("level3 save");
        }
        else if (Multiplier == 4)
        {
            PlayerPrefs.SetInt(CommonTypes.GATE_4X, PlayerPrefs.GetInt(CommonTypes.GATE_4X) + 1);
            PlayerPrefs.SetInt(CommonTypes.GATE_3X, PlayerPrefs.GetInt(CommonTypes.GATE_3X) - 2);
            Debug.Log("level4 save");
        }
    }

    #region Getters

    public GateComponent GetMergeGate()
    {
        return MergedGate;
    }

    public GateSlotComponent GetLastSlot()
    {
        return LastSlot;
    }

    public GateSlotComponent GetTargetSlot()
    {
        return TargetSlot;
    }

    #endregion

    #region Setters

    public void SetLastSlot(GateSlotComponent TargetSlot)
    {
        LastSlot = TargetSlot;
    }

    public void SetLastGateSlot(GateSlotComponent TargetSlot)
    {
        LastGateSlot = TargetSlot;
    }

    public void SetTargetSlot(GateSlotComponent TargetSlot)
    {
        this.TargetSlot = TargetSlot;
    }

    #endregion

    #region Trigger

    private void IsUpgradeFalse()
    {
        DOVirtual.DelayedCall(0.75f, () => { IsUpgrade = false; });
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(CommonTypes.PICKAXES_TAG))
        {
            if (IsUpgrade is true) return;
            IsUpgrade = true;
            IsUpgradeFalse();
            other.gameObject.GetComponent<PickaxeComponent>()
                .UpgradePickaxes(this);
            other.gameObject.GetComponent<PickaxeComponent>().GetLevelUpParticle().Play();
        }
    }

    #endregion
}