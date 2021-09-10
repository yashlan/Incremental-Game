using UnityEngine;
using UnityEngine.UI;

public class ResourceController : MonoBehaviour
{
    [Header("id")]
    public int id = 0;
    private static int _id = 0;

    [Header("Resource Component")]
    [SerializeField]
    private Text _resourceDescription;
    [SerializeField]
    private Text _resourceUpgradeCost;
    [SerializeField]
    private Text _resourceUnlockCost;
    [SerializeField]
    private Button _resourceButton;
    [SerializeField]
    private Image _resourceImage;
    [SerializeField]
    private Sprite _defaultResourceSprite;

    [Header("Level")]
    [SerializeField]
    private int _level = 1;
    [SerializeField]
    private bool _maxLevel = false;

    private ResourceConfig _config;

    public int Level => _level;
    public Image ResourceImage => _resourceImage;
    public bool IsUnlocked { get; private set; }
    public bool MaxLevel => _maxLevel;

    void Start()
    {
        _id++;
        id = _id;
    }

    void Update()
    {
        switch (id)
        {
            case 1:
                _maxLevel = _level >= 100;
                SetMaxLevel();
                break;
            case 2:
                _maxLevel = _level >= 100;
                SetMaxLevel();
                break;
            case 3:
                _maxLevel = _level >= 100;
                SetMaxLevel();
                break;
            case 4:
                _maxLevel = _level >= 100;
                SetMaxLevel();
                break;
        }
    }

    public void SetConfig(ResourceConfig config)
    {
        _config = config;
        _resourceDescription.text = $"{ _config.Name } Lv. { _level }\n+{ GetOutput():0}";
        _resourceUnlockCost.text = $"Unlock Cost\n{ _config.UnlockCost }";
        _resourceUpgradeCost.text = $"Upgrade Cost\n{ GetUpgradeCost() }";
    }

    public double GetOutput() => _config.Output * _level;
    public double GetUpgradeCost() => _config.UpgradeCost * _level;
    public double GetUnlockCost() => _config.UnlockCost;

    public void ResourceOnClick()
    {
        if (!_maxLevel)
        {
            if (IsUnlocked)
                UpgradeLevel();
            else
                UnlockResource();
        }
    }

    void SetMaxLevel()
    {
        if (_maxLevel)
        {
            _resourceButton.interactable = false;
            _resourceImage.sprite = _defaultResourceSprite;
            _resourceDescription.text = $"{ _config.Name } Lv. { _level }";
            _resourceUpgradeCost.text = "Max Level!";
            _resourceImage.color = new Color(0, 235, 255, 255);
        }
    }

    void UpgradeLevel()
    {
        double upgradeCost = GetUpgradeCost();
        GameManager.Instance.SpendGoldToUpgrade(upgradeCost, IsUpgrade => {

            if (IsUpgrade)
            {
                _level++;
                _resourceUpgradeCost.text = $"Upgrade Cost\n{ GetUpgradeCost() }";
                _resourceDescription.text = $"{ _config.Name } Lv. { _level }\n+{ GetOutput():0}";

                switch (id)
                {
                    case 1:
                        AchievementController.Instance.UnlockClickLevelAchievement(_level);
                        break;

                    case 2:
                        AchievementController.Instance.UnlockPegawaiMagangLevelAchievement(_level);
                        break;

                    case 3:
                        AchievementController.Instance.UnlockPegawaiTetapLevelAchievement(_level);
                        break;

                    case 4:
                        AchievementController.Instance.UnlockManagerLevelAchievement(_level);
                        break;
                }
            }
        });
    }

    void UnlockResource()
    {
        double unlockCost = GetUnlockCost();
        GameManager.Instance.SpendGoldToUnlock(unlockCost, IsUnlocked => {

            if (IsUnlocked)
            {
                SetUnlocked(true);
                AchievementController.Instance.UnlockResourceAchievement(AchievementType.UnlockResource, _config.Name);
                GameManager.Instance.ShowNextResource();
            }
        });
    }

    void SetUnlocked(bool unlocked)
    {
        IsUnlocked = unlocked;
        _resourceImage.color = IsUnlocked ? Color.white : Color.grey;
        _resourceUnlockCost.gameObject.SetActive(!unlocked);
        _resourceUpgradeCost.gameObject.SetActive(unlocked);
    }
}
