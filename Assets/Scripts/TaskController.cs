using UnityEngine;
using UnityEngine.UI;

public class TaskController : MonoBehaviour
{
    [SerializeField]
    private int _id;
    public static int id = 0;

    [SerializeField]
    private Text _label;
    [SerializeField]
    private Toggle _toggle;
    public Toggle toggle => _toggle;
    public int ID => _id;

    void Start()
    {
        id++;
        _id = id;
    }
    public void SetTask(Task task)
    {
        _label.text = task.label;
        _toggle.isOn = task.isCompleted;
    }
}
