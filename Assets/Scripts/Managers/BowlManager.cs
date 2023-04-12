using System.Collections.Generic;
using UnityEngine;

public class BowlManager : MonoBehaviourSingleton<BowlManager>
{
    private int _maxBowl;

    public List<Bowl> Bowls { private set; get; } = new();

    [SerializeField]
    private Bowl _bowlPrefab;

    private void Awake()
    {
        _maxBowl = 5;
    }

    private void Start()
    {
        SpawnBowls();
    }

    private void SpawnBowls()
    {
        for (int i = _maxBowl; i > 0; i--)
        {
            var bowl = Instantiate(_bowlPrefab, new Vector3(Random.Range(-7f, 7f), Random.Range(-1f, -4f), 0), Quaternion.identity);
            bowl.OnRefilled += IncreaseScore;
            bowl.Init();

            Bowls.Add(bowl);
        }
    }

    private void IncreaseScore()
    {
        GameController.Instance.ChangeScore(ScoreChangeType.FoodRefill);
    }
}