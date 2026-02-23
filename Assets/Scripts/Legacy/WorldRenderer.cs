using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class WorldRenderer : MonoBehaviour
{
    public Tilemap groundTilemap;
    public Tilemap overgroundTilemap;
    [SerializeField] TileBase[] groundTiles = new TileBase[3]; // can be grassy-arid -> island variation -> level of grass + dryness so can also get bare moist dirt
    [SerializeField] TileBase[] forestTiles = new TileBase[3]; // can be determined by types of tree + the soil that is usually associated w them - littlewood, pine, oak -> variation per island, w influence from neighbouring islands, also influence from type of ground as above
    [SerializeField] TileBase[] mountainTiles = new TileBase[3]; // can also be barren - sometimes don't spawn colliders and use diff sprite for this -> variation per mountain tile on island
    [SerializeField] TileBase[] houseTiles = new TileBase[2];
    [SerializeField] TileBase[] groundPropTiles = new TileBase[3];
    [SerializeField] TileBase[] forestPropTiles = new TileBase[3];
    [SerializeField] TileBase[] mountainPropTiles = new TileBase[3];

    public void RenderWorld(CA2D landCA, CA2D houseCA, CA2D forestCA) {
        Debug.Log("starting batch tile rendering ...");
        
        int xSize = landCA.GetXsize();
        int ySize = landCA.GetYsize();
        
        // Pre-allocate lists with estimated capacity
        List<Vector3Int> groundPositions = new List<Vector3Int>(xSize * ySize);
        List<TileBase> groundTilesList = new List<TileBase>(xSize * ySize);
        
        List<Vector3Int> overgroundPositions = new List<Vector3Int>(xSize * ySize);
        List<TileBase> overgroundTilesList = new List<TileBase>(xSize * ySize);
        
        Vector3Int pos = new();
        
        // Single pass through all cells, collecting tiles for batch operations
        for (int i = 0; i < xSize; i++) {
            for (int j = 0; j < ySize; j++) {
                pos.Set(i, j, 0);
                int landCell = landCA.GetCell(i, j);
                
                // Process ground layer
                if (landCell > 0 && landCell < 4) {
                    groundPositions.Add(pos);
                    groundTilesList.Add(groundTiles[landCell - 1]);
                }
                else if (landCell >= 4 && landCell < 7) {
                    groundPositions.Add(pos);
                    groundTilesList.Add(forestTiles[landCell - 4]);
                }
                else if (landCell >= 7 && landCell < 10) {
                    groundPositions.Add(pos);
                    groundTilesList.Add(mountainTiles[landCell - 7]);
                }
                
                // Process overground layer (houses and props)
                int houseCell = houseCA.GetCell(i, j);
                if (houseCell > 0) {
                    overgroundPositions.Add(pos);
                    overgroundTilesList.Add(houseTiles[houseCell - 1]);
                }
                
                int forestCell = forestCA.GetCell(i, j);
                if (forestCell > 0) {
                    overgroundPositions.Add(pos);
                    overgroundTilesList.Add(forestPropTiles[forestCell - 1]);
                }
            }
        }
        
        // Batch set all tiles at once
        groundTilemap.SetTiles(groundPositions.ToArray(), groundTilesList.ToArray());
        overgroundTilemap.SetTiles(overgroundPositions.ToArray(), overgroundTilesList.ToArray());
        
        Debug.Log("finished batch tile rendering !");
    }
}