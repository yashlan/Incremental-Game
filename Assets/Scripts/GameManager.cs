using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[Serializable]
public struct ResourceConfig
{
    public string Name;
    public double UnlockCost;
    public double UpgradeCost;
    public double Output;
}

[Serializable]
public struct Task
{
    public int id;
    public string label;
    public bool isCompleted;
}

public enum StateGame
{
    Ready,
    Start,
    Win,
    Lose
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

    [Header("State Game")]
    [SerializeField]
    private StateGame _stateGame;

    [Header("Auto Collect")]
    [Range(0f, 1f)]
    [SerializeField]
    private float _autoCollectPercentage = 0.1f;

    [Header("Panel Info")]
    [SerializeField]
    private GameObject panelWin;
    [SerializeField]
    private GameObject panelLose;

    [Header("TimeLeft")]
    public float _timeLeft;
    [SerializeField]
    private Text _timeText;

    [Header("resource")]
    [SerializeField]
    private ResourceConfig[] _resourcesConfigs;
    [SerializeField]
    public Sprite[] ResourcesSprites;
    [SerializeField]
    private Transform _resourcesParent;
    [SerializeField]
    private ResourceController _resourcePrefab;

    [Header("tap text")]
    [SerializeField]
    private TapText _tapTextPrefab;
    [SerializeField]
    private RectTransform _coinIcon;

    [Header("Text Info")]
    [SerializeField]
    private Text _goldInfo;
    [SerializeField]
    private Text _autoCollectInfo;

    [Header("Current Golds")]
    [SerializeField]
    private double _totalGold;

    [Header("Task")]
    public bool AllTaskComplete;
    [SerializeField]
    private Task[] tasks;
    [SerializeField]
    private Transform _taskParent;
    [SerializeField]
    private TaskController _taskPrefab;

    private List<ResourceController> _activeResources = new List<ResourceController>();
    private List<TapText> _tapTextPool = new List<TapText>();
    private List<TaskController> _taskControllerList = new List<TaskController>();
    private float _collectSecond;

    public double TotalGold => _totalGold;

    public void ChangeStateToStartOnClick() => _stateGame = StateGame.Start;
    public void RestartOnClick() => SceneManager.LoadScene(0);

    void Start()
    {
        AddAllResources();
        AddAllTasks();
        _timeText.text = null;
    }

    void Update()
    {
        if (_stateGame == StateGame.Start)
        {
            _timeLeft -= Time.deltaTime;
            _timeText.text = "Time : " + Mathf.Round(_timeLeft).ToString() + "s";
            _collectSecond += Time.unscaledDeltaTime;
            if (_collectSecond >= 1f)
            {
                CollectPerSecond();
                _collectSecond = 0f;
            }

            CheckResourceCost();
            SetTaskCompleted(tasks);

            AchievementController.Instance.SetAchievementReachedGolds(TotalGold);

            _coinIcon.transform.localScale = Vector3.LerpUnclamped(_coinIcon.transform.localScale, Vector3.one * 1.2f, 0.15f);
            _coinIcon.transform.Rotate(0f, 0f, Time.deltaTime * -100f);

            if(_timeLeft <= 0)
            {
                _timeLeft = 0;
                _stateGame = StateGame.Lose;
            }

            CheckAllTaskCompleted();

        }
        else if(_stateGame == StateGame.Win)
        {
            panelWin.SetActive(true);
        }
        else if(_stateGame == StateGame.Lose)
        {
            panelLose.SetActive(true);
        }

    }

