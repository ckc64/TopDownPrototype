using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MapGenerator : MonoBehaviour
{
    public Transform tilePrefab;
    public Transform obstaclePrefab;
    public Transform navMeshMaskPrefab;
    // transfer to Map Class public Vector2 mapSize;
    public Vector2 maximumMapSize;
    public Transform mapFloor;
    public Transform currentMapFloor;
    public float outlinePercent;

    /* transfer to Map Class [Range(0,1)]
    public float obstaclePercent;*/

    List<Coord> allTileCoords;
    Queue<Coord> shuffledTileCoords;
    Queue<Coord> shuffledOpenTileCoords;
    // transfer to Map Class public int seed = 10;


    public float tileSize;

    // transfer to Map ClassCoord mapCenter;

    public Map[] maps;
    public int mapIndex;
    Map currentMap;

    //variable for saving tiles
    Transform[,] tileMap;

  void Awake()
    {
        FindObjectOfType<Spawner>().OnNewWave += OnNewWave;
    }


    void OnNewWave(int waveNumber)
    {
        mapIndex = waveNumber - 1;
        GenerateMap();
    }
public void GenerateMap()
{
        
        currentMap = maps[mapIndex];
        tileMap = new Transform[currentMap.mapSize.x, currentMap.mapSize.y];
        System.Random randomIndex = new System.Random(currentMap.seed);
      
        //Generate Coords
    allTileCoords = new List<Coord>();
    for (int x = 0; x < currentMap.mapSize.x; x++)
    {
        for (int y = 0; y < currentMap.mapSize.y; y++)
        {
            allTileCoords.Add(new Coord(x, y));
        }
    }

    shuffledTileCoords = new Queue<Coord>(Utility.ShuffleArray(allTileCoords.ToArray(), currentMap.seed));
        //already in the class mapCenter = new Coord((int)currentMap.mapSize.x / 2, (int)currentMap.mapSize.y / 2);
        //Creating Map Holder
        string holderName = "Generated Map";

    if (transform.Find(holderName))
    {
        DestroyImmediate(transform.Find(holderName).gameObject);
    }

    Transform mapHolder = new GameObject(holderName).transform;
    mapHolder.parent = transform;
        //Spawn Tiles
    for(int x = 0; x < currentMap.mapSize.x; x++)
    {
        for(int y = 0; y < currentMap.mapSize.y; y++)
        {
            Vector3 tilePosition = CoordToPosition(x, y);
            Transform newTile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right * 90)) as Transform;
            newTile.localScale = Vector3.one * (1 - outlinePercent) * tileSize;
            newTile.parent = mapHolder;
                tileMap[x, y] = newTile;
        }
    }

    //Spawning Obstacles

    bool[,] obstacleMap = new bool[(int)currentMap.mapSize.x, (int)currentMap.mapSize.y];

    int currentObstacleCount = 0;
    int obstacleCount = (int) (currentMap.mapSize.x * currentMap.mapSize.y * currentMap.obstaclePercent);

        //making list of coordinates
        List<Coord> allOpenCoords = new List<Coord>(allTileCoords);

    for(int i = 0;i<obstacleCount; i++)
    {
        Coord randomCoord = GetRandomCoordinate();
        obstacleMap[randomCoord.x, randomCoord.y] = true;
        currentObstacleCount++;

        if(randomCoord.x != currentMap.mapCenter.x && randomCoord.y != currentMap.mapCenter.y && MapIsAccesible(obstacleMap, currentObstacleCount))
        {
                float obstacleHeight = Mathf.Lerp(currentMap.minObstacleHeight, currentMap.maxObstacleHeight,(float)randomIndex.NextDouble());
            Vector3 obstaclePosition = CoordToPosition(randomCoord.x, randomCoord.y);

            Transform newObstacle = Instantiate(obstaclePrefab, obstaclePosition + Vector3.up * obstacleHeight/2, Quaternion.identity) as Transform;
            newObstacle.parent = mapHolder;
                newObstacle.localScale = new Vector3((1 - outlinePercent) * tileSize, obstacleHeight, ((1 - outlinePercent) * tileSize)); //(1 - outlinePercent) * tileSize;

                allOpenCoords.Remove(randomCoord);
        }
        else
        {
            obstacleMap[randomCoord.x, randomCoord.y] = false;
            currentObstacleCount--;
        }

    }

        shuffledOpenTileCoords = new Queue<Coord>(Utility.ShuffleArray(allOpenCoords.ToArray(), currentMap.seed));

        //creating NaV mESH MASK
        //----
        Transform maskLeft = Instantiate(navMeshMaskPrefab, Vector3.left * (currentMap.mapSize.x + maximumMapSize.x) / 4f * tileSize, Quaternion.identity) as Transform;
    maskLeft.parent = mapHolder;
    maskLeft.localScale = new Vector3((maximumMapSize.x - currentMap.mapSize.x) / 2f, 1, currentMap.mapSize.y ) * tileSize;

    //----
    Transform maskRight = Instantiate(navMeshMaskPrefab, Vector3.right * (currentMap.mapSize.x + maximumMapSize.x) / 4f * tileSize, Quaternion.identity) as Transform;
    maskRight.parent = mapHolder;
    maskRight.localScale = new Vector3((maximumMapSize.x - currentMap.mapSize.x) / 2f, 1, currentMap.mapSize.y) * tileSize;

    //----
    Transform maskTop = Instantiate(navMeshMaskPrefab, (Vector3.forward * (currentMap.mapSize.x + maximumMapSize.x) / 4f * tileSize) , Quaternion.identity) as Transform;
    maskTop.parent = mapHolder;
    maskTop.localScale = new Vector3((maximumMapSize.x) / 2f, 1, (maximumMapSize.y- currentMap.mapSize.y)/2f) * tileSize;
        Debug.Log(Vector3.forward * (currentMap.mapSize.x + maximumMapSize.x) / 4f * tileSize);
        
        //----
        Transform maskBottom = Instantiate(navMeshMaskPrefab, Vector3.back * (currentMap.mapSize.x + maximumMapSize.x) / 4f * tileSize, Quaternion.identity) as Transform;
    maskBottom.parent = mapHolder;
    maskBottom.localScale = new Vector3((maximumMapSize.x) / 2f, 1, (maximumMapSize.y - currentMap.mapSize.y) / 2f) * tileSize;

    //navmeshfloor
    mapFloor.localScale = new Vector3(maximumMapSize.x, maximumMapSize.y) * tileSize;
        currentMapFloor.localScale = new Vector3(currentMap.mapSize.x * tileSize, currentMap.mapSize.y * tileSize);
    }

