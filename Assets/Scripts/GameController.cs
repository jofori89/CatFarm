using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public readonly float RoomYLength = 4;

    public readonly float RoomXLength = 7;

    private static GameController instance = null;
    private static readonly Object syncRoot = new();

    public static GameController Instance
    {
        get
        {
            if (instance != null)
            {
                return instance;
            }

            lock (syncRoot)
            {
                if (instance == null)
                {
                    instance = FindObjectOfType(typeof(GameController)) as GameController;
                    if (instance == null)
                        instance = new();
                }
            }
            return instance;
        }
    }

    [SerializeField]
    private Text _LblScore;

    [SerializeField]
    private Text _LblCats;

    [SerializeField]
    private int _score;

    private void FixedUpdate()
    {
        if (_LblScore != null)
        { 
            _LblScore.text = $"Score: {_score}";
        }

        if(_LblCats != null)
        {
            _LblCats.text = $"{CatBehaviour.Instance.Cats.Count} cats";
        }
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

        if(_score < 0)
        {
            _score = 0;
        }
    }
}

public enum ScoreChangeType
{
    FoodRefill,
    CatIncrease,
    CatDecrease
}