using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldGenerator : MonoBehaviour {

    public Tilemap groundTilemap; // poss make separate WorldRenderer
    public Tilemap overgroundTilemap; // poss make separate WorldRenderer
    public TileBase groundTile; // can be grassy-arid -> island variation -> level of grass + dryness so can also get bare moist dirt
    public TileBase forestTile; // can be determined by types of tree + the soil that is usually associated w them - spruce, pine, oak -> variation per island, w influence from neighbouring islands
    public TileBase mountainTile; // can also be barren - sometimes don't spawn colliders and use diff sprite for this -> variation per mountain tile on island

    public TileBase houseTile;
    public TileBase hutTile;
    public TileBase rockTile;

    public TileBase shrubTile;
    public TileBase treeTile;
    public TileBase woodTile;

    public TileBase stoneTile;
    public TileBase cragTile;
    public TileBase peakTile;

    public ProceduralIsland[] islands;
    public Transform player;

    public enum blocktypes {
        ground = 1,
        forest = 2,
        mountain = 3
    }

    private CA2D ca, blockCA;
    private int[] groundTileRuleset = { 0, 0, 0, 1, 2, 0, 0, 3, 0, 1, 0, 0, 1, 0, 0, 0, 0, 2, 0, 0, 1, 1, 3, 0, 0, 0, 0, 0, 3, 0, 3, 1 },
                  forestTileRuleset = { 0, 0, 0, 1, 0, 0, 1, 3, 3, 0, 1, 0, 2, 3, 0, 0, 3, 1, 0, 2, 3, 3, 0, 3, 3, 2, 0, 0, 3, 3, 0, 0 },
                  mountainTileRuleset = { 0, 0, 3, 0, 3, 0, 2, 0, 3, 3, 3, 2, 3, 0, 3, 0, 0, 1, 2, 0, 0, 2, 2, 0, 1, 3, 0, 0, 0, 0, 3, 0 };
    // other for specific features eg ruins

    private int blockSize = 10;

    // Start is called before the first frame update
    void Start() {

        ca = new CA2D(4, 384) {
            ruleSet = new int[] { 0, 0, 3, 0, 3, 0, 2, 0, 3, 3, 3, 2, 3, 0, 3, 0, 0, 1, 2, 0, 0, 2, 2, 0, 1, 3, 0, 0, 0, 0, 3, 0 }
        };

        blockCA = new CA2D(4, blockSize) {
            //ruleSet = new int[] { 0, 1, 0, 1, 0, 0, 1, 3, 3, 0, 1, 0, 2, 3, 0, 0, 3, 1, 0, 2, 3, 3, 0, 3, 0, 2, 0, 0, 3, 3, 0, 0 }
            ruleSet = groundTileRuleset
        };

        islands = new ProceduralIsland[30];

        generateWorld();

        player.position = new Vector3(islands[0].location.x * blockSize, islands[0].location.y * blockSize, 0);

        // below should probs be all in WorldRenderer
        Vector3Int blockArea = new Vector3Int(blockSize, blockSize, 1);
        BoundsInt area = new BoundsInt(new Vector3Int(0,0,0), blockArea);
        TileBase[] groundTilesArray = Enumerable.Repeat(groundTile, blockSize * blockSize).ToArray();
        TileBase[] forestTilesArray = Enumerable.Repeat(forestTile, blockSize * blockSize).ToArray();
        TileBase[] mountainTilesArray = Enumerable.Repeat(mountainTile, blockSize * blockSize).ToArray();
        TileBase[] tilesArray = groundTilesArray;

        //Array.ForEach(ca.cells[0], Debug.Log);

        for (int i = 0; i < ca.GetXsize(); i++) {
            for (int j = 0; j < ca.GetYsize(); j++) {
                if (ca.cells[i, j] == 0) continue;
                else if (ca.cells[i, j] == 1) tilesArray = groundTilesArray;
                else if (ca.cells[i, j] == 2) tilesArray = forestTilesArray;
                else if (ca.cells[i, j] == 3) tilesArray = mountainTilesArray;
                area.x = i * blockSize;
                area.y = j * blockSize;
                if (ca.cells[i, j] > 0) groundTilemap.SetTilesBlock(area, tilesArray);
            }
        }
    }

    private void generateWorld() {

        System.Random rnd = new System.Random();
        int[,] islandLocations = { { 272, 137 }, { 300, 164 }, { 308, 192 }, { 222, 132 }, { 120, 79 },
                                   { 181, 27 }, { 347, 184 }, { 129, 120 }, { 265, 74 }, { 95, 31 },
                                   { 68, 50 }, { 303, 71 }, { 309, 135 }, { 291, 140 }, { 209, 101 },
                                   { 283, 185 }, { 124, 105 }, { 157, 33 }, { 356, 165 }, { 123, 103 },
                                   { 217, 183 }, { 30, 63 }, { 324, 190 }, { 44, 32 }, { 317, 67 },
                                   { 89, 134 }, { 138, 21 }, { 249, 157 }, { 331, 165 }, { 207, 30 } };

        for (int i = 0; i < islands.Length; i++) {
            //var x = 20 + ((float)rnd.NextDouble() * (ca.GetXsize() - 40));
            //var y = 20 + ((float)rnd.NextDouble() * (ca.GetXsize() - 40));
            islands[i] = new ProceduralIsland(
                new Vector2(islandLocations[i, 0], islandLocations[i, 1]),
                ca.GetXsize(),
                rnd.Next(0, 9),
                rnd.Next(4, 7),
                new int[,] { } // add seeds here
            );
        }

        ca.Clear();
        // smaller isles only get seeded after X iterations -> growthDelay
        for (int i = 0; i < 11; i++) {
            for (int n = 0; n < islands.Length; n++) {
                if (i == islands[n].growthDelay) {
                    for (int j = 0; j < islands[n].seeds.GetLength(0); j++) {
                        ca.cells[(int)islands[n].location.x + islands[n].seeds[j, 0],
                                 (int)islands[n].location.y + islands[n].seeds[j, 1]] =
                                 islands[n].seeds[j, 2];
                    }
                }
            }
            ca.Update();
        }

        removeLagoons(4);

        for (int i = 0; i < ca.GetXsize(); i++) {
            for (int j = 0; j < ca.GetYsize(); j++) {
                if(ca.cells[i,j] == 1) {
                    // ground block
                    blockCA.ruleSet = groundTileRuleset;
                    blockCA.SetRandomStates(0.05d);
                    blockCA.Update(3);
                    Vector3Int loc = new Vector3Int();
                    for (int k = 0; k < blockSize; k++) {
                        for(int l = 0; l < blockSize; l++) {
                            if(blockCA.cells[k,l] > 0) {
                                loc = new Vector3Int( (i * blockSize) + k, (j * blockSize) + l, 0);
                                if (blockCA.cells[k,l] == 1) overgroundTilemap.SetTile(loc, houseTile);
                                else if(blockCA.cells[k,l] == 2) overgroundTilemap.SetTile(loc, hutTile);
                                else if(blockCA.cells[k,l] == 3) overgroundTilemap.SetTile(loc, rockTile);
                            }
                        }
                    }
                }
                else if (ca.cells[i, j] == 2) {
                    // forest block
                    blockCA.ruleSet = forestTileRuleset;
                    blockCA.SetRandomStates(0.1d);
                    blockCA.Update(2);
                    Vector3Int loc = new Vector3Int();
                    for (int k = 0; k < blockSize; k++) {
                        for (int l = 0; l < blockSize; l++) {
                            if (blockCA.cells[k, l] > 0) {
                                loc = new Vector3Int((i * blockSize) + k, (j * blockSize) + l, 0);
                                if (blockCA.cells[k, l] == 1) overgroundTilemap.SetTile(loc, shrubTile);
                                else if (blockCA.cells[k, l] == 2) overgroundTilemap.SetTile(loc, treeTile);
                                else if (blockCA.cells[k, l] == 3) overgroundTilemap.SetTile(loc, woodTile);
                            }
                        }
                    }
                }
                else if (ca.cells[i, j] == 3) {
                    // mountain block
                    blockCA.ruleSet = mountainTileRuleset;
                    blockCA.SetRandomStates(0.1d);
                    blockCA.Update(4);
                    Vector3Int loc = new Vector3Int();
                    for (int k = 0; k < blockSize; k++) {
                        for (int l = 0; l < blockSize; l++) {
                            if (blockCA.cells[k, l] > 0) {
                                loc = new Vector3Int((i * blockSize) + k, (j * blockSize) + l, 0);
                                if (blockCA.cells[k, l] == 1) overgroundTilemap.SetTile(loc, stoneTile);
                                else if (blockCA.cells[k, l] == 2) overgroundTilemap.SetTile(loc, cragTile);
                                else if (blockCA.cells[k, l] == 3) overgroundTilemap.SetTile(loc, peakTile);
                            }
                        }
                    }
                }
            }
        }
    }

    private void removeLagoons(int max) {   // doesn't work with neighbourTotals cuz that's from last ca iteration ...
        int[,] buffer = (int[,])ca.cells.Clone();
        for (int i = 0; i < ca.GetXsize(); i++) {
            for (int j = 0; j < ca.GetYsize(); j++) {
                if (ca.neighbourTotals[i,j] > max && ca.cells[i,j] > 0) buffer[i,j] = UnityEngine.Random.Range(1, ca.numStates - 1);
            }
        }
        ca.cells = (int[,])buffer.Clone();
    }
}
