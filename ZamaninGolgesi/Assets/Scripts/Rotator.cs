using UnityEngine;

public class Rotator : MonoBehaviour
{
    public float rotateSpeed = 90f;
    public GameManager gameManager;

    void Update()
    {
        if (gameManager.timeIsMoving)
        {
            transform.Rotate(0, 0, rotateSpeed * Time.deltaTime);
        }
    }
}