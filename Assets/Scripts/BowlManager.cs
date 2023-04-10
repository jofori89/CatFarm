using System.Collections.Generic;
using UnityEngine;

public class BowlManager : MonoBehaviour
{
    public static BowlManager Instance;

    private readonly int _maxBowl = 5;

    [SerializeField]
    public List<Bowl> Bowls { private set; get; } = new();

    [SerializeField]
    private Bowl _bowlPrefab;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SpawnBowls();
    }

    // Update is called once per frame
    private void Update()
    {
       
    }

    private void SpawnBowls()
    {
        for (int i = _maxBowl; i > 0; i--)
        {
            var bowl = Instantiate(_bowlPrefab, new Vector3(Random.Range(-7f, 7f), Random.Range(-1f, -4f), 0), Quaternion.identity);

            bowl.Init(this);
            Bowls.Add(bowl);
        }
    }

}