using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Emir;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class CubeComponent : MonoBehaviour
{
    #region Serializable Fields

    [SerializeField] private EGemType type;
    [SerializeField] private Vector2 Coordinate;
    [SerializeField] private Transform Root;
    [SerializeField] private List<Rigidbody> Rigids = new List<Rigidbody>();
    [SerializeField] private List<Transform> Transforms = new List<Transform>();
    [SerializeField] private List<Vector3> Positions = new List<Vector3>();
    [SerializeField] private List<Quaternion> Rotations = new List<Quaternion>();
    [SerializeField] private List<Collider> Colliders = new List<Collider>();
    [SerializeField] private int MoneyCount;
    [SerializeField] private bool IsChest;
    [SerializeField] private ChestComponent Chest;
    [SerializeField] private int Health;
    [SerializeField] private ParticleSystem Particle;

    #endregion

    #region Private fields

    private float ChestTimer = 0;
    private int HealthRef;

    #endregion

    private void Start()
    {
        if (IsChest is false)
        {
            CoordinateInitialize();
            Positions.Clear();
            HealthRef = Health;
        }
    }


    private void CoordinateInitialize()
    {
        Coordinate = new Vector2(transform.position.x / 2, transform.position.z / 2);
    }

    public async UniTask DestroyCube()
    {
        if (GetIsChest() == true)
        {
            OpenChest();
        }
        else if (GetIsChest() == false)
        {
            foreach (var trans in Transforms)
            {
                Vector3 pos = trans.position;
                Quaternion Rot = trans.rotation;
                Colliders.Add(trans.GetComponent<Collider>());
                Positions.Add(pos);
                Rotations.Add(Rot);
            }

            foreach (var target in Rigids)
            {
                target.isKinematic = false;
                target.AddExplosionForce(3000f, transform.forward, 10f);
            }

            DOVirtual.DelayedCall(0.15f, () =>
            {
                foreach (Collider Obj in Colliders)
                {
                    Obj.isTrigger = true;
                }
            });
            GameManager.Instance.SetCurrency(MoneyCount);
            int count = MoneyCount / 100;
            int counter = 0;
            while (true)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(0.05f));
                InterfaceManager.Instance.FlyCurrencyFromWorld(transform.position);
                counter++;
                if (counter >= count)
                {
                    break;
                }
            }

            InterfaceManager.Instance.OnPlayerCurrencyUpdated();
            // DOVirtual.DelayedCall(1.5f, () => { _ = InitCube(); });
        }
    }

    public async UniTask InitCube()
    {
        Health = HealthRef;
        int counter = 0;
        foreach (Collider Obj in Colliders)
        {
            Obj.isTrigger = false;
        }

        while (true)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(0.075f));
            Rigids[counter].Sleep();
            Rigids[counter].velocity = Vector3.zero;
            Rigids[counter].isKinematic = true;
            Transforms[counter].DORotateQuaternion(Rotations[counter], 0.01f);
            Transforms[counter].DOMove(Positions[counter], 0.1f);
            counter++;
            if (counter >= Rigids.Count)
            {
                break;
            }
        }
    }

    private void Update()
    {
        ChestControl();
    }

    private async UniTask ChestControl()
    {
        if (IsChest == true)
        {
            ChestTimer = Time.time;
            await UniTask.Delay(100);
            if (Time.time > ChestTimer + GameManager.Instance.GetGameSettings().ChestDestroyTimer)
            {
                ChangeCube();
                Debug.Log("chest yok oldu");
            }
        }
    }

    public async UniTask OpenChest()
    {
        GameManager.Instance.SetCurrency(MoneyCount * 2);
        for (int i = 0; i < 10; i++)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(0.075f));
            InterfaceManager.Instance.FlyCurrencyFromWorld(transform.position);
        }

        InterfaceManager.Instance.OnPlayerCurrencyUpdated();
        Chest.GetUpperChest().DORotate(new Vector3(-65f, 0, 0), 0.5f).OnComplete(() =>
        {
            DOVirtual.DelayedCall(2f, () => { ChangeCube(); });
        });
    }

    public void ChangeChest()
    {
        ChestTimer = Time.time;
        foreach (Transform Meshes in Transforms)
        {
            Meshes.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    public void ChangeCube()
    {
        GameManager.Instance.GetPlayerView().SetIsActiveChest(false);
        Destroy(Chest.gameObject);
        IsChest = false;
        Health = HealthRef;
        foreach (Transform Meshes in Transforms)
        {
            Meshes.GetComponent<MeshRenderer>().enabled = true;
        }
    }

    public void SetChestObj(ChestComponent Chestt)
    {
        Chest = Chestt;
    }

    #region Getters

    public ParticleSystem GetParticle()
    {
        return Particle;
    }

    public int GetHealth()
    {
        return Health;
    }

    public void DecreaseHealth(int value)
    {
        Health -= value;
    }

    public EGemType GetEGemType()
    {
        return type;
    }

    public Transform GetRoot()
    {
        return Root;
    }

    public bool GetIsChest()
    {
        return IsChest;
    }

    public void SetIsChest(bool value)
    {
        IsChest = value;
    }

    #endregion

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Handles.Label(transform.position, $"{Coordinate.ToString()}");
    }
#endif
}