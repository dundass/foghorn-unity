using System.Collections.Generic;
using UnityEngine;

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
    public bool generateSettlements = false;

    public WorldRenderer worldRenderer;

    public GameObject overgroundLights;

    public GameObject denizenParent;
    public GameObject denizenPrefab;

    public ProceduralIsland[] islands;
    public Transform player;

    public enum BlockType {
        ground = 1,
        forest = 2,
        mountain = 3
    }

    public CA2D landCA, houseCA, chunkCA;

    private Dictionary<string, int[]> rulesets = new Dictionary<string, int[]> {
        { "island", new int[] { 0, 0, 3, 0, 3, 0, 2, 0, 3, 3, 3, 2, 3, 0, 3, 0, 0, 1, 2, 0, 0, 2, 2, 0, 1, 3, 0, 0, 0, 0, 3, 0 } },
        { "island2", new int[] { 0, 1, 0, 1, 0, 0, 1, 3, 3, 0, 1, 0, 2, 3, 0, 0, 3, 1, 0, 2, 3, 3, 0, 3, 0, 2, 0, 0, 3, 3, 0, 0 } },
        { "island3", new int[] { 0, 0, 3, 0, 3, 0, 2, 0, 3, 3, 3, 2, 3, 0, 3, 0, 0, 1, 2, 0, 0, 2, 2, 0, 1, 3, 0, 0, 0, 0, 3, 0 } },
        { "ground", new int[] { 0, 0, 0, 1, 2, 0, 0, 3, 0, 1, 0, 0, 1, 0, 0, 0, 0, 2, 0, 0, 1, 1, 3, 0, 0, 0, 0, 0, 3, 0, 3, 1 } },
        { "forest", new int[] { 0, 0, 0, 1, 0, 0, 1, 3, 3, 0, 1, 0, 2, 3, 0, 0, 3, 1, 0, 2, 3, 3, 0, 3, 3, 2, 0, 0, 3, 3, 0, 0 } },
        { "mountain", new int[] { 0, 0, 3, 0, 3, 0, 2, 0, 3, 3, 3, 2, 3, 0, 3, 0, 0, 1, 2, 0, 0, 2, 2, 0, 1, 3, 0, 0, 0, 0, 3, 0 } },
        // other for specific features eg ruins
        { "land", new int[] { 0, 1, 1, 1, 2, 9, 1, 4, 1, 2, 2, 1, 2, 1, 2, 2, 2, 2, 1, 2, 2, 3, 2, 3, 3, 4, 3, 3, 4, 4, 4, 1, 4, 5, 4, 5, 4, 4, 5, 2, 5, 5, 4, 0, 5, 4, 4, 4, 6, 6, 6, 6, 7, 6, 6, 7, 7, 8, 4, 8, 7, 9, 7, 8, 8, 7, 8, 8, 7, 8, 8, 4, 9, 9, 9, 7, 8, 7, 9, 7 } },
        { "land2", new int[] { 0, 0, 0, 0, 0, 0, 1, 0, 1, 2, 2, 0, 2, 1, 2, 2, 2, 2, 0, 0, 0, 0, 0, 3, 3, 0, 3, 0, 4, 0, 0, 0, 4, 5, 4, 0, 4, 4, 5, 0, 5, 5, 0, 0, 5, 0, 0, 0, 6, 6, 6, 6, 7, 6, 6, 7, 7, 0, 0, 0, 0, 0, 7, 0, 8, 0, 8, 0, 0, 8, 8, 0, 9, 9, 9, 0, 0, 0, 9, 0 } },
        { "land3", new int[] { 0, 0, 0, 0, 0, 0, 0, 5, 1, 8, 0, 0, 8, 5, 3, 0, 2, 0, 0, 3, 1, 3, 0, 0, 3, 0, 0, 0, 3, 0, 4, 6, 4, 0, 0, 3, 7, 0, 0, 0, 5, 1, 9, 0, 0, 6, 0, 0, 6, 0, 4, 4, 4, 0, 1, 0, 7, 0, 0, 0, 0, 0, 0, 0, 8, 0, 0, 0, 0, 0, 0, 0, 9, 0, 7, 0, 0, 9, 0, 0 } },
        { "land4", new int[] {0,3,0,0,0,0,7,1,1,3,0,3,0,2,0,1,2,2,0,3,0,0,0,4,3,3,9,0,0,0,5,4,4,4,0,0,0,0,0,6,5,4,0,0,6,0,0,6,6,6,2,6,0,0,0,5,7,6,0,0,0,2,0,7,8,7,0,3,2,2,6,8,9,9,0,0,2,1,5,0 } },

        { "house", new int[] { 0, 0, 1, 0, 0, 0, 0, 0, 0, 2, 0, 2, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
    };

    private int blockSize = 10;

    // Start is called before the first frame update
    void Start() {

        // stores all terra tile states
        landCA = new CA2D(10, 384 * blockSize) {
            ruleSet = rulesets["land"]
        };

        // set all rules for uniformly live neighbourhoods to stay at that total
        for (int i = 0; i < landCA.numStates; i++) landCA.ruleSet[i * 8] = i;
        //Debug.Log(string.Join(", ", ca.ruleSet));

        houseCA = new CA2D(3, 384 * blockSize);

        // stores all world/island chunk states
        chunkCA = new CA2D(4, 384) {
            ruleSet = rulesets["island"]
        };

        islands = new ProceduralIsland[30];

        GenerateWorld();

        if (generateSettlements)
            GenerateSettlements(); // todo - this adds 40s+ to startup ...

        worldRenderer.RenderWorld(landCA, houseCA);

        player.position = new Vector3(islands[0].location.x * blockSize, islands[0].location.y * blockSize, 0);
    }

    private void GenerateWorld() {

        System.Random rnd = new System.Random();

        Debug.Log("starting island initialisation ...");

        for (int i = 0; i < islands.Length; i++) {
            islands[i] = new ProceduralIsland(
                islandData[i].location,
                chunkCA.GetXsize(),
                rnd.Next(4, 10),
                rnd.Next(4, 7)
                // add seeds here
            );
        }

        Debug.Log("finished island initialisation !");

        Debug.Log("starting island generation ...");

        chunkCA.Clear();
        // smaller isles only get seeded after X iterations -> growthDelay
        for (int i = 0; i < chunkCellIterations; i++) {
            for (int n = 0; n < islands.Length; n++) {
                if (i == islands[n].growthDelay) {
                    foreach (var seed in islands[n].seeds) {
                        int x = (int)islands[n].location.x + seed.Key.x;
                        int y = (int)islands[n].location.y + seed.Key.y;
                        chunkCA.cells[x, y] = seed.Value;
                    }
                }
            }
            chunkCA.Update();
        }

        Debug.Log("finished island generation !");

        //RemoveLagoons(4);

        /*for (int i = 0; i < chunkCA.GetXsize(); i++) {
            for (int j = 0; j < chunkCA.GetYsize(); j++) {
                if (chunkCA.GetLiveNeighbours(i, j) == 8 && UnityEngine.Random.value > 0.1f) {
                    chunkCA.cells[i, j] = 1;
                }
            }
        }*/

        Debug.Log("starting upscale to main CA ...");

        // interpolate the larger world chunks

        // set cells of land CA to chunkCA and scale

        for (int i = 0; i < landCA.GetXsize(); i++) {
            for (int j = 0; j < landCA.GetYsize(); j++) {
                int chunkX = i / blockSize;
                int chunkY = j / blockSize;
                //if(Mathf.PerlinNoise(i, j) > 0.5f) {
                if (chunkCA.cells[chunkX, chunkY] == 0) {
                    // remove some lagoons
                    if (chunkCA.GetLiveNeighbours(chunkX, chunkY) == 8 && UnityEngine.Random.value > 0.05f) {
                        chunkCA.cells[chunkX, chunkY] = 1;
                    } else {
                        continue;
                    }

                    // add some noise to the ocean tiles to add variation
                    if(UnityEngine.Random.value > 0.05f) {
                        landCA.cells[i, j] = UnityEngine.Random.Range(0, landCA.numStates);
                    }
                }

                // set the land tile CA cells to the chunk CA cell value * 3 (number of land tile types) + a random number from 0 to 2 
                if(UnityEngine.Random.value > 0.3f) {
                    landCA.cells[i, j] = (chunkCA.cells[chunkX, chunkY] * 3) + UnityEngine.Random.Range(0, 3);  // maybe distribute so that each large scale cell eg for ground has a mostly uniform value (of either 1, 2 or 3 in case of ground) that is mildly deviated from using perlin noise to the other cell vals for cell type (ground, forest, mountain)
                }
                //else {
                //    ca.cells[i, j] = (int)(1.0f + (Mathf.PerlinNoise(i, j) * 3.0f));
                //}
                    
            }
        }

        Debug.Log("finished upscale to main CA !");

        // then evolve to merge sections

        Debug.Log("starting iteration of main CA ...");

        landCA.Update(upscaleCellIterations);

        Debug.Log("finished iteration of main CA !");

    }

    private void GenerateSettlements() {

        Debug.Log("starting settlement generation ...");

        int chunkX, chunkY;

        for (int i = 0; i < landCA.GetXsize(); i++) {
            for (int j = 0; j < landCA.GetYsize(); j++) {
                chunkX = i / blockSize;
                chunkY = j / blockSize;
                if (chunkCA.cells[chunkX, chunkY] == 1) {
                    houseCA.cells[i,j] = UnityEngine.Random.Range(1, 3); // 1 or 2

                    if (UnityEngine.Random.value < 0.05f) {
                        GameObject denizen = Instantiate(denizenPrefab, new Vector3(i + UnityEngine.Random.Range(-5f, 5f), j + UnityEngine.Random.Range(-5f, 5f), 0), Quaternion.identity);
                        denizen.transform.parent = denizenParent.transform;
                    }
                }
            }
        }

        // 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 2, 0, 0, 0, 1, 0, 0, 0, 0, 0
        // 0, 0, 1, 0, 0, 0, 0, 0, 0, 2, 0, 2, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0
        houseCA.ruleSet = rulesets["house"];
        //houseCA.SetLambdaRuleset(0.4f);
        //Debug.Log("House ruleset: " + string.Join(", ", houseCA.ruleSet));

        houseCA.Update(5);

        Debug.Log("finished settlement generation !");
    }

    /*private void RemoveLagoons(int max) {   // doesn't work with neighbourTotals cuz that's from last ca iteration ...
        int[,] buffer = (int[,])ca.cells.Clone();
        for (int i = 0; i < ca.GetXsize(); i++) {
            for (int j = 0; j < ca.GetYsize(); j++) {
                if (ca.neighbourTotals[i, j] > max && ca.cells[i, j] > 0) buffer[i, j] = UnityEngine.Random.Range(1, ca.numStates - 1);
            }
        }
        ca.cells = (int[,])buffer.Clone();
    }*/
}
