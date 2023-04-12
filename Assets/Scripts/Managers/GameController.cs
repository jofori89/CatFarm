using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviourSingleton<GameController>
{
    public int RoomYLength => 4;

    public int RoomXLength => 7;

    [SerializeField]
    private Text _lblScore;

    [SerializeField]
    private Text _lblCats;

    [SerializeField]
    private Button _btnStart;

    [SerializeField]
    private int _score;

    private void Start()
    {
        _btnStart.onClick.AddListener(() => StartGame());
        _btnStart.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (_lblScore != null)
        {
            _lblScore.text = $"Score: {_score}";
        }

        if (_lblCats != null)
        {
            _lblCats.text = $"{CatManager.Instance.TotalCat} cats. Total: {CatManager.Instance.Cats.Count}";
        }

        if (CatManager.Instance.TotalCat <= 1 && !_btnStart.IsActive())
        {
           _btnStart.gameObject.SetActive(true);
        }
    }

    public void StartGame()
    {
        _btnStart.gameObject.SetActive(false);

        _score = 0;
        CatManager.Instance.StartNew();
    }

    public void ChangeScore(ScoreChangeType type)
    {
        switch (type)
        {
            case ScoreChangeType.FoodRefill:
                _score++;
                break;

            case ScoreChangeType.CatIncrease:
                _score += 2;
                break;

            case ScoreChangeType.CatDecrease:
                _score--;
                break;
        }

        if (_score < 0)
        {
            _score = 0;
        }
    }
}