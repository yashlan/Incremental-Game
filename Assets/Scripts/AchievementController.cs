using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class AchievementController : MonoBehaviour
{
    private static AchievementController _instance = null;
    public static AchievementController Instance
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<AchievementController>();
            return _instance;
        }

    }

    [SerializeField] 
    private Transform _popUpTransform;
    [SerializeField] 
    private Text _popUpText;
    [SerializeField] 
    private float _popUpShowDuration = 3f;
    [SerializeField] 
    private List<AchievementDataUnlockResource> _achievementUnlockResourceList;
    [SerializeField]
    private List<AchievementDataClickUnlockLevel> _achievementClickUnlockList;
    [SerializeField]
    private List<AchievementDataPegawaiMagangUnlockLevel> _achievementPegawaiMagangUnlockLevelList;
    [SerializeField]
    private List<AchievementDataPegawaiTetapUnlockLevel> _achievementPegawaiTetapUnlockLevelList;    
    [SerializeField]
    private List<AchievementDataManagerUnlockLevel> _achievementManagerUnlockLevelList;
    [SerializeField]
    private List<AchievementDataReachedGold> _achievementReachedGoldList;


    private float _popUpShowDurationCounter;

    void Awake()
    {
        _popUpTransform.localScale = Vector3.right;
    }

    void Update()
    {
        if (_popUpShowDurationCounter > 0)
        {
            _popUpShowDurationCounter -= Time.unscaledDeltaTime;
            _popUpTransform.localScale = Vector3.LerpUnclamped(_popUpTransform.localScale, Vector3.one, 0.5f);
        }
        else
        {
            _popUpTransform.localScale = Vector2.LerpUnclamped(_popUpTransform.localScale, Vector3.right, 0.5f);
        }
    }

    #region UnlockResource

    [System.Serializable]
    public class AchievementDataUnlockResource
    {
        public string Title;
        public AchievementType Type;
        public string Value;
        public bool IsUnlocked;
    }

    public void UnlockResourceAchievement(AchievementType type, string value)
    {
        AchievementDataUnlockResource achievement = 
            _achievementUnlockResourceList.Find(a => a.Type == type && a.Value == value);

        if (achievement != null && !achievement.IsUnlocked)
        {
            achievement.IsUnlocked = true;
            ShowAchivementUnlockResourcePopUp(achievement);
        }
    }

    private void ShowAchivementUnlockResourcePopUp(AchievementDataUnlockResource achievement)
    {
        _popUpText.text = achievement.Title;
        _popUpShowDurationCounter = _popUpShowDuration;
        _popUpTransform.localScale = Vector2.right;
    }
    #endregion


    #region ClickUnlockLevel

    [System.Serializable]
    public class AchievementDataClickUnlockLevel
    {
        public string Title;
        public int Level;
        public bool IsUnlocked;
    }

    public void UnlockClickLevelAchievement(int level)
    {
        AchievementDataClickUnlockLevel achievement =
            _achievementClickUnlockList.Find(a => a.Level == level);

        if (achievement != null && !achievement.IsUnlocked)
        {
            achievement.IsUnlocked = true;
            ShowAchivementUnlockClickLevelPopUp(achievement);
        }
    }

    private void ShowAchivementUnlockClickLevelPopUp(AchievementDataClickUnlockLevel achievement)
    {
        _popUpText.text = achievement.Title;
        _popUpShowDurationCounter = _popUpShowDuration;
        _popUpTransform.localScale = Vector2.right;
    }
    #endregion


    #region PegawaiMagangUnlockLevel

    [System.Serializable]
    public class AchievementDataPegawaiMagangUnlockLevel
    {
        public string Title;
        public int Level;
        public bool IsUnlocked;
    }

    public void UnlockPegawaiMagangLevelAchievement(int level)
    {
        AchievementDataPegawaiMagangUnlockLevel achievement = _achievementPegawaiMagangUnlockLevelList.Find(a => a.Level == level);

        if (achievement != null && !achievement.IsUnlocked)
        {
            achievement.IsUnlocked = true;
            ShowAchivementPegawaiMagangUnlockLevelPopUp(achievement);
        }
    }

    private void ShowAchivementPegawaiMagangUnlockLevelPopUp(AchievementDataPegawaiMagangUnlockLevel achievement)
    {
        _popUpText.text = achievement.Title;
        _popUpShowDurationCounter = _popUpShowDuration;
        _popUpTransform.localScale = Vector2.right;
    }

    #endregion


    #region PegawaiTetapUnlockLevel

    [System.Serializable]
    public class AchievementDataPegawaiTetapUnlockLevel
    {
        public string Title;
        public int Level;
        public bool IsUnlocked;
    }

    public void UnlockPegawaiTetapLevelAchievement(int level)
    {
        AchievementDataPegawaiTetapUnlockLevel achievement = _achievementPegawaiTetapUnlockLevelList.Find(a => a.Level == level);

        if (achievement != null && !achievement.IsUnlocked)
        {
            achievement.IsUnlocked = true;
            ShowAchivementPegawaiTetapUnlockLevelPopUp(achievement);
        }
    }

    private void ShowAchivementPegawaiTetapUnlockLevelPopUp(AchievementDataPegawaiTetapUnlockLevel achievement)
    {
        _popUpText.text = achievement.Title;
        _popUpShowDurationCounter = _popUpShowDuration;
        _popUpTransform.localScale = Vector2.right;
    }

    #endregion


    #region ManagerUnlockLevel

    [System.Serializable]
    public class AchievementDataManagerUnlockLevel
    {
        public string Title;
        public int Level;
        public bool IsUnlocked;
    }

    public void UnlockManagerLevelAchievement(int level)
    {
        AchievementDataManagerUnlockLevel achievement = _achievementManagerUnlockLevelList.Find(a => a.Level == level);

        if (achievement != null && !achievement.IsUnlocked)
        {
            achievement.IsUnlocked = true;
            ShowAchivementManagerUnlockLevelPopUp(achievement);
        }
    }

    private void ShowAchivementManagerUnlockLevelPopUp(AchievementDataManagerUnlockLevel achievement)
    {
        _popUpText.text = achievement.Title;
        _popUpShowDurationCounter = _popUpShowDuration;
        _popUpTransform.localScale = Vector2.right;
    }

    #endregion


    #region Reached Gold

    [System.Serializable]
    public class AchievementDataReachedGold
    {
        public AchievementType type;
        public string Title;
        public double Gold;
        public bool IsUnlocked;
    }

    public void UnlockReachedGoldAchievement(AchievementType type, double gold)
    {
        AchievementDataReachedGold achievement = _achievementReachedGoldList.Find(a => gold >= a.Gold && a.type == type);

        if (achievement != null && !achievement.IsUnlocked)
        {
            achievement.IsUnlocked = true;
            ShowAchivementReachedGoldPopUp(achievement);
        }
    }

    private void ShowAchivementReachedGoldPopUp(AchievementDataReachedGold achievement)
    {
        _popUpText.text = achievement.Title;
        _popUpShowDurationCounter = _popUpShowDuration;
        _popUpTransform.localScale = Vector2.right;
    }

    public void SetAchievementReachedGolds(double TotalGold)
    {
        UnlockReachedGoldAchievement(AchievementType.Reach100kGolds, TotalGold);
        UnlockReachedGoldAchievement(AchievementType.Reach500kGolds, TotalGold);
        UnlockReachedGoldAchievement(AchievementType.Reach1MGolds,   TotalGold);
        UnlockReachedGoldAchievement(AchievementType.Reach5MGolds,   TotalGold);
        UnlockReachedGoldAchievement(AchievementType.Reach10MGolds,  TotalGold);
        UnlockReachedGoldAchievement(AchievementType.Reach50MGolds,  TotalGold);
        UnlockReachedGoldAchievement(AchievementType.Reach100MGolds, TotalGold);
        UnlockReachedGoldAchievement(AchievementType.Reach500MGolds, TotalGold);
        UnlockReachedGoldAchievement(AchievementType.Reach1BGolds,   TotalGold);
        UnlockReachedGoldAchievement(AchievementType.Reach100BGolds, TotalGold);
    }

    #endregion
}

public enum AchievementType
{
    UnlockResource,
    Reach100kGolds,
    Reach500kGolds,
    Reach1MGolds,
    Reach5MGolds,
    Reach10MGolds,
    Reach50MGolds,
    Reach100MGolds,
    Reach500MGolds,
    Reach1BGolds,
    Reach100BGolds,
}