using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralIsland {

    public string name { get; set; }
    public Vector2 location { get; }
    public int growthDelay { get; }
    private int lagoonThreshold { get; }
    private int gridSize;
    public Dictionary<Vector2Int, int> seeds { get; }
    private float maxSeedDist = 0.01f;
    
    public ProceduralIsland (Vector2 loc, int grid_size, int growth_delay, int lagoon_threshold, Dictionary<Vector2Int, int> seeds = null) {

        location = loc;
        //Debug.Log(loc.x + ", " + loc.y);
        gridSize = grid_size;
        growthDelay = growth_delay;
        lagoonThreshold = lagoon_threshold;
        this.seeds = seeds ?? new Dictionary<Vector2Int, int>();

        // TODO - temp rand seeding until data is there
        if (this.seeds.Count == 0) {
            System.Random rnd = new System.Random();
            int numSeeds = rnd.Next(10, 15);
            int caMax = 4;
            for (int i = 0; i < numSeeds; i++) {
                var seedPos = new Vector2Int(
                    rnd.Next((int)(gridSize * -maxSeedDist), (int)(gridSize * maxSeedDist)),
                    rnd.Next((int)(gridSize * -maxSeedDist), (int)(gridSize * maxSeedDist))
                );
                this.seeds[seedPos] = rnd.Next(1, caMax);
                //Debug.Log(seedPos.x + ", " + seedPos.y + ": " + this.seeds[seedPos]);
            }
        }

    }

}
