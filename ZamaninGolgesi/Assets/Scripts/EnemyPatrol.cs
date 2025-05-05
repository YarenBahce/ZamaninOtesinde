using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    private CircularStack<SavedPosition> _savedPositionStack =  new CircularStack<SavedPosition>(5000);
    private bool isRewinding;
    private float rewindTimer;

    private void Update()
    {
        if (!GameManager.Instance.timeIsMoving) { return; }
        if (isRewinding)
        {
            if (rewindTimer < 0f)
            {
                isRewinding = false;
                return;
            }
            if (_savedPositionStack.IsEmpty)
            {
                isRewinding = false;
                return;
            }
            float totalTimeDelta = 0f;
            float timeDelta = Time.deltaTime * 3;
            do
            {
                SavedPosition lastPosition = _savedPositionStack.Pop();
                totalTimeDelta += lastPosition.timeDelta;
                transform.position = lastPosition.position;
            } while (!_savedPositionStack.IsEmpty && totalTimeDelta < timeDelta);
        }
        else
        {
            Vector2 direction = (PlayerMovement.Instance.transform.position - transform.position).normalized;
            if(direction == Vector2.zero) return;
            transform.position += (Vector3)direction * (GameManager.Instance.Current().EnemySpeed * Time.deltaTime);
            _savedPositionStack.Push(new SavedPosition(transform.position, Time.deltaTime));
        }
    }
    public void TriggerRewind(float duration)
    {
        isRewinding = true;
        rewindTimer = duration;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManager.Instance.Lose();
        }
    }
    class SavedPosition
    {
        public Vector3 position;
        public float timeDelta;

        public SavedPosition(Vector3 position, float timeDelta)
        {
            this.position = position;
            this.timeDelta = timeDelta;
        }
    }
    
    public class CircularStack<T>
    {
        private T[] buffer;
        private int top = -1;
        private int count = 0;

        public int Capacity { get; private set; }

        public CircularStack(int capacity)
        {
            Capacity = capacity;
            buffer = new T[capacity];
        }

        public void Push(T item)
        {
            top = (top + 1) % Capacity;
            buffer[top] = item;
            if (count < Capacity) count++;
        }

        public T Pop()
        {
            if (count == 0) throw new System.Exception("Empty stack");

            T item = buffer[top];
            top = (top - 1 + Capacity) % Capacity;
            count--;
            return item;
        }

        public bool IsEmpty => count == 0;
    }
    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, GetComponent<BoxCollider2D>().size);
    }
}