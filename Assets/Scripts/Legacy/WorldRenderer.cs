using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldRenderer : MonoBehaviour
{
    public Tilemap groundTilemap;
    public Tilemap overgroundTilemap;
    [SerializeField] TileBase[] groundTiles = new TileBase[3]; // can be grassy-arid -> island variation -> level of grass + dryness so can also get bare moist dirt
    [SerializeField] TileBase[] forestTiles = new TileBase[3]; // can be determined by types of tree + the soil that is usually associated w them - littlewood, pine, oak -> variation per island, w influence from neighbouring islands, also influence from type of ground as above
    [SerializeField] TileBase[] mountainTiles = new TileBase[3]; // can also be barren - sometimes don't spawn colliders and use diff sprite for this -> variation per mountain tile on island
    [SerializeField] TileBase[] houseTiles = new TileBase[2];

    public void RenderWorld(CA2D landCA, CA2D houseCA) {
        Debug.Log("starting tile setting loop ...");

        Vector3Int pos = new Vector3Int();

        for (int i = 0; i < landCA.GetXsize(); i++) {
            for (int j = 0; j < landCA.GetYsize(); j++) {
                pos.Set(i, j, 0);
                int landCell = landCA.GetCell(i, j);

                if (landCell == 0) continue;
                else if (landCell > 0 && landCell < 4) {
                    groundTilemap.SetTile(pos, groundTiles[landCell - 1]);
                }
                else if (landCell >= 4 && landCell < 7) {
                    groundTilemap.SetTile(pos, forestTiles[landCell - 4]);
                }
                else if (landCell >= 7 && landCell < 10) {
                    groundTilemap.SetTile(pos, mountainTiles[landCell - 7]);
                }
                // else ?

                int houseCell = houseCA.GetCell(i, j);
                if (houseCell > 0) {
                    overgroundTilemap.SetTile(pos, houseTiles[houseCell - 1]);
                }
            }
        }

        Debug.Log("finished tile setting loop !");
    }
}