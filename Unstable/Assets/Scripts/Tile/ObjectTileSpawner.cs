using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ObjectTileSpawner : MonoBehaviour
{
    [Header("Tilemap")]
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Transform parentObj;

    [Header("3D Tile Prefabs")]
    [SerializeField] private GameObject woodTile3D;
    [SerializeField] private GameObject iceTile3D;
    [SerializeField] private GameObject puddleTile3D;
    [SerializeField] private GameObject wallTile3D;

    [Header("2D Tiles")]
    [SerializeField] private TileBase woodTile;
    [SerializeField] private TileBase iceTile;
    [SerializeField] private TileBase puddleTile;
    [SerializeField] private TileBase wallTile;

    private void Start()
    {
        if (parentObj.childCount == 0)
            SpawnTiles();
    }

    public void SpawnTiles()
    {
        for (int i = tilemap.cellBounds.xMin; i < tilemap.cellBounds.xMax; i++) // scan from left to right for tiles
        {
            for (int j = tilemap.cellBounds.yMin; j < tilemap.cellBounds.yMax; j++) // scan from bottom to top for tiles
            {
                Vector3Int tilePos = new Vector3Int(i, j, 0); // if you find a tile, record its position on the tile map grid

                TileBase thisTile = tilemap.GetTile(tilePos);

                GameObject objToSpwan = GetTilePrefab(thisTile);
                if (objToSpwan == null)
                    continue;

                
                GameObject newObj = Instantiate(objToSpwan);

                Vector3 worldPos = tilemap.CellToWorld(tilePos) + new Vector3(1, -0.5f, -1);
                
                newObj.transform.position = worldPos;
                newObj.transform.parent = parentObj;
            }
        }
        tilemap.gameObject.SetActive(false);
    }

    GameObject GetTilePrefab(TileBase currTile)
    {
        if (currTile == woodTile)
            return woodTile3D;

        if (currTile == iceTile)
            return iceTile3D;

        if (currTile == puddleTile)
            return puddleTile3D;

        if (currTile == wallTile)
            return wallTile3D;

        return null;
    }

    public void RemoveTiles()
    {
        int teskBreak = 1000;
        while(teskBreak > 0)
        {
            if (parentObj.childCount <= 0)
                break;

            DestroyImmediate(parentObj.GetChild(0).gameObject);
            teskBreak--;
        }

        tilemap.gameObject.SetActive(true);
    }
}
