using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct ResourceConfig
{
    public string Name;
    public double UnlockCost;
    public double UpgradeCost;
    public double Output;
}

public class GameManager : MonoBehaviour
{
    private static GameManager _instance = null;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<GameManager>();
            return _instance;
        }
    }

    [Range(0f, 1f)]
    [SerializeField]
    private float _autoCollectPercentage = 0.1f;
    [SerializeField]
    private ResourceConfig[] _resourcesConfigs;
    [SerializeField]
    public Sprite[] ResourcesSprites;
    [SerializeField]
    private Transform _resourcesParent;
    [SerializeField]
    private ResourceController _resourcePrefab;
    [SerializeField]
    private TapText _tapTextPrefab;
    [SerializeField]
    private RectTransform _coinIcon;


    [Header("Text Info")]
    [SerializeField]
    private Text _goldInfo;
    [SerializeField]
    private Text _autoCollectInfo;


    private List<ResourceController> _activeResources = new List<ResourceController>();
    private List<TapText> _tapTextPool = new List<TapText>();
    private float _collectSecond;
    private double _totalGold;

    public double TotalGold => _totalGold;

    void Start()
    {
        AddAllResources();
    }

    void Update()
    {
        _collectSecond += Time.unscaledDeltaTime;
        if (_collectSecond >= 1f)
        {
            CollectPerSecond();
            _collectSecond = 0f;
        }

        CheckResourceCost();

        _coinIcon.transform.localScale = Vector3.LerpUnclamped(_coinIcon.transform.localScale, Vector3.one * 1.2f, 0.15f);
        _coinIcon.transform.Rotate(0f, 0f, Time.deltaTime * -100f);
    }

    bool isBuyable = false;
    private void CheckResourceCost()
    {
        foreach (ResourceController resource in _activeResources)
        {
            if (resource.IsUnlocked) 
                isBuyable = TotalGold >= resource.GetUpgradeCost();
            else
                isBuyable = TotalGold >= resource.GetUnlockCost();

            resource.ResourceImage.sprite = ResourcesSprites[isBuyable ? 1 : 0];
        }
    }

    bool showResources = true;
    private void AddAllResources()
    {
        foreach (ResourceConfig config in _resourcesConfigs)
        {
            GameObject obj = Instantiate(_resourcePrefab.gameObject, _resourcesParent, false);
            ResourceController resource = obj.GetComponent<ResourceController>();
            resource.SetConfig(config);
            obj.gameObject.SetActive(showResources);
            if (showResources && !resource.IsUnlocked) showResources = false;
            _activeResources.Add(resource); 
        }
    }

    public void ShowNextResource()
    {
        foreach (ResourceController resource in _activeResources)
        {
            if (!resource.gameObject.activeSelf)
            {
                resource.gameObject.SetActive(true);
                break;
            }
        }
    }

    private void CollectPerSecond()
    {
        double output = 0;

        foreach (ResourceController resource in _activeResources)
        {
            if (resource.IsUnlocked) output += resource.GetOutput();
        }

        output *= _autoCollectPercentage;
        _autoCollectInfo.text = $"Auto Collect: { output:F1} / second";
        AddGold(output);

    }

    private void AddGold(double value)
    {
        _totalGold += value;
        _goldInfo.text = $"Gold: { _totalGold:0}";
    }

    public void SpendGoldToUpgrade(double upgradeCost, Action<bool> isUpgrade)
    {
        if ((_totalGold - upgradeCost) >= 0)
        {
            isUpgrade(true);
            _totalGold -= upgradeCost;
            _goldInfo.text = $"Gold: { _totalGold:0}";
        }
        else
            print("cannot upgrade your gold not enough");
    }

    public void SpendGoldToUnlock(double unlockCost, Action<bool> isUnlock)
    {
        if ((_totalGold - unlockCost) >= 0)
        {
            isUnlock(true);
            _totalGold -= unlockCost;
            _goldInfo.text = $"Gold: { _totalGold:0}";
        }
        else
            print("cannot unlock your gold not enough");
    }

    public void CollectByTap(Vector3 tapPosition, Transform parent)
    {
        double output = 0;

        foreach (ResourceController resource in _activeResources)
        {
            if(resource.IsUnlocked) output += resource.GetOutput();
        }

        TapText tapText = GetOrCreateTapText();
        tapText.transform.SetParent(parent, false);
        tapText.transform.position = tapPosition;

        tapText.Text = $"+{ output:0}";
        tapText.gameObject.SetActive(true);
        _coinIcon.transform.localScale = Vector3.one * 1.4f;

        AddGold(output);
    }

    private TapText GetOrCreateTapText()
    {
        TapText tapText = _tapTextPool.Find(t => !t.gameObject.activeSelf);

        if (tapText == null)
        {
            tapText = Instantiate(_tapTextPrefab).GetComponent<TapText>();

            _tapTextPool.Add(tapText);

        }
        return tapText;
    }
}
