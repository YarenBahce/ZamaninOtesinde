using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum TileType { Empty, Ground, Wall, TimeCapsule }

public class TileData {
    public TileType GroundLayer = TileType.Empty;
    public TileType ObstacleLayer = TileType.Empty;
    public TileType ObjectLayer = TileType.Empty;
}