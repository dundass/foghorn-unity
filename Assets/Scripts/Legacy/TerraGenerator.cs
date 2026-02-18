using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TerraGenerator : MonoBehaviour {

    private class IslandData {
        public Vector2Int location;
        public int[,] seeds;
        public IslandData(Vector2Int location, int[,] seeds) {
            this.location = location;
            this.seeds = seeds;
        }
    }

    private IslandData[] islandData = {
        new IslandData(new Vector2Int( 272, 137 ), new int[,]{ }), new IslandData(new Vector2Int( 300, 164 ), new int[,]{ }), new IslandData(new Vector2Int( 308, 192 ), new int[,]{}), new IslandData(new Vector2Int( 222, 132 ), new int[,]{}), new IslandData(new Vector2Int( 120, 79 ), new int[,]{}),
        new IslandData(new Vector2Int( 181, 27 ), new int[,]{ }), new IslandData(new Vector2Int( 347, 184 ), new int[,]{}), new IslandData(new Vector2Int( 129, 120 ), new int[,]{}), new IslandData(new Vector2Int( 265, 74 ), new int[,]{}), new IslandData(new Vector2Int( 95, 31 ), new int[,]{}),
        new IslandData(new Vector2Int( 68, 50 ), new int[,]{}), new IslandData(new Vector2Int( 303, 71 ), new int[,]{}), new IslandData(new Vector2Int( 309, 135 ), new int[,]{}), new IslandData(new Vector2Int( 291, 140 ), new int[,]{}), new IslandData(new Vector2Int( 209, 101 ), new int[,]{}),
        new IslandData(new Vector2Int( 283, 185 ), new int[,]{}), new IslandData(new Vector2Int( 124, 105 ), new int[,]{}), new IslandData(new Vector2Int( 157, 33 ), new int[,]{}), new IslandData(new Vector2Int( 356, 165 ), new int[,]{}), new IslandData(new Vector2Int( 123, 103 ), new int[,]{}),
        new IslandData(new Vector2Int( 217, 183 ), new int[,]{}), new IslandData(new Vector2Int( 30, 63 ), new int[,]{}), new IslandData(new Vector2Int( 324, 190 ), new int[,]{}), new IslandData(new Vector2Int( 44, 32 ), new int[,]{}), new IslandData(new Vector2Int( 317, 67 ), new int[,]{}),
        new IslandData(new Vector2Int( 89, 134 ), new int[,]{}), new IslandData(new Vector2Int( 138, 21 ), new int[,]{}), new IslandData(new Vector2Int( 249, 157 ), new int[,]{}), new IslandData(new Vector2Int( 331, 165 ), new int[,]{}), new IslandData(new Vector2Int( 207, 30 ), new int[,]{})
    };

    public int chunkCellIterations = 11;
    public int upscaleCellIterations = 2;

    public Tilemap groundTilemap; // poss make separate WorldRenderer
    public Tilemap overgroundTilemap;
    //public Tilemap overgroundTilemap; // poss make separate WorldRenderer
    [SerializeField] TileBase[] groundTiles = new TileBase[3]; // can be grassy-arid -> island variation -> level of grass + dryness so can also get bare moist dirt
    [SerializeField] TileBase[] forestTiles = new TileBase[3]; // can be determined by types of tree + the soil that is usually associated w them - littlewood, pine, oak -> variation per island, w influence from neighbouring islands, also influence from type of ground as above
    [SerializeField] TileBase[] mountainTiles = new TileBase[3]; // can also be barren - sometimes don't spawn colliders and use diff sprite for this -> variation per mountain tile on island
    [SerializeField] TileBase[] houseTiles = new TileBase[2];

    public GameObject overgroundLights;

    public GameObject denizenParent;
    public GameObject denizenPrefab;

    public ProceduralIsland[] islands;
    public Transform player;

    public enum blocktypes {
        ground = 1,
        forest = 2,
        mountain = 3
    }

    private CA2D landTileCA, houseCA, chunkCA, oceanCA;
    /*private enum rulesets {
        island = new int[] { 0, 0, 3, 0, 3, 0, 2, 0, 3, 3, 3, 2, 3, 0, 3, 0, 0, 1, 2, 0, 0, 2, 2, 0, 1, 3, 0, 0, 0, 0, 3, 0 }
    };*/
    private int[] islandRuleset = { 0, 0, 3, 0, 3, 0, 2, 0, 3, 3, 3, 2, 3, 0, 3, 0, 0, 1, 2, 0, 0, 2, 2, 0, 1, 3, 0, 0, 0, 0, 3, 0 };
        //groundTileRuleset = { 0, 0, 0, 1, 2, 0, 0, 3, 0, 1, 0, 0, 1, 0, 0, 0, 0, 2, 0, 0, 1, 1, 3, 0, 0, 0, 0, 0, 3, 0, 3, 1 },
        //          forestTileRuleset = { 0, 0, 0, 1, 0, 0, 1, 3, 3, 0, 1, 0, 2, 3, 0, 0, 3, 1, 0, 2, 3, 3, 0, 3, 3, 2, 0, 0, 3, 3, 0, 0 },
        //          mountainTileRuleset = { 0, 0, 3, 0, 3, 0, 2, 0, 3, 3, 3, 2, 3, 0, 3, 0, 0, 1, 2, 0, 0, 2, 2, 0, 1, 3, 0, 0, 0, 0, 3, 0 };
        // other for specific features eg ruins

    private int blockSize = 10;
    //private int tileSize = 16;

    System.Random rnd = new System.Random(System.DateTime.Now.Millisecond);

    // Start is called before the first frame update
    void Start() {

        // stores all terra tile states
        /*landTileCA = new CA2D(4, 384 * blockSize) {
            //ruleSet = new int[] { 0, 1, 0, 1, 0, 0, 1, 3, 3, 0, 1, 0, 2, 3, 0, 0, 3, 1, 0, 2, 3, 3, 0, 3, 0, 2, 0, 0, 3, 3, 0, 0 }
            //ruleSet = new int[] { 0, 0, 3, 0, 3, 0, 2, 0, 3, 3, 3, 2, 3, 0, 3, 0, 0, 1, 2, 0, 0, 2, 2, 0, 1, 3, 0, 0, 0, 0, 3, 0 }
            ruleSet = islandRuleset
        };
        landTileCA.ruleSet[8] = 1;
        landTileCA.ruleSet[16] = 2;
        landTileCA.ruleSet[24] = 3;*/
        landTileCA = new CA2D(10, 384 * blockSize) {
            //ruleSet = new int[] {0,3,0,0,0,0,7,1,1,3,0,3,0,2,0,1,2,2,0,3,0,0,0,4,3,3,9,0,0,0,5,4,4,4,0,0,0,0,0,6,5,4,0,0,6,0,0,6,6,6,2,6,0,0,0,5,7,6,0,0,0,2,0,7,8,7,0,3,2,2,6,8,9,9,0,0,2,1,5,0 }
            //ruleSet = new int[] { 0, 0, 0, 0, 0, 0, 0, 5, 1, 8, 0, 0, 8, 5, 3, 0, 2, 0, 0, 3, 1, 3, 0, 0, 3, 0, 0, 0, 3, 0, 4, 6, 4, 0, 0, 3, 7, 0, 0, 0, 5, 1, 9, 0, 0, 6, 0, 0, 6, 0, 4, 4, 4, 0, 1, 0, 7, 0, 0, 0, 0, 0, 0, 0, 8, 0, 0, 0, 0, 0, 0, 0, 9, 0, 7, 0, 0, 9, 0, 0  }
            //ruleSet = new int[] { 0, 0, 0, 0, 0, 0, 1, 0, 1, 2, 2, 0, 2, 1, 2, 2, 2, 2, 0, 0, 0, 0, 0, 3, 3, 0, 3, 0, 4, 0, 0, 0, 4, 5, 4, 0, 4, 4, 5, 0, 5, 5, 0, 0, 5, 0, 0, 0, 6, 6, 6, 6, 7, 6, 6, 7, 7, 0, 0, 0, 0, 0, 7, 0, 8, 0, 8, 0, 0, 8, 8, 0, 9, 9, 9, 0, 0, 0, 9, 0 }
            ruleSet = new int[] { 0, 1, 1, 1, 2, 9, 1, 4, 1, 2, 2, 1, 2, 1, 2, 2, 2, 2, 1, 2, 2, 3, 2, 3, 3, 4, 3, 3, 4, 4, 4, 1, 4, 5, 4, 5, 4, 4, 5, 2, 5, 5, 4, 0, 5, 4, 4, 4, 6, 6, 6, 6, 7, 6, 6, 7, 7, 8, 4, 8, 7, 9, 7, 8, 8, 7, 8, 8, 7, 8, 8, 4, 9, 9, 9, 7, 8, 7, 9, 7 }
        };
        // set all rules for uniformly live neighbourhoods to stay at that total
        for (int i = 0; i < landTileCA.numStates; i++) landTileCA.ruleSet[i * 8] = i;
        //Debug.Log(string.Join(", ", ca.ruleSet));

        houseCA = new CA2D(3, 384 * blockSize);

        // stores all world/island chunk states
        chunkCA = new CA2D(4, 384) {
            ruleSet = islandRuleset
        };

        // generates dynamic texture for ocean tiles
        oceanCA = new CA2D(4, 32) {
            ruleSet = islandRuleset
        };

        islands = new ProceduralIsland[30];

        generateWorld();

        //generateSettlements(); // todo - this adds 40s+ to startup ...

        player.position = new Vector3(islands[0].location.x * blockSize, islands[0].location.y * blockSize, 0);

        // below should probs be all in WorldRenderer

        Debug.Log("starting tile setting loop ...");

        Vector3Int pos = new Vector3Int();

        for (int i = 0; i < landTileCA.getXsize(); i++) {
            for (int j = 0; j < landTileCA.getYsize(); j++) {
                int chunkX = i / blockSize;
                int chunkY = j / blockSize;

                pos.Set(i, j, 0);
                //TileBase tile = groundTiles[0];

                /*if (chunkCA.cells[chunkX, chunkY] == 0) continue;
                else if (chunkCA.cells[chunkX, chunkY] == 1)
                    this.groundTilemap.SetTile(pos, ca.cells[i, j] == 0 ? groundTiles[0] : groundTiles[ca.cells[i, j] - 1]);
                else if (chunkCA.cells[chunkX, chunkY] == 2)
                    this.groundTilemap.SetTile(pos, ca.cells[i, j] == 0 ? forestTiles[0] : forestTiles[ca.cells[i, j] - 1]);
                else if (chunkCA.cells[chunkX, chunkY] == 3)
                    this.groundTilemap.SetTile(pos, ca.cells[i, j] == 0 ? mountainTiles[0] : mountainTiles[ca.cells[i, j] - 1]);*/

                if (landTileCA.cells[i, j] == 0) continue;
                else if (landTileCA.cells[i, j] > 0 && landTileCA.cells[i, j] < 4) groundTilemap.SetTile(pos, groundTiles[landTileCA.cells[i, j] - 1]);
                else if (landTileCA.cells[i, j] >= 4 && landTileCA.cells[i, j] < 7) groundTilemap.SetTile(pos, forestTiles[landTileCA.cells[i, j] - 4]);
                else if (landTileCA.cells[i, j] >= 7 && landTileCA.cells[i, j] < 10) groundTilemap.SetTile(pos, mountainTiles[landTileCA.cells[i, j] - 7]); // else ?

                if (houseCA.cells[i, j] > 0) overgroundTilemap.SetTile(pos, houseTiles[houseCA.cells[i, j] - 1]);

            }
        }

        Debug.Log("finished tile setting loop !");
    }

    private void generateWorld() {

        System.Random rnd = new System.Random();

        Debug.Log("starting island initialisation ...");

        for (int i = 0; i < islands.Length; i++) {
            islands[i] = new ProceduralIsland(
                islandData[i].location,
                chunkCA.getXsize(),
                rnd.Next(3, 10),
                rnd.Next(4, 7),
                new int[,] { } // add seeds here
            );
        }

        Debug.Log("finished island initialisation !");

        Debug.Log("starting island generation ...");

        chunkCA.clear();
        // smaller isles only get seeded after X iterations -> growthDelay
        for (int i = 0; i < chunkCellIterations; i++) {
            for (int n = 0; n < islands.Length; n++) {
                if (i == islands[n].growthDelay) {
                    for (int j = 0; j < islands[n].seeds.GetLength(0); j++) {
                        chunkCA.cells[(int)islands[n].location.x + islands[n].seeds[j, 0],
                                 (int)islands[n].location.y + islands[n].seeds[j, 1]] =
                                 islands[n].seeds[j, 2];
                    }
                }
            }
            chunkCA.update();
        }

        Debug.Log("finished island generation !");

        //removeLagoons(4);

        /*for (int i = 0; i < chunkCA.getXsize(); i++) {
            for (int j = 0; j < chunkCA.getYsize(); j++) {
                if (chunkCA.getLiveNeighbours(i, j) == 8 && UnityEngine.Random.value > 0.1f) {
                    chunkCA.cells[i, j] = 1;
                }
            }
        }*/

        Debug.Log("starting upscale to main CA ...");

        // interpolate the larger world chunks

        // set cells of main CA to chunkCA and scale

        for (int i = 0; i < landTileCA.getXsize(); i++) {
            for (int j = 0; j < landTileCA.getYsize(); j++) {
                int chunkX = i / blockSize;
                int chunkY = j / blockSize;
                //if(Mathf.PerlinNoise(i, j) > 0.5f) {
                if (chunkCA.cells[chunkX, chunkY] == 0) {
                    if (chunkCA.getLiveNeighbours(chunkX, chunkY) == 8 && UnityEngine.Random.value > 0.05f) {
                        chunkCA.cells[chunkX, chunkY] = 1;
                    } else {
                        continue;
                    }

                    if(UnityEngine.Random.value > 0.05f) {
                        landTileCA.cells[i, j] = (int)UnityEngine.Random.Range(0f, 9.99f);
                    }
                }

                if(UnityEngine.Random.value > 0.3f) {
                    landTileCA.cells[i, j] = (chunkCA.cells[chunkX, chunkY] * 3) + (int)UnityEngine.Random.Range(0f, 2.9999f);  // maybe distribute so that each large scale cell eg for ground has a mostly uniform value (of either 1, 2 or 3 in case of ground) that is mildly deviated from using perlin noise to the other cell vals for cell type (ground, forest, mountain)
                }
                //else {
                //    ca.cells[i, j] = (int)(1.0f + (Mathf.PerlinNoise(i, j) * 3.0f));
                //}
                    
            }
        }

        Debug.Log("finished upscale to main CA !");

        // then evolve to merge sections

        Debug.Log("starting iteration of main CA ...");

        landTileCA.update(upscaleCellIterations);

        Debug.Log("finished iteration of main CA !");

    }

    private void generateSettlements() {

        Debug.Log("starting settlement generation ...");

        int chunkX, chunkY;

        for (int i = 0; i < landTileCA.getXsize(); i++) {
            for (int j = 0; j < landTileCA.getYsize(); j++) {
                chunkX = i / blockSize;
                chunkY = j / blockSize;
                if (chunkCA.cells[chunkX, chunkY] == 1) {
                    houseCA.cells[i,j] = (int)UnityEngine.Random.Range(1f, 2.9999f);

                    if (UnityEngine.Random.value < 0.05f) {
                        GameObject denizen = Instantiate(denizenPrefab, new Vector3(i + UnityEngine.Random.Range(-5f, 5f), j + UnityEngine.Random.Range(-5f, 5f), 0), Quaternion.identity);
                        denizen.transform.parent = denizenParent.transform;
                    }
                }
            }
        }

        // 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 2, 0, 0, 0, 1, 0, 0, 0, 0, 0
        // 0, 0, 1, 0, 0, 0, 0, 0, 0, 2, 0, 2, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0
        houseCA.ruleSet = new int[] { 0, 0, 1, 0, 0, 0, 0, 0, 0, 2, 0, 2, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        //houseCA.setLambdaRuleset(0.4f);
        Debug.Log("House ruleset: " + string.Join(", ", houseCA.ruleSet));

        houseCA.update(5);

        Debug.Log("finished settlement generation !");
    }

    /*private void removeLagoons(int max) {   // doesn't work with neighbourTotals cuz that's from last ca iteration ...
        int[,] buffer = (int[,])ca.cells.Clone();
        for (int i = 0; i < ca.getXsize(); i++) {
            for (int j = 0; j < ca.getYsize(); j++) {
                if (ca.neighbourTotals[i, j] > max && ca.cells[i, j] > 0) buffer[i, j] = UnityEngine.Random.Range(1, ca.numStates - 1);
            }
        }
        ca.cells = (int[,])buffer.Clone();
    }*/
}
