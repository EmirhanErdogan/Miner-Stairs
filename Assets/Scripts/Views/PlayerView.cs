using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Emir;
using Random = UnityEngine.Random;

public class PlayerView : MonoBehaviour
{
    #region Serializable Fields

    [SerializeField] private Transform CameraRoot;
    [SerializeField] private float CameraRotateSpeed;
    [SerializeField] private GateComponent TargetGate;
    [SerializeField] private float speedx;
    [SerializeField] private float speedz;
    [SerializeField] private float ChestInstantiateDuration;
    [SerializeField] private float ChestTimer;
    [SerializeField] private bool IsActiveChest;

    #endregion

    #region Private Fields

    private float CameraMinY = 60;
    private float CameraMaxY = -40;
    private Vector3 LastPos;
    private Vector3 LastPosGate;
    private Ray ray;
    private RaycastHit hit;
    private bool IsNotRotate = false;
    private Camera _cam;

    #endregion

    private void Start()
    {
        ChestTimer = Time.time + ChestInstantiateDuration;
    }

    private void Awake()
    {
        _cam = Camera.main;
    }

    private void Update()
    {
        var isDraggingGate = CheckGateInput();
        if (!isDraggingGate) CheckCameraRotate();
        CreateChest();
    }

    private void CheckCameraRotate()
    {
        if (TargetGate is not null) return;
        // if (IsNotRotate is true) return;

        if (TouchManager.Instance.IsTouching())
        {
            var deltaPos = Input.mousePosition.x - LastPos.x;
            if (LastPos == Vector3.zero) deltaPos = 0;
            LastPos = Input.mousePosition;
            if (deltaPos is < 0.5f and > -0.5f) return;
            if (LastPos == Vector3.zero) return;

            CameraRoot.Rotate(Vector3.up, deltaPos * 5 * Time.deltaTime);
            var rotation = CameraRoot.rotation;
            var eulerAngles = rotation.eulerAngles;
            rotation = Quaternion.Euler(eulerAngles.x, ClampAngle(eulerAngles.y, -40, 60),
                eulerAngles.z);
            CameraRoot.rotation = rotation;
        }
        else if (TouchManager.Instance.IsTouchUp())
        {
            LastPos = Vector3.zero;
        }
    }

    static float ClampAngle(float angle, float from, float to)
    {
        // accepts e.g. -80, 80
        if (angle < 0f) angle = 360 + angle;
        return angle > 180f ? Mathf.Max(angle, 360 + from) : Mathf.Min(angle, to);
    }

    private bool CheckGateInput()
    {
        if (TouchManager.Instance.IsTouchDown())
        {
            TargetGate = null;

            ray = _cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if (hit.collider.gameObject.CompareTag(CommonTypes.GATE_TAG) ||
                    hit.collider.gameObject.CompareTag(CommonTypes.STAIR_COMPONENT) ||
                    hit.collider.gameObject.CompareTag(CommonTypes.GATE_SLOT_TAG))
                {
                    IsNotRotate = true;
                }
                else
                {
                    IsNotRotate = false;
                }

                if (hit.collider.gameObject.CompareTag(CommonTypes.GATE_TAG))
                {
                    TargetGate = hit.collider.gameObject.GetComponent<GateComponent>();
                    TargetGate.TransformUp();
                    LastPosGate = Input.mousePosition;
                }
            }
            else
            {
                IsNotRotate = false;
            }
        }

        if (TargetGate is null) return false;

        else if (TouchManager.Instance.IsTouching())
        {
            Vector3 DeltaPos = Input.mousePosition - LastPosGate;
            DeltaPos = new Vector3(DeltaPos.x, 0, DeltaPos.y);
            if (DeltaPos.x < 0.2f && DeltaPos.x > -0.2f)
            {
                DeltaPos.x = 0;
            }
            else if (DeltaPos.x < -0.2f)
            {
                DeltaPos.x = -1;
            }
            else
            {
                DeltaPos.x = 1;
            }

            if (DeltaPos.z < 0.2f && DeltaPos.z > -0.2f)
            {
                DeltaPos.z = 0;
            }
            else if (DeltaPos.z < -0.2f)
            {
                DeltaPos.z = -1;
            }
            else
            {
                DeltaPos.z = 1;
            }

            TargetGate.transform.position =
                new Vector3(
                    TargetGate.transform.position.x + DeltaPos.x * (Time.fixedDeltaTime * speedx),
                    TargetGate.transform.position.y,
                    TargetGate.transform.position.z);
            //preview kontrol et
            TargetGate.GroundControl();
            LastPosGate = Input.mousePosition;
        }
        else if (TouchManager.Instance.IsTouchUp())
        {
            if (TargetGate.GetTargetSlot() is null)
            {
                //kendi slotuna 
                TargetGate.TargetTransformMove(TargetGate.GetLastSlot().transform.position);
            }
            else if (TargetGate.GetTargetSlot() is not null)
            {
                //başka slota
                Vector3 LastPos = TargetGate.GetLastSlot().transform.position;
                TargetGate.TargetTransformMove(TargetGate.GetTargetSlot().transform.position);
                TargetGate.GetLastSlot().SetIsEmpty(false);
                TargetGate.SetLastSlot(TargetGate.GetTargetSlot());
                TargetGate.GetLastSlot().SetIsEmpty(true);
                TargetGate.SetTargetSlot(null);

                if (TargetGate.GetMergeGate() != null)
                {
                    if (TargetGate.Multiplier >= 4)
                    {
                        TargetGate.TargetTransformMove(LastPos);
                        Debug.Log("küfür yoook");
                    }
                    else if (TargetGate.Multiplier < 4)
                    {
                        _ = TargetGate.MergeGate();
                    }
                }
            }

            TargetGate.GetLastSlot().GetMesh().enabled = false;
            TargetGate = null;
        }

        return true;
        //dokunduysam hareket ettir
        //gate'in altından ray at yere yapıştır 
        // previewları aktifleştir
        //
    }

    private void CreateChest()
    {
        if (Time.time > ChestTimer)
        {
            if (IsActiveChest is true)
            {
                return;
            }

            Debug.Log("chest oluşu");
            ChestComponent Chest = Instantiate(GameManager.Instance.GetGameSettings().ChestPrefab);
            int Layercount = 0;
            foreach (LayerComponent layer in StairComponent.Instance.GetLayers())
            {
                if (layer.GetIsActive() == true)
                {
                    Layercount++;
                }
            }

            int randomlayer = Random.Range(0, Layercount);
            int randomCube = Random.Range(0, 5);
            CubeComponent TargetCube = StairComponent.Instance.GetLayers()[randomlayer].GetCubes()[randomCube];
            TargetCube.SetChestObj(Chest);
            TargetCube.SetIsChest(true);
            TargetCube.ChangeChest();
            Chest.transform.position = TargetCube.transform.position;
            IsActiveChest = true;
            ChestTimer += ChestInstantiateDuration;
        }
    }

    public bool GetIsActiveChest()
    {
        return IsActiveChest;
    }

    public void SetIsActiveChest(bool value)
    {
        IsActiveChest = value;
    }
}