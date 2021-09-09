using UnityEngine;
using UnityEngine.UI;

public class ResourceController : MonoBehaviour
{
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

    [Header("Level")]
    [SerializeField]
    private int _level = 1;

    private ResourceConfig _config;

    public Image ResourceImage => _resourceImage;
    public bool IsUnlocked { get; private set; }


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
        if (IsUnlocked)
            UpgradeLevel();
        else
            UnlockResource();
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
                GameManager.Instance.ShowNextResource();
            }
        });
    }

    void SetUnlocked(bool unlocked)
    {
        IsUnlocked = unlocked;
        ResourceImage.color = IsUnlocked ? Color.white : Color.grey;
        _resourceUnlockCost.gameObject.SetActive(!unlocked);
        _resourceUpgradeCost.gameObject.SetActive(unlocked);
    }
}
