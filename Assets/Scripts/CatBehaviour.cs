using UnityEngine;

public class CatBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
    }

    internal void Spawn(float x, float y)
    {
        Debug.Log("Spawn cat at  x:" + x + " y:" + y);
        Instantiate(gameObject, new Vector3(x, y, 0), Quaternion.identity);
    }
}