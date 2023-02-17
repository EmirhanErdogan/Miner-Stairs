using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Emir;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[Serializable]
public struct ExistingPickaxeData
{
    public int Id;
}

public class PickaxeComponent : MonoBehaviour
{
    #region Serializable Fields

    [SerializeField] private EGemType type;
    [SerializeField] private List<Transform> TargetTransforms = new List<Transform>();
    [SerializeField] private List<CubeComponent> TargetCubes = new List<CubeComponent>();
    [SerializeField] private int Index;
    [SerializeField] private int IndexRef;
    [SerializeField] private int DestroyableCubeCount;
    [SerializeField] private int DestroyableCubeCountRef;
    [SerializeField] private GameObject Mesh;
    [SerializeField] private GameObject LastMesh;
    [SerializeField] private ParticleSystem Particle;
    [SerializeField] private ParticleSystem LevelUpParticle;
    [SerializeField] private int DestroyCount = 0;
    [SerializeField] private int Damage;

    #endregion

    #region Private Fields

    private bool IsActive = false;
    private bool IsMove = false;
    private Transform TargetTransform = null;
    private LineComponent TargetLines;
    private StairComponent Stair;
    public int TargetPickaxesLevel = -1;
    private bool IsUpgrade = false;
    private int damageRef;
    private CancellationTokenSource _cancellationTokenSource;
    private CubeComponent LastCube;
    private List<CubeComponent> DestroyedCubes = new List<CubeComponent>();

    #endregion


    #region Getters

    public int GetDamage()
    {
        return Damage;
    }

    public EGemType GetEGemType()
    {
        return type;
    }

    public int GetCubeCount()
    {
        return DestroyableCubeCount;
    }

    public int GetIndex()
    {
        return IndexRef;
    }

    public GameObject GetMesh()
    {
        return Mesh;
    }


    public ParticleSystem GetParticle()
    {
        return Particle;
    }

    public ParticleSystem GetLevelUpParticle()
    {
        return LevelUpParticle;
    }

    #endregion

    private void OnEnable()
    {
        DOVirtual.DelayedCall(0.25f, () =>
        {
            IndexRef = Index;
            damageRef = Damage;
            _cancellationTokenSource = new CancellationTokenSource();
            DestroyableCubeCountRef = DestroyableCubeCount;
            transform.position = StairComponent.Instance.GetSpawnRoot().position;
            Stair = StairComponent.Instance;
            TargetLines = Stair.GetLines().FirstOrDefault(x => x.GetIsEmpty() == false);
            TargetLines.SetIsEmpty(true);
            if (TargetLines == null)
            {
                return;
            }
            else
            {
                TargetLines.SetIsEmpty(true);
                ResetTargetCubes();
            }

            TargetTransforms = TargetLines.GetSlots();
            _ = Jump();
        });
    }

    private void OnDisable()
    {
        CancelToken();
    }

    private LineComponent GetLines()
    {
        return TargetLines;
    }

    public void Killlll()
    {
        GetLines().SetIsEmpty(false);
        TargetLines = null;
        DOTween.Kill(this, false);
    }

    public void ResetTargetCubes()
    {
        int Index = TargetLines.GetOrder();
        TargetCubes.Clear();
        foreach (LayerComponent layer in Stair.GetLayers())
        {
            if (layer.GetIsActive() == false || layer.GetCubes().Count < 1)
            {
                continue;
            }

            TargetCubes.Add(layer.GetCubes()[Index]);
        }
    }