bool MapIsAccesible(bool[,] obstacleMap, int currentObstacleCount)
{
    bool[,] mapFlags = new bool[obstacleMap.GetLength(0), obstacleMap.GetLength(1)];
    Queue <Coord> q= new Queue<Coord>();
    q.Enqueue(currentMap.mapCenter);
    mapFlags[currentMap.mapCenter.x, currentMap.mapCenter.y] = true;

    int accesibleTileCount = 1;
    while(q.Count > 0)
    {
        Coord tile = q.Dequeue();

        for(int x = -1;  x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                int neighbourX = tile.x + x;
                int neighbourY = tile.y + y;

                if(x == 0 || y == 0)
                {
                    if(neighbourX >= 0 && neighbourX < obstacleMap.GetLength(0) && neighbourY >= 0 && neighbourY < obstacleMap.GetLength(1))
                    {
                        if(!mapFlags[neighbourX, neighbourY] && !obstacleMap[neighbourX, neighbourY])
                        {
                            mapFlags[neighbourX, neighbourY] = true;
                            q.Enqueue(new Coord(neighbourX, neighbourY));
                            accesibleTileCount++;
                        }
                    }
                }
            }
        }
    }

    int targetAccessibleTileCount = (int)(currentMap.mapSize.x * currentMap.mapSize.y - currentObstacleCount);
    return targetAccessibleTileCount == accesibleTileCount;
}

Vector3 CoordToPosition(int x,int y)
{
    return new Vector3(-currentMap.mapSize.x / 2f + 0.5f + x, 0, -currentMap.mapSize.y / 2f + 0.5f + y) *tileSize;
}

    public Transform GetTileFromPosition(Vector3 position)
    {
        //position = ((-mapsize/2)+(1/2)+x)*tileSize
        //x = ((position/tileSize) + (mapSize-1)/2)
        int x = Mathf.RoundToInt(position.x / tileSize + (currentMap.mapSize.x - 1) / 2 ) + 2;
        int y = Mathf.RoundToInt(position.z / tileSize + (currentMap.mapSize.y - 1) / 2 ) + 2;
        x = Mathf.Clamp(x,0, tileMap.GetLength(0)-1);
        y = Mathf.Clamp(y, 0, tileMap.GetLength(1)-1);
        return tileMap[x, y];
    }

public Coord GetRandomCoordinate()
{
    Coord randomCoord = shuffledTileCoords.Dequeue();
    shuffledTileCoords.Enqueue(randomCoord);
    return randomCoord;
}

    public Transform GetRandomOpenTile()
    {
        Coord randomCoord = shuffledOpenTileCoords.Dequeue();
        shuffledOpenTileCoords.Enqueue(randomCoord);
        return tileMap[randomCoord.x, randomCoord.y];
    }
[System.Serializable]
public struct Coord
{
    public int x;
    public int y;

    public Coord(int _x, int _y)
    {
        x = _x;
        y = _y;

    }

}

    [System.Serializable]
    public class Map
    {
        public Coord mapSize;
        [Range(0,1)]
        public float obstaclePercent;
        public int seed;
        public float minObstacleHeight;
        public float maxObstacleHeight;
       

        public Coord mapCenter
        {
            get{
                return new Coord(mapSize.x / 2, mapSize.y / 2);
            }
        }
    }
}
