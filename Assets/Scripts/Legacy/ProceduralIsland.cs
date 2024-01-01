using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralIsland {

    public string name { get; set; }
    public Vector2 location { get; }
    public int growthDelay { get; }
    private int lagoonThreshold { get; }
    private int gridSize;
    public int[,] seeds { get; }
    private float maxSeedDist = 0.01f;
    
    public ProceduralIsland (Vector2 loc, int grid_size, int growth_delay, int lagoon_threshold, int[,] seeds) {

        location = loc;
        //Debug.Log(loc.x + ", " + loc.y);
        gridSize = grid_size;
        growthDelay = growth_delay;
        lagoonThreshold = lagoon_threshold;
        this.seeds = seeds;

        // TODO - temp rand seeding until data is there
        System.Random rnd = new System.Random();
        int numSeeds = rnd.Next(10, 15), caMax = 4;
        this.seeds = new int[numSeeds, 3];
        for (int i = 0; i < numSeeds; i++) {
            // TODO - refactor seeds -> seeds[x][y] = seedVal;
            // or a list u can push {x: , y: , val: } to or sutin
            this.seeds[i,0] = rnd.Next((int)(gridSize * -maxSeedDist), (int)(gridSize * maxSeedDist));  //x
            this.seeds[i,1] = rnd.Next((int)(gridSize * -maxSeedDist), (int)(gridSize * maxSeedDist));  //y
            this.seeds[i,2] = rnd.Next(1, caMax);
            //Debug.Log(this.seeds[i, 0] + ", " + this.seeds[i, 1] + ": " + this.seeds[i,2]);
        }

    }

}
