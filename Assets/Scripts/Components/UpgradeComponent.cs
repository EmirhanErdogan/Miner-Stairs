using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Emir;
using Lofelt.NiceVibrations;
using Newtonsoft.Json;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeComponent : Emir.Singleton<UpgradeComponent>
{
    #region Serializable Fields

    [Header("Buttons")] [SerializeField] private Image AddButton;
    [SerializeField] private Image AddGateButton;
    [SerializeField] private Image MergeButton;
    [SerializeField] private Image AddLayerButton;
    [SerializeField] private GameObject NextLevelButton;

    [Header("Texts")] [SerializeField] private TextMeshProUGUI AddButtonText;
    [SerializeField] private TextMeshProUGUI AddGateButtonText;
    [SerializeField] private TextMeshProUGUI MergeButtonText;
    [SerializeField] private TextMeshProUGUI AddLayerButtonText;

    [Header("Values")] [SerializeField] private int AddAxeValue;
    [SerializeField] private int AddGateValue;
    [SerializeField] private int AddLayerValue;
    [SerializeField] private int MergeAxeValue;

    #endregion

    private int PickaxeCount = 1;

    #region Click

    private void Start()
    {
        DOVirtual.DelayedCall(0.07f, () =>
        {
            AddButtonText.text = PlayerPrefs.GetInt(CommonTypes.ADD_AXE_PRICE).ConvertMoney();
            AddGateButtonText.text = PlayerPrefs.GetInt(CommonTypes.ADD_GATE_PRICE).ConvertMoney();
            AddLayerButtonText.text = PlayerPrefs.GetInt(CommonTypes.ADD_LAYER_PRICE).ConvertMoney();
            MergeButtonText.text = PlayerPrefs.GetInt(CommonTypes.MERGE_AXE_PRICE).ConvertMoney();
        });


        DOVirtual.DelayedCall(0.95f, () =>
        {
            int StoneCounter = 0;
            int SilverCounter = 0;
            int GoldCounter = 0;
            for (int i = 0; i < StairComponent.Instance.GetPickaxes().Count; i++)
            {
                if (StairComponent.Instance.GetPickaxes()[i].GetEGemType() == EGemType.STONE)
                {
                    StoneCounter++;
                }
                else if (StairComponent.Instance.GetPickaxes()[i].GetEGemType() == EGemType.SILVER)
                {
                    SilverCounter++;
                }
                else if (StairComponent.Instance.GetPickaxes()[i].GetEGemType() == EGemType.GOLD)
                {
                    GoldCounter++;
                }
            }

            if (StoneCounter < 3 && SilverCounter < 3 && GoldCounter < 3)
            {
                //deaktif olacak
                MergeButton.color = GameManager.Instance.GetGameSettings().DeactiveButtonColor;
            }
            else
            {
                MergeButton.color = GameManager.Instance.GetGameSettings().DefaultButtonColor;
                //aktif olacak
            }
        });
    }

    public void AddButtonClick()
    {
        if (GameManager.Instance.GetCurreny() >= AddAxeValue)
        {
            if (PickaxeCount >= 5) return;

            SoundManager.Instance.Play("CLICK");
            HapticManager.Instance.PlayHaptic(HapticPatterns.PresetType.SoftImpact);

            Debug.Log("AddPickaxes");
            PickaxeCount++;
            GameObject Pickaxes = Instantiate(GameManager.Instance.GetGameSettings().PickaxesPrefab.First().gameObject);
            PickaxeComponent pickaxeComponent = Pickaxes.GetComponent<PickaxeComponent>();
            DOVirtual.DelayedCall(0.25f, () => { pickaxeComponent.GetParticle().Play(); });
            GameManager.Instance.GetGameSettings().Pickaxes
                .Add(GameManager.Instance.GetGameSettings().PickaxesPrefab.First());
            SavePickaxeList();
            StairComponent.Instance.GetPickaxes().Add(Pickaxes.GetComponent<PickaxeComponent>());
            GameManager.Instance.SetCurrency(-AddAxeValue);
            InterfaceManager.Instance.OnPlayerCurrencyUpdated();
            int targetprice = GetTargetPrice(GameManager.Instance.GetGameSettings().AxeAddPrice, AddAxeValue);
            AddAxeValue = targetprice;
            AddButtonText.text = targetprice.ConvertMoney();
            ButtonControl();
            PlayerPrefs.SetInt(CommonTypes.ADD_AXE_PRICE, AddAxeValue);
            PlayerPrefs.SetInt(CommonTypes.AXE_COUNT, PickaxeCount);
        }
        else
        {
        }
    }

    public void MergeClick()
    {
        _ = MergeButtonClick();
    }

    public async UniTask MergeButtonClick()
    {
        if (GameManager.Instance.GetCurreny() >= MergeAxeValue)
        {
            HapticManager.Instance.PlayHaptic(HapticPatterns.PresetType.SoftImpact);
            int StoneCounter = 0;
            int SilverCounter = 0;
            int GoldCounter = 0;
            for (int i = 0; i < StairComponent.Instance.GetPickaxes().Count; i++)
            {
                if (StairComponent.Instance.GetPickaxes()[i].GetEGemType() == EGemType.STONE)
                {
                    StoneCounter++;
                }
                else if (StairComponent.Instance.GetPickaxes()[i].GetEGemType() == EGemType.SILVER)
                {
                    SilverCounter++;
                }
                else if (StairComponent.Instance.GetPickaxes()[i].GetEGemType() == EGemType.GOLD)
                {
                    GoldCounter++;
                }
            }

            if (StoneCounter >= 3)
            {
                int counter = 0;
                List<PickaxeComponent> DeletedPick = new List<PickaxeComponent>();
                foreach (var Axe in StairComponent.Instance.GetPickaxes().ToList())
                {
                    if (Axe.GetEGemType() == EGemType.STONE)
                    {
                        Axe.InitCubes(Axe.GetCubesList());
                        counter++;
                        Axe.CancelToken();
                        Axe.transform.DOKill();
                        Axe.Killlll();
                        Axe.transform.DOJump(StairComponent.Instance.GetSpawnRoot().position, 5f, 0, 0.6f);
                        Axe.gameObject.GetComponent<PickaxeComponent>().enabled = false;
                        StairComponent.Instance.GetPickaxes().Remove(Axe);
                        DeletedPick.Add(Axe);
                        // Destroy(Axe.gameObject);

                        if (counter >= 3)
                        {
                            await UniTask.Delay(TimeSpan.FromSeconds(0.35f));
                            PickaxeComponent Axes = Instantiate(GameManager.Instance.GetGameSettings().PickaxesPrefab
                                .FirstOrDefault(x => x.GetEGemType() == EGemType.SILVER));
                            Axes.transform.position = StairComponent.Instance.GetSpawnRoot().position;
                            DOVirtual.DelayedCall(0.25f, () => { Axes.GetParticle().Play(); });
                            StairComponent.Instance.GetPickaxes().Add(Axes);
                            break;
                        }
                    }
                }

                GameManager.Instance.GetGameSettings().Pickaxes.Clear();
                DOVirtual.DelayedCall(0.76f, () =>
                {
                    foreach (var axe in DeletedPick)
                    {
                        StairComponent.Instance.GetPickaxes().Remove(axe);
                        Destroy(axe.gameObject);
                    }

                    foreach (var Pickaxe in StairComponent.Instance.GetPickaxes())
                    {
                        PickaxeComponent TargetPrefab = GameManager.Instance.GetGameSettings().PickaxesPrefab
                            .FirstOrDefault(x => x.GetEGemType() == Pickaxe.GetEGemType());
                        GameManager.Instance.GetGameSettings().Pickaxes.Add(TargetPrefab);
                    }
                });
            }
            else if (SilverCounter >= 3)
            {
                int counter = 0;
                List<PickaxeComponent> DeletedPick = new List<PickaxeComponent>();
                foreach (var Axe in StairComponent.Instance.GetPickaxes().ToList())
                {
                    if (Axe.GetEGemType() == EGemType.SILVER)
                    {
                        Axe.InitCubes(Axe.GetCubesList());
                        counter++;
                        Axe.CancelToken();
                        Axe.transform.DOKill();
                        Axe.Killlll();
                        Axe.transform.DOJump(StairComponent.Instance.GetSpawnRoot().position, 5f, 0, 0.6f);
                        Axe.gameObject.GetComponent<PickaxeComponent>().enabled = false;
                        StairComponent.Instance.GetPickaxes().Remove(Axe);
                        DeletedPick.Add(Axe);
                        // Destroy(Axe.gameObject);

                        if (counter >= 3)
                        {
                            await UniTask.Delay(TimeSpan.FromSeconds(0.35f));
                            PickaxeComponent Axes = Instantiate(GameManager.Instance.GetGameSettings().PickaxesPrefab
                                .FirstOrDefault(x => x.GetEGemType() == EGemType.GOLD));
                            Axes.transform.position = StairComponent.Instance.GetSpawnRoot().position;
                            DOVirtual.DelayedCall(0.25f, () => { Axes.GetParticle().Play(); });
                            StairComponent.Instance.GetPickaxes().Add(Axes);
                            break;
                        }
                    }
                }

                GameManager.Instance.GetGameSettings().Pickaxes.Clear();
                DOVirtual.DelayedCall(0.76f, () =>
                {
                    foreach (var axe in DeletedPick)
                    {
                        StairComponent.Instance.GetPickaxes().Remove(axe);
                        Destroy(axe.gameObject);
                    }

                    foreach (var Pickaxe in StairComponent.Instance.GetPickaxes())
                    {
                        PickaxeComponent TargetPrefab = GameManager.Instance.GetGameSettings().PickaxesPrefab
                            .FirstOrDefault(x => x.GetEGemType() == Pickaxe.GetEGemType());
                        GameManager.Instance.GetGameSettings().Pickaxes.Add(TargetPrefab);
                    }
                });
            }
            else if (GoldCounter >= 3)
            {
                int counter = 0;
                List<PickaxeComponent> DeletedPick = new List<PickaxeComponent>();
                foreach (var Axe in StairComponent.Instance.GetPickaxes().ToList())
                {
                    if (Axe.GetEGemType() == EGemType.GOLD)
                    {
                        Axe.InitCubes(Axe.GetCubesList());
                        counter++;
                        Axe.CancelToken();
                        Axe.transform.DOKill();
                        Axe.Killlll();
                        Axe.transform.DOJump(StairComponent.Instance.GetSpawnRoot().position, 5f, 0, 0.6f);
                        Axe.gameObject.GetComponent<PickaxeComponent>().enabled = false;
                        StairComponent.Instance.GetPickaxes().Remove(Axe);
                        DeletedPick.Add(Axe);
                        // Destroy(Axe.gameObject);

                        if (counter >= 3)
                        {
                            await UniTask.Delay(TimeSpan.FromSeconds(0.35f));
                            PickaxeComponent Axes = Instantiate(GameManager.Instance.GetGameSettings().PickaxesPrefab
                                .FirstOrDefault(x => x.GetEGemType() == EGemType.DIAMOND));
                            Axes.transform.position = StairComponent.Instance.GetSpawnRoot().position;
                            DOVirtual.DelayedCall(0.25f, () => { Axes.GetParticle().Play(); });
                            StairComponent.Instance.GetPickaxes().Add(Axes);
                            break;
                        }
                    }
                }

                GameManager.Instance.GetGameSettings().Pickaxes.Clear();
                DOVirtual.DelayedCall(0.76f, () =>
                {
                    foreach (var axe in DeletedPick)
                    {
                        StairComponent.Instance.GetPickaxes().Remove(axe);
                        Destroy(axe.gameObject);
                    }

                    foreach (var Pickaxe in StairComponent.Instance.GetPickaxes())
                    {
                        PickaxeComponent TargetPrefab = GameManager.Instance.GetGameSettings().PickaxesPrefab
                            .FirstOrDefault(x => x.GetEGemType() == Pickaxe.GetEGemType());
                        GameManager.Instance.GetGameSettings().Pickaxes.Add(TargetPrefab);
                    }
                });
            }
            else
            {
                return;
            }

            SoundManager.Instance.Play("CLICK");


            PickaxeCount -= 2;
            GameManager.Instance.SetCurrency(-MergeAxeValue);
            InterfaceManager.Instance.OnPlayerCurrencyUpdated();
            int targetprice = GetTargetPrice(GameManager.Instance.GetGameSettings().AxeMergePrice, MergeAxeValue);
            MergeAxeValue = targetprice;
            MergeButtonText.text = targetprice.ConvertMoney();
            ButtonControl();
            PlayerPrefs.SetInt(CommonTypes.MERGE_AXE_PRICE, MergeAxeValue);
            PlayerPrefs.SetInt(CommonTypes.AXE_COUNT, PickaxeCount);
            await UniTask.Delay(TimeSpan.FromSeconds(1f));
            SavePickaxeList();
        }
        else
        {
        }
    }

    private void SavePickaxeList()
    {
        Debug.Log($"Saving Pickaxe List");
        var myList = CreateExistingPickaxeData();
        var json = JsonConvert.SerializeObject(myList);
        PlayerPrefs.SetString(CommonTypes.STORED_PICKAXE_KEY, json);
    }

    private ExistingPickaxeData[] CreateExistingPickaxeData()
    {
        Debug.Log($"Creating Existing Pickaxe Data");

        var newStoredPickaxe = new List<ExistingPickaxeData>();
        foreach (var pickaxe in GameManager.Instance.GetGameSettings().Pickaxes)
        {
            newStoredPickaxe.Add(new ExistingPickaxeData()
            {
                Id = pickaxe.GetIndex()
            });
        }

        return newStoredPickaxe.ToArray();
    }

    public void LoadPickAxeList()
    {
        Debug.Log($"Loading Pickaxe List");
        GameManager.Instance.GetGameSettings().Pickaxes.Clear();
        var storedPickaxeJson = PlayerPrefs.GetString(CommonTypes.STORED_PICKAXE_KEY);
        var existingPickaxeDatas = string.IsNullOrEmpty(storedPickaxeJson)
            ? Array.Empty<ExistingPickaxeData>()
            : JsonConvert.DeserializeObject<ExistingPickaxeData[]>(storedPickaxeJson);

        if (existingPickaxeDatas.Length == 0) return;
        foreach (var existingPickaxeData in existingPickaxeDatas)
        {
            var pickaxeInfo = GameManager.Instance.GetGameSettings().PickaxesPrefab
                .FirstOrDefault(data => data.GetIndex() == existingPickaxeData.Id);

            GameManager.Instance.GetGameSettings().Pickaxes.Add(pickaxeInfo);
        }
    }

    public void AddLayerClick()
    {
        if (GameManager.Instance.GetCurreny() >= AddLayerValue)
        {
            SoundManager.Instance.Play("CLICK");
            HapticManager.Instance.PlayHaptic(HapticPatterns.PresetType.SoftImpact);

            Debug.Log("AddLayer");
            LayerComponent TargetLayer =
                StairComponent.Instance.GetLayers().FirstOrDefault(x => x.GetIsActive() == false);
            if (TargetLayer is null)
            {
                //max seviyeye ulaştı
                //max yazdır text kısmına 
                AddLayerButton.color = Color.gray;
                AddLayerButton.gameObject.GetComponent<Button>().enabled = false;
                AddLayerButtonText.color = Color.red;
                AddLayerButton.gameObject.SetActive(false);
                NextLevelButton.SetActive(true);
                return;
            }

            TargetLayer.gameObject.SetActive(true);
            TargetLayer.SetIsActive(true);
            CameraManager.Instance.GetVirtualCamera().m_Lens.FieldOfView += 1;
            StairComponent.Instance.GetTube().ResetTransform();
            GameManager.Instance.GetGameSettings().ActiveLayerCount++;
            foreach (PickaxeComponent pickaxe in StairComponent.Instance.GetPickaxes())
            {
                pickaxe.ResetTargetCubes();
            }

            GameManager.Instance.SetCurrency(-AddLayerValue);
            InterfaceManager.Instance.OnPlayerCurrencyUpdated();
            int targetprice = GetTargetPrice(GameManager.Instance.GetGameSettings().LayerAddPrice, AddLayerValue);
            AddLayerValue = targetprice;
            AddLayerButtonText.text = targetprice.ConvertMoney();
            PlayerPrefs.SetInt(CommonTypes.LAYER_COUNT, PlayerPrefs.GetInt(CommonTypes.LAYER_COUNT) + 1);
            ButtonControl();
            PlayerPrefs.SetInt(CommonTypes.ADD_LAYER_PRICE, AddLayerValue);
        }
        else
        {
        }
    }

    public void AddGateClick()
    {
        if (GameManager.Instance.GetCurreny() >= AddGateValue)
        {
            SoundManager.Instance.Play("CLICK");
            HapticManager.Instance.PlayHaptic(HapticPatterns.PresetType.SoftImpact);
            Debug.Log("AddGate");

            GateSlotComponent TargetSlots =
                StairComponent.Instance.GetGateSlots().FirstOrDefault(x => x.GetIsEmpty() == false);
            TargetSlots.SetIsEmpty(true);
            GateComponent Gate = Instantiate(GameManager.Instance.GetGameSettings().GatePrefab);
            Gate.SetLastSlot(TargetSlots);
            Gate.SetLastGateSlot(TargetSlots);
            Gate.transform.position = TargetSlots.GetRoot().transform.position;
            GameManager.Instance.SetCurrency(-AddGateValue);
            InterfaceManager.Instance.OnPlayerCurrencyUpdated();
            int targetprice = GetTargetPrice(GameManager.Instance.GetGameSettings().GateAddPrice, AddGateValue);
            AddGateValue = targetprice;
            AddGateButtonText.text = targetprice.ConvertMoney();
            ButtonControl();
            PlayerPrefs.SetInt(CommonTypes.ADD_GATE_PRICE, AddGateValue);
            PlayerPrefs.SetInt(CommonTypes.GATE_2X, PlayerPrefs.GetInt(CommonTypes.GATE_2X) + 1);
            if (PlayerPrefs.GetInt(CommonTypes.GATE_2X) == 4 && PlayerPrefs.GetInt(CommonTypes.ISTUTORIAL) == 0)
            {
                PlayerPrefs.SetInt(CommonTypes.ISTUTORIAL, 1);
                TutorialComponent.Instance.OpenPanel();
                TutorialComponent.Instance.GetVideo().Play();
                TutorialComponent.Instance.GetVideo().DOPlayForward();
            }
        }
        else
        {
        }
    }

    private int GetTargetPrice(List<int> TargetList, int price)
    {
        int Index = TargetList.IndexOf(price);
        if (Index == TargetList.Count - 1)
        {
            //son elemansa
            return TargetList.Last() * 2;
        }
        else
        {
            //son eleman değilse 
            return TargetList[Index + 1];
        }
    }


    private void OnEnable()
    {
        DOVirtual.DelayedCall(0.05f, () =>
        {
            if (PlayerPrefs.GetInt("Game") == 0)
            {
                //ilk açılma
                PlayerPrefs.SetInt(CommonTypes.ADD_AXE_PRICE,
                    GameManager.Instance.GetGameSettings().AxeAddPrice.First());
                PlayerPrefs.SetInt(CommonTypes.ADD_LAYER_PRICE,
                    GameManager.Instance.GetGameSettings().LayerAddPrice.First());
                PlayerPrefs.SetInt(CommonTypes.ADD_GATE_PRICE,
                    GameManager.Instance.GetGameSettings().GateAddPrice.First());
                PlayerPrefs.SetInt(CommonTypes.MERGE_AXE_PRICE,
                    GameManager.Instance.GetGameSettings().AxeMergePrice.First());
                PlayerPrefs.SetInt(CommonTypes.AXE_COUNT, 1);
            }
            else
            {
                //ikinci açılma
            }

            AddAxeValue = PlayerPrefs.GetInt(CommonTypes.ADD_AXE_PRICE);
            AddLayerValue = PlayerPrefs.GetInt(CommonTypes.ADD_LAYER_PRICE);
            AddGateValue = PlayerPrefs.GetInt(CommonTypes.ADD_GATE_PRICE);
            MergeAxeValue = PlayerPrefs.GetInt(CommonTypes.MERGE_AXE_PRICE);
            PickaxeCount = PlayerPrefs.GetInt(CommonTypes.AXE_COUNT);
        });
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt(CommonTypes.ADD_AXE_PRICE, AddAxeValue);
        PlayerPrefs.SetInt(CommonTypes.ADD_LAYER_PRICE, AddLayerValue);
        PlayerPrefs.SetInt(CommonTypes.ADD_GATE_PRICE, AddGateValue);
        PlayerPrefs.SetInt(CommonTypes.MERGE_AXE_PRICE, MergeAxeValue);
        PlayerPrefs.SetInt(CommonTypes.AXE_COUNT, PickaxeCount);
    }

    public void ButtonControl()
    {
        if (GameManager.Instance.GetCurreny() >= AddAxeValue)
        {
            AddButton.color = GameManager.Instance.GetGameSettings().DefaultButtonColor;
        }
        else
        {
            AddButton.color = GameManager.Instance.GetGameSettings().DeactiveButtonColor;
        }

        if (GameManager.Instance.GetCurreny() >= AddGateValue)
        {
            AddGateButton.color = GameManager.Instance.GetGameSettings().DefaultButtonColor;
        }
        else
        {
            AddGateButton.color = GameManager.Instance.GetGameSettings().DeactiveButtonColor;
        }

        if (GameManager.Instance.GetCurreny() >= AddLayerValue)
        {
            AddLayerButton.color = GameManager.Instance.GetGameSettings().DefaultButtonColor;
        }
        else
        {
            AddLayerButton.color = GameManager.Instance.GetGameSettings().DeactiveButtonColor;
        }


        if (PickaxeCount >= 5)
        {
            AddButton.color = GameManager.Instance.GetGameSettings().DeactiveButtonColor;
            AddButtonText.text = "MAX";
        }
        else
        {
            if (GameManager.Instance.GetCurreny() >= AddAxeValue)
            {
                AddButton.color = GameManager.Instance.GetGameSettings().DefaultButtonColor;
                AddButtonText.text = AddAxeValue.ConvertMoney();
            }
            else
            {
                AddButton.color = GameManager.Instance.GetGameSettings().DeactiveButtonColor;
                AddButtonText.text = AddAxeValue.ConvertMoney();
            }
        }

        DOVirtual.DelayedCall(0.35f, () =>
        {
            int StoneCounter = 0;
            int SilverCounter = 0;
            int GoldCounter = 0;
            for (int i = 0; i < StairComponent.Instance.GetPickaxes().Count; i++)
            {
                if (StairComponent.Instance.GetPickaxes()[i].GetEGemType() == EGemType.STONE)
                {
                    StoneCounter++;
                }
                else if (StairComponent.Instance.GetPickaxes()[i].GetEGemType() == EGemType.SILVER)
                {
                    SilverCounter++;
                }
                else if (StairComponent.Instance.GetPickaxes()[i].GetEGemType() == EGemType.GOLD)
                {
                    GoldCounter++;
                }
            }

            if (StoneCounter < 3 && SilverCounter < 3 && GoldCounter < 3)
            {
                //deaktif olacak
                MergeButton.color = GameManager.Instance.GetGameSettings().DeactiveButtonColor;
            }
            else
            {
                if (GameManager.Instance.GetCurreny() >= MergeAxeValue)
                {
                    MergeButton.color = GameManager.Instance.GetGameSettings().DefaultButtonColor;
                }
                else
                {
                    MergeButton.color = GameManager.Instance.GetGameSettings().DeactiveButtonColor;
                }
                //aktif olacak
            }
        });
    }

    #endregion
}