    private async UniTask Jump()
    {
        Transform TargetTransform = TargetTransforms[0];
        int Counter = 0;
        while (true)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(0.76f), cancellationToken: _cancellationTokenSource.Token);
            this.TargetTransform = TargetTransforms[Counter];
            TargetTransform = TargetTransforms[Counter];
            transform.DORotate(Vector3.right * -360, 0.75f, RotateMode.FastBeyond360).SetEase(Ease.OutSine);
            transform.DOJump(TargetTransform.position, 3.5f, 1, .75f).SetEase(Ease.OutSine)
                .OnStart(() => { IsMove = true; })
                .OnUpdate(() => { })
                .OnComplete(() => { IsMove = false; });
            Counter++;
            if (Counter >= TargetTransforms.Count)
            {
                Counter = 0;
                _ = JumpCube();
                break;
            }
        }
    }

    private async UniTask JumpCube()
    {
        Transform TargetTransform = TargetCubes[0].GetRoot();
        int Counter = 0;
        DestroyedCubes.Clear();
        while (true)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(0.76f), cancellationToken: _cancellationTokenSource.Token);
            if (Counter >= TargetCubes.Count || Counter >= this.DestroyableCubeCountRef)
            {
                Counter = 0;
                _ = InitCubes(DestroyedCubes);

                _ = JumpTube();
                break;
            }

            this.TargetTransform = TargetCubes[Counter].GetRoot();
            TargetTransform = TargetCubes[Counter].GetRoot();
            transform.DORotate(Vector3.right * -360, 0.75f, RotateMode.FastBeyond360).SetEase(Ease.InOutSine);
            transform.DOJump(TargetTransform.position, 5f, 1, .75f).SetEase(Ease.InOutSine)
                .OnStart(() =>
                {
                    IsMove = true;
                    LastCube = TargetCubes[Counter];
                })
                .OnUpdate(() => { })
                .OnComplete(() =>
                {
                    IsMove = false;
                    Counter++;

                    if (TargetCubes[Counter - 1].GetHealth() >= 1)
                    {
                        Counter--;
                        TargetCubes[Counter].DecreaseHealth(Damage);
                        TargetCubes[Counter].transform.DOShakeRotation(0.42f, 9f, 1, 9f);
                        TargetCubes[Counter].GetParticle().Play();
                    }

                    if (TargetCubes[Counter].GetHealth() <= 0)
                    {
                        TargetCubes[Counter].GetParticle().Play();
                        _ = TargetCubes[Counter].DestroyCube();
                        DestroyedCubes.Add(TargetCubes[Counter]);
                        Counter++;
                    }
                });
        }
    }

    public async UniTask InitCubes(List<CubeComponent> DestroyedCubes)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
        foreach (var Cubes in DestroyedCubes)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(0.15f));
            _ = Cubes.InitCube();
        }
    }

    public List<CubeComponent> GetCubesList()
    {
        return DestroyedCubes;
    }

    private async UniTask JumpTube()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(0.16f), cancellationToken: _cancellationTokenSource.Token);
        int xIndex = TargetLines.GetOrder();
        Transform targetRoot = Stair.GetTube().GetRoots()[xIndex];
        IndexRef = Index;
        LastCube = null;
        transform.DOJump(targetRoot.position, 5f, 1, 0.75f).OnComplete(() =>
        {
            Destroy(LastMesh);
            GetMesh().SetActive(true);
            TargetLines.SetIsEmpty(false);
            Damage = damageRef;
        });
        transform.DORotate(Vector3.right * -360, 0.75f, RotateMode.FastBeyond360).SetEase(Ease.InOutSine);
        _ = MovePath();
    }

    private async UniTask MovePath()
    {
        //move path
        await UniTask.Delay(TimeSpan.FromSeconds(0.76f), cancellationToken: _cancellationTokenSource.Token);
        int Counter = 0;
        float Timer = 0f;
        while (true)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(Timer));
            float Distance =
                Vector3.Distance(transform.position, Stair.GetTube().GetPath()[Counter].transform.position);
            float Duration = Distance / 16;
            Timer = Duration - 0.01f;
            transform.DOMove(Stair.GetTube().GetPath()[Counter].transform.position, Duration).SetEase(Ease.OutSine);

            Counter++;
            if (Counter >= Stair.GetTube().GetPath().Count)
            {
                Counter = 0;
                jumpRandomLayer(Duration);
                break;
            }
        }
    }

    private async UniTask jumpRandomLayer(float Delay)
    {
        //JumpRandomLayer
        await UniTask.Delay(TimeSpan.FromSeconds(Delay), cancellationToken: _cancellationTokenSource.Token);
        DestroyableCubeCountRef = DestroyableCubeCount;

        while (true)
        {
            await UniTask.WaitForEndOfFrame();
            int RandomIndex = Random.Range(0, 5);
            TargetLines = Stair.GetLines().FirstOrDefault(x => !x.GetIsEmpty() && x.GetOrder() == RandomIndex);
            if (TargetLines != null)
            {
                TargetLines.SetIsEmpty(true);
                ResetTargetCubes();

                break;
            }
        }


        TargetTransforms = TargetLines.GetSlots();
        _ = Jump();
    }

    public void UpgradePickaxes(GateComponent Gate)
    {
        DestroyableCubeCountRef += 2;
        GetMesh().SetActive(false);
        if (LastMesh != null)
        {
            Destroy(LastMesh);
        }

        Particle.Play();
        IndexRef += Gate.Multiplier - 1;
        var targetMesh = GameManager.Instance.GetGameSettings().PickaxesPrefab
            .FirstOrDefault(x => x.GetIndex() == IndexRef).GetMesh();

        var targetDamage = GameManager.Instance.GetGameSettings().PickaxesPrefab
            .FirstOrDefault(x => x.GetIndex() == IndexRef).GetDamage();
        Damage = targetDamage;
        var pickaxes = Instantiate(targetMesh, this.transform);
        LastMesh = pickaxes;
        var t = GetMesh().transform;
        pickaxes.transform.SetPositionAndRotation(t.position, t.rotation);
    }

    public CubeComponent GetLastCube()
    {
        return LastCube;
    }

    public void CancelToken()
    {
        _cancellationTokenSource.Cancel();
    }
}