    bool isBuyable = false;
    private void CheckResourceCost()
    {
        foreach (ResourceController resource in _activeResources)
        {
            if (!resource.MaxLevel)
            {
                if (resource.IsUnlocked)
                    isBuyable = TotalGold >= resource.GetUpgradeCost();
                else
                    isBuyable = TotalGold >= resource.GetUnlockCost();

                resource.ResourceImage.sprite = ResourcesSprites[isBuyable ? 1 : 0];
            }
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

    private void AddAllTasks()
    {
        foreach (Task task in tasks)
        {
            GameObject obj = Instantiate(_taskPrefab.gameObject, _taskParent, false);
            TaskController taskController = obj.GetComponent<TaskController>();
            taskController.SetTask(task);
            _taskControllerList.Add(taskController);
        }
    }

    private bool CheckAllTaskCompleted()
    {
        for (int i = 0; i < tasks.Length; i++)
        {
            if (!tasks[i].isCompleted)
            {
                return false;
            }
        }
        _stateGame = StateGame.Win;
        AllTaskComplete = true;
        return true;
    }

    private void SetTaskCompleted(Task[] tasks)
    {
        foreach(var task in tasks)
        {
            int index = task.id - 1;
            switch (task.id)
            {
                case 1:
                    TaskReachLevel(25, task.id, isReached => 
                    {
                        if (isReached) tasks[index].isCompleted = true;
                    });
                    break;

                case 2:
                    TaskReachLevel(50, task.id, isReached =>
                    {
                        if (isReached) tasks[index].isCompleted = true;
                    });
                    break;

                case 3:
                    TaskReachLevel(100, task.id, isReached =>
                    {
                        if (isReached) tasks[index].isCompleted = true;
                    });
                    break;

                case 4:
                    TaskReachAutoCollect(1000000, task.id, isReached =>
                    {
                        if (isReached) tasks[index].isCompleted = true;
                    });
                    break;

                case 5:
                    TaskReachAutoCollect(5000000, task.id, isReached =>
                    {
                        if (isReached) tasks[index].isCompleted = true;
                    });
                    break;

                case 6:
                    TaskReachAutoCollect(10000000, task.id, isReached =>
                    {
                        if (isReached) tasks[index].isCompleted = true;
                    });
                    break;

                case 7:
                    TaskReachGolds(100000000, task.id, isReached =>
                    {
                        if (isReached) tasks[index].isCompleted = true;
                    });
                    break;

                case 8:
                    TaskReachGolds(1000000000, task.id, isReached =>
                    {
                        if (isReached) tasks[index].isCompleted = true;
                    });
                    break;

                case 9:
                    TaskReachGolds(100000000000, task.id, isReached =>
                    {
                        if (isReached) tasks[index].isCompleted = true;
                    });
                    break;

                case 10:
                    TaskReachGolds(150000000000, task.id, isReached =>
                    {
                        if (isReached) tasks[index].isCompleted = true;
                    });
                    break;
            }
        }
    }

    private void TaskReachLevel(int level, int id, Action<bool> isReached)
    {
        if (IsResourcesLevelSame(level))
        {
            foreach (var taskcontroller in _taskControllerList)
            {
                if (taskcontroller.ID == id)
                {
                    isReached(true);
                    taskcontroller.toggle.isOn = true;
                }
            }
        }
    }

    private bool IsResourcesLevelSame(int level)
    {
        return _activeResources[0].Level >= level &&
               _activeResources[1].Level >= level && 
               _activeResources[2].Level >= level && 
               _activeResources[3].Level >= level;
    }

    private void TaskReachAutoCollect(double taskPoint, int id, Action<bool> isReached)
    {
        foreach (ResourceController resource in _activeResources)
        {
            if ((resource.GetOutput() * _autoCollectPercentage) >= taskPoint)
            {
                foreach (var taskcontroller in _taskControllerList)
                {
                    if (taskcontroller.ID == id)
                    {
                        isReached(true);
                        taskcontroller.toggle.isOn = true;
                    }
                }
            }
        }
    }
    private void TaskReachGolds(double golds, int id, Action<bool> isReached)
    {
        foreach (ResourceController resource in _activeResources)
        {
            if (TotalGold >= golds)
            {
                foreach (var taskcontroller in _taskControllerList)
                {
                    if (taskcontroller.ID == id)
                    {
                        isReached(true);
                        taskcontroller.toggle.isOn = true;
                    }
                }
            }
        }
    }
}
