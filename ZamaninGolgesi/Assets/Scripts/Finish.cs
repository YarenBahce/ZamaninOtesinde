using System;
using UnityEngine;

public class Finish : MonoBehaviour
{
    public void Start()
    {
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManager.Instance.Win();
        }
    }
}