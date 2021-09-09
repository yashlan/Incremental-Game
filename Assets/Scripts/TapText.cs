using UnityEngine;
using UnityEngine.UI;

public class TapText : MonoBehaviour
{
    public float SpawnTime = 0.5f;
    private float _spawnTime;

    [SerializeField]
    private Text _tapText;
    public string text {
        get => _tapText.text;
        set => _tapText.text = value;
    }  

    void OnEnable()
    {
        _spawnTime = SpawnTime;
    }

    void Update()
    {
        _spawnTime -= Time.unscaledDeltaTime;
        if (_spawnTime <= 0f)
        {
            gameObject.SetActive(false);
        }
        else
        {
            _tapText.CrossFadeAlpha(0f, 0.5f, false);
            if (_tapText.color.a == 0f)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
