using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Emir;
using UnityEngine;

public class StairComponent : Singleton<StairComponent>
{
    #region Serializable Fields

    [SerializeField] private List<LineComponent> Lines = new List<LineComponent>();
    [SerializeField] private List<LayerComponent> Layers = new List<LayerComponent>();
    [SerializeField] private List<PickaxeComponent> Pickaxes = new List<PickaxeComponent>();
    [SerializeField] private List<GateSlotComponent> GateSlots = new List<GateSlotComponent>();
    [SerializeField] private TubeComponent Tube;
    [SerializeField] private Transform SpawnRoot;

    #endregion

    private void Start()
    {
        Debug.Log(PlayerPrefs.GetInt(CommonTypes.LAYER_COUNT));
        for (int i = 1; i < PlayerPrefs.GetInt(CommonTypes.LAYER_COUNT); i++)
        {
            Debug.Log(PlayerPrefs.GetInt(CommonTypes.LAYER_COUNT));
            StairComponent.Instance.GetLayers()[i].gameObject.SetActive(true);
            StairComponent.Instance.GetLayers()[i].SetIsActive(true);
        }

        for (int i = 0; i < Pickaxes.Count; i++)
        {
            Destroy(Pickaxes[i]);
        }

        Pickaxes.Clear();
        UpgradeComponent.Instance.LoadPickAxeList();
      
        for (int i = 0; i < GameManager.Instance.GetGameSettings().Pickaxes.Count; i++)
        {
            PickaxeComponent Pickaxe = Instantiate(GameManager.Instance.GetGameSettings().Pickaxes[i]);
            Pickaxe.transform.position = SpawnRoot.position;
            Pickaxes.Add(Pickaxe);
        }
        if (GameManager.Instance.GetGameSettings().Pickaxes.Count == 0)
        {
            PickaxeComponent Pickaxe = Instantiate(GameManager.Instance.GetGameSettings().PickaxesPrefab[0]);
            Pickaxe.transform.position = SpawnRoot.position;
            Pickaxes.Add(Pickaxe);
            GameManager.Instance.GetGameSettings().Pickaxes.Add(Pickaxe);
        }

        StairComponent.Instance.GetTube().ResetTransform();
        CreateGate();
    }

    private void CreateGate()
    {
        for (int i = 0; i < PlayerPrefs.GetInt(CommonTypes.GATE_2X); i++)
        {
            GateInitialize(2);
        }

        for (int i = 0; i < PlayerPrefs.GetInt(CommonTypes.GATE_3X); i++)
        {
            GateInitialize(3);
        }

        for (int i = 0; i < PlayerPrefs.GetInt(CommonTypes.GATE_4X); i++)
        {
            GateInitialize(4);
        }
    }

    private void GateInitialize(int Multiplier)
    {
        GateSlotComponent TargetSlots =
            StairComponent.Instance.GetGateSlots().FirstOrDefault(x => x.GetIsEmpty() == false);
        TargetSlots.SetIsEmpty(true);
        GateComponent Gate = Instantiate(GameManager.Instance.GetGameSettings().GatePrefab);
        Gate.SetLastSlot(TargetSlots);
        Gate.SetLastGateSlot(TargetSlots);
        Gate.transform.position = TargetSlots.GetRoot().transform.position;
        Gate.Multiplier = Multiplier;
        Gate.ResetText();
        Gate.ResetColor();
    }

    #region Getters

    public List<GateSlotComponent> GetGateSlots()
    {
        return GateSlots;
    }

    public Transform GetSpawnRoot()
    {
        return SpawnRoot;
    }

    public List<LineComponent> GetLines()
    {
        return Lines;
    }

    public List<LayerComponent> GetLayers()
    {
        return Layers;
    }

    public TubeComponent GetTube()
    {
        return Tube;
    }

    public List<PickaxeComponent> GetPickaxes()
    {
        return Pickaxes;
    }

    #endregion
}