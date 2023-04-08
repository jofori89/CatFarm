using Unity.VisualScripting;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private readonly int _maxBowl = 5;

    private readonly int _initCat = 2;

    [SerializeField]
    private BowlBehaviour _pfBowl;

    [SerializeField]
    private GameObject[] pfCat;

    private void Awake()
    {
    }

    // Start is called before the first frame update
    private void Start()
    {
        for (int i = _maxBowl; i > 0; i--)
        {
            _pfBowl.SpawnBowl(Random.Range(-7f, 7f), Random.Range(-1f, -4f));
        }

        for (int i = _initCat; i > 0; i--)
        {
            var cat = pfCat[Random.Range(0, pfCat.Length - 1)]
                .GetComponent<CatBehaviour>();
            cat.Spawn(Random.Range(-7f, 7f), Random.Range(-1f, -4f));
        }
    }

    // Update is called once per frame
    private void Update()
    {
    }
}