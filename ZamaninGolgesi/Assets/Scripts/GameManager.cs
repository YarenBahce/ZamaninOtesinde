using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool timeIsMoving = true;
    public Level[] level;

    [Header("UI")]
    public GameObject winPanel;
    public GameObject losePanel;
    public GameObject portal;

    
    private static int _currentLevel;
    private int catchedCapsuleCount = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        Time.timeScale = 1f;
        catchedCapsuleCount = 0;
    }
    
    public void Win()
    {
        timeIsMoving = false;
        if (!LevelUp())
        {
            winPanel.SetActive(true);
            Time.timeScale = 0f;
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        
    }

    public void Lose()
    {
        timeIsMoving = false;
        losePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public Vector2 ProcessMovement()
    {
        Vector2 movement = _calculateMovement();
        bool isMoving = movement != Vector2.zero;
        timeIsMoving = isMoving;
        return movement;
    }

    public void CatchCapsule()
    {
        catchedCapsuleCount++;
        if (IsAllCapsulesCatched())
        {
            portal.SetActive(true);
        }
    }

    public bool IsAllCapsulesCatched()
    {
        return Current().CapsuleCount == catchedCapsuleCount;
    }

    private Vector2 _calculateMovement()
    {
        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    public Level Current()
    {
        return level[_currentLevel];
    }

    public bool LevelUp()
    {
        _currentLevel++;
        return _currentLevel < level.Length;
    }
    
    [System.Serializable]
    public class Level
    {
        public float EnemySpeed;
        public int CapsuleCount;
        public int ObstacleRate;
        public int EdgeSize;
        public Tile BackgroundTile;
        public Tile[] ObstacleTiles;
        public Tile Border;
        public GameObject EnemyPrefab;
        public GameObject TimeCapsulePrefab;
    }
}