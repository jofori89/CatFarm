using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class Cat : MonoBehaviour
{
    private readonly int _maxFullLevel = 30;

    [SerializeField]
    public int FullLevel { get; private set; } = 10;

    [SerializeField]
    public bool IsHungry => FullLevel <= 25;

    public bool IsFull => FullLevel == _maxFullLevel;

    public bool IsDoingSomething { get; private set; }

    private bool _isMoving;

    private Coroutine id;

    private void Move(float roomX, float roomY)
    {
        if (IsDoingSomething || _isMoving)
        {
            return;
        }

        var directionX = RandomDirection();
        var directionY = RandomDirection();

        var stepX = UnityEngine.Random.value * directionX;
        var stepY = UnityEngine.Random.value * directionY;

        if (Math.Abs(stepX + gameObject.transform.position.x) >= roomX)
        {
            stepX = 0;
        }

        if (Math.Abs(stepY + gameObject.transform.position.y) >= roomY)
        {
            stepY = 0;
        }

        StartCoroutine(MoveFunction(transform.position += new Vector3(stepX, stepY, 0), 1.0f));
    }

    private int RandomDirection()
    {
        return UnityEngine.Random.value < 0.5f ? 1 : -1;
    }

    private void Start()
    {
        id = StartCoroutine(DoCountDownHungry());  //fires coroutine
    }

    private void Update()
    {
        Move(GameController.Instance.RoomXLength, GameController.Instance.RoomYLength);
    }

    private void GotoEat()
    {
        IsDoingSomething = true;

        Debug.Log("GotoEat");

        Bowl nearestBowl = null;
        float magnitude = 0;

        foreach (var bowl in BowlManager.Instance.Bowls.Where(o => !o.IsEmpty))
        {
            var offset = transform.position - bowl.transform.position;

            if (nearestBowl == null || offset.magnitude < magnitude)
            {
                nearestBowl = bowl;
                magnitude = offset.magnitude;
            }
        }

        IsDoingSomething = false;
        if (nearestBowl == null)
        {
            Debug.Log("No bowl.");
            return;
        }

        Debug.Log("Bowl " + nearestBowl.transform.position);

        StartCoroutine(MoveFunction(nearestBowl.transform.position, magnitude));

        new WaitForSeconds(0.5f);
        FullLevel = nearestBowl.DecreaseFood(3) * 10;

        IsDoingSomething = false;
    }

    private IEnumerator MoveFunction(Vector3 newPosition, float duration)
    {
        if (_isMoving)
        {
            yield break; ///exit if this is still running
        }

        _isMoving = true;

        float counter = 0f;
        while (counter < duration)
        {
            counter += Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, newPosition, counter / duration);

            // If the object has arrived, stop the coroutine
            if (transform.position == newPosition)
            {
                _isMoving = false;
                yield break;
            }

            // Otherwise, continue next frame
            yield return null;
        }

        _isMoving = false;
    }

    private IEnumerator DoCountDownHungry()
    {
        while (FullLevel >= 0)
        {
            yield return new WaitForSeconds(1);
            FullLevel--;

            if (FullLevel > 0 && IsHungry)
            {
                GotoEat();
            }

            // timer is finished...
            if (FullLevel == 0)
            {
                Destroy(gameObject, 1);
            }
        }

        StopCoroutine(id);
    }
}