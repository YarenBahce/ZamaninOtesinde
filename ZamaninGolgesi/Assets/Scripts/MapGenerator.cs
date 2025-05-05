using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class MapGenerator : MonoBehaviour
{
    public Tilemap Background;
    public Tilemap Midground;
    // 
    
    private const int BackgroundTileNumber = -1;
    private const int ObjectCloseToPlayerMin = 3;

    void Start()
    {
        GameManager.Level currentLevel = GameManager.Instance.Current();
        _clearTilemaps();
        _initializeBackground(currentLevel.EdgeSize);
        _initializeBorder(currentLevel.EdgeSize);
        int[,] matrix = new int[currentLevel.EdgeSize*2, currentLevel.EdgeSize*2];
        _initializeMatrix(matrix,currentLevel.EdgeSize);
        Tuple<int,int> start = new Tuple<int, int>(currentLevel.EdgeSize, currentLevel.EdgeSize);
        _initializeMidgroundInMatrix(matrix, start, currentLevel);
        bool[,] visited = _BFS(matrix, start,currentLevel.EdgeSize);
        //
        _finalizeMap(matrix,currentLevel.EdgeSize);
        _spawnObjects(visited, start,currentLevel.EdgeSize);
    }

    private void _initializeBorder(int edgeSize)
    {
        for (int x = -edgeSize-1; x < edgeSize+1; x++)
        {
           Midground.SetTile(new Vector3Int(x, edgeSize, 0), GameManager.Instance.Current().Border);
           Midground.SetTile(new Vector3Int(x, -edgeSize-1, 0), GameManager.Instance.Current().Border);
           Midground.SetTile(new Vector3Int(edgeSize, x, 0), GameManager.Instance.Current().Border);
           Midground.SetTile(new Vector3Int(-edgeSize-1, x, 0), GameManager.Instance.Current().Border);
        }
    }

    private void _clearTilemaps()
    {
        Background.ClearAllTiles();
        Midground.ClearAllTiles();
    }

    private void _initializeBackground(int edgeSize)
    {
        for (int x = -edgeSize; x < edgeSize; x++)
        {
            for (int y = -edgeSize; y < edgeSize; y++)
            {
                Background.SetTile(new Vector3Int(x, y, 0), GameManager.Instance.Current().BackgroundTile);
            }
        }
    }

    private void _initializeMidgroundInMatrix(int[,] matrix, Tuple<int, int> start, GameManager.Level currentLevel)
    {
        int numObstacles = (currentLevel.ObstacleRate * currentLevel.EdgeSize*2 * currentLevel.EdgeSize*2) / 100;
        int placed = 0;
        while (placed < numObstacles)
        {
            int x = Random.Range(0, currentLevel.EdgeSize*2); 
            int y = Random.Range(0, currentLevel.EdgeSize*2);
            if (matrix[x, y] == BackgroundTileNumber && _isValidForObject(start, x,y))
            {
                matrix[x, y] = Random.Range(0, GameManager.Instance.Current().ObstacleTiles.Length);
                placed++;
            }
        }
    }

    private void _spawnObjects(bool[,] visited, Tuple<int, int> start, int edgeSize)
    {
        _spawnEnemy(start, edgeSize);
        //
        List<Vector2Int> emptyTiles = _getEmptyTiles(visited, start, edgeSize);
        if (emptyTiles.Count <= 0)
        {
            Debug.LogError("emptyTiles is Empty");
            return;
        }
        for (int i = 0; i < GameManager.Instance.Current().CapsuleCount; i++) {
            Vector2Int capsuleCell = emptyTiles[Random.Range(0, emptyTiles.Count)];
            Instantiate(GameManager.Instance.Current().TimeCapsulePrefab, new Vector3(capsuleCell.x, capsuleCell.y, 0) + new Vector3(0.5f, 0.5f, 0), Quaternion.identity);
        }
    }

    private void _spawnEnemy(Tuple<int, int> start, int edgeSize)
    {
        Vector3 enemyPos = Vector3.zero;
        do
        {
            int x = Random.Range(-edgeSize, edgeSize);
            int y = Random.Range(-edgeSize, edgeSize);
            if (_isValidForObject(start, x, y))
            { 
                enemyPos = new Vector3(x, y, 0);
                Instantiate(GameManager.Instance.Current().EnemyPrefab, enemyPos + new Vector3(0.5f, 0.5f, 0), Quaternion.identity);
            }
        } while (enemyPos == Vector3.zero);
    }

    private void _finalizeMap(int[,] matrix, int edgeSize)
    {
        for (int x = -edgeSize; x < edgeSize; x++)
        {
            for (int y = -edgeSize; y < edgeSize; y++)
            {
                int tile = matrix[x+edgeSize, y+edgeSize];
                Vector3Int vector3Int = new Vector3Int(x, y, 0);
                if (tile == BackgroundTileNumber)
                {
                    continue;
                }
                Tile midgroundTile = GameManager.Instance.Current().ObstacleTiles[tile];
                midgroundTile.colliderType = Tile.ColliderType.Grid;
                Midground.SetTile(vector3Int, midgroundTile);
            }
        }
    }
  
    private bool _isValidMove(int[,] matrix, int x, int y, int edgeSize)
    {
        return x >= 0 && x < edgeSize*2 && y >= 0 && y < edgeSize*2 && matrix[x, y] == BackgroundTileNumber;
    }
    
    private bool[,] _BFS(int[,] matrix, Tuple<int, int> start, int edgeSize)
    {
        bool[,] visited = new bool[edgeSize*2, edgeSize*2];
        Queue<Tuple<int, int>> queue = new Queue<Tuple<int, int>>();
        queue.Enqueue(start);
        visited[start.Item1, start.Item2] = true;

        // Directions for up, down, left, right
        int[] dx = { -1, 1, 0, 0 };
        int[] dy = { 0, 0, -1, 1 };
        
        while (queue.Count > 0)
        {
            Tuple<int,int> current = queue.Dequeue();
            int x = current.Item1;
            int y = current.Item2;
            for (int i = 0; i < 4; i++)
            {
                int nx = x + dx[i];
                int ny = y + dy[i];

                if (_isValidMove(matrix,nx, ny, edgeSize) && !visited[nx, ny])
                {
                    visited[nx, ny] = true;
                    queue.Enqueue(new Tuple<int, int>(nx, ny));
                }
            }
        }
        return visited;
    }

    private void _initializeMatrix(int[,] matrix, int edgeSize)
    {
        for (int x = 0; x < edgeSize*2; x++)
        {
            for (int y = 0; y < edgeSize*2; y++)
            {
                matrix[x, y] = -1;
            }
        }
    }

    private List<Vector2Int> _getEmptyTiles(bool[,] visited, Tuple<int, int> start,  int edgeSize)
    {
        List<Vector2Int> emptyTiles = new List<Vector2Int>();
        for (int x = 0; x < edgeSize*2; x++) {
            for (int y = 0; y < edgeSize*2; y++) {
                if (visited[x, y] && _isValidForObject(start, x, y))
                    emptyTiles.Add(new Vector2Int(x-edgeSize, y-edgeSize));
            }
        }
        return emptyTiles;
    }

    private bool _isValidForObject(Tuple<int, int> start, int x, int y)
    {
        int startLeftX = start.Item1 - ObjectCloseToPlayerMin;
        int startRightX = start.Item1 + ObjectCloseToPlayerMin;
        int startUpY = start.Item2 - ObjectCloseToPlayerMin;
        int startDownY = start.Item2 + ObjectCloseToPlayerMin;
        return x>startRightX || x<startLeftX || y<startUpY || y>startDownY;
    }
    
  
}