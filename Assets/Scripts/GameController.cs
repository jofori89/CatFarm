using UnityEngine;

public class GameController : MonoBehaviour
{
    public readonly float RoomYLength = 4;

    public readonly float RoomXLength = 7;

    public static GameController Instance;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }
}