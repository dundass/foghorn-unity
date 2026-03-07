using UnityEngine;

public class CA2D {
    public int numStates { get; }
    public int genCount { get; set; }
    public int[] ruleSet { get; set; }
    
    // Flattened arrays for speed
    private int[] cells;
    private int[] buffer;
    private int width;
    private int height;

    public CA2D(int num_states, int size) : this(num_states, size, size) { }

    public CA2D(int num_states, int x_size, int y_size) {
        numStates = num_states;
        width = x_size;
        height = y_size;
        
        cells = new int[width * height];
        buffer = new int[width * height];
        
        // Max possible total is (numStates - 1) * 8 neighbours
        ruleSet = new int[(numStates * 8) + 1];
        genCount = 0;
    }

    public void Update() {
        // Cache local references to avoid 'this' lookups
        int w = width;
        int h = height;
        int[] currentCells = cells;
        int[] nextCells = buffer;
        int[] rules = ruleSet;

        for (int y = 0; y < h; y++) {
            for (int x = 0; x < w; x++) {
                int total = 0;

                // Manual unrolling or simple loops for neighbours
                // This version handles wrapping (Toroidal)
                for (int ny = -1; ny <= 1; ny++) {
                    for (int nx = -1; nx <= 1; nx++) {
                        if (nx == 0 && ny == 0) continue;

                        // Fast wrapping logic
                        int ix = (x + nx + w) % w;
                        int iy = (y + ny + h) % h;
                        
                        total += currentCells[iy * w + ix];
                    }
                }

                // Direct lookup instead of a loop
                // Ensure total doesn't exceed ruleSet bounds
                nextCells[y * w + x] = (total < rules.Length) ? rules[total] : 0;
            }
        }

        // Fast pointer-like swap
        int[] temp = cells;
        cells = buffer;
        buffer = temp;
        
        genCount++;
    }

    public void Update(int iterations) {
        for(int i = 0; i < iterations; i++) Update();
    }

    // Helper to map 2D coordinates to flattened index
    private int GetIndex(int x, int y) => y * width + x;

    public int GetCell(int x, int y) => cells[GetIndex(x, y)];
    public void SetCell(int x, int y, int state) => cells[GetIndex(x, y)] = state;

    public void Clear() {
        System.Array.Clear(cells, 0, cells.Length);
    }

    public int GetXsize() {
        return width;
    }

    public int GetYsize() {
        return height;
    }

    public int GetLiveNeighbours(int x, int y) {
        int tot = 0;
        for(int i = -1; i < 2; i++) {
            for (int j = -1; j < 2; j++) {
                if (x + i < 0 || x + i >= GetXsize() || y + j < 0 || y + j >= GetYsize()) continue;
                if (cells[GetIndex(x + i, y + j)] > 0) tot++;
            }
        }
        return tot;
    }

    public void SetRandomStates(double p = 0.5d) {
        System.Random rnd = new(System.DateTime.Now.Millisecond);
        double r = 0;
        for (int i = 0; i < GetXsize(); i++) {
            for (int j = 0; j < GetYsize(); j++) {
                r = rnd.NextDouble();
                if (r < p) cells[GetIndex(i, j)] = 1 + rnd.Next(0, numStates - 1);
                else cells[GetIndex(i, j)] = 0;
            }
        }
    }

    public void SetLambdaRuleset(double p = 0.38d) {
        System.Random rnd = new(System.DateTime.Now.Millisecond);
        double r = 0;
        ruleSet[0] = 0;
        for (int i = 1; i < ruleSet.Length; i++) {
            r = rnd.NextDouble();
            if (r < p) {
                double roll = rnd.NextDouble();
                for (int j = 0; j < numStates - 1; j++) {
                    if (roll > (double)j / numStates && roll < (double)(j + 1) / numStates) ruleSet[i] = j + 1;
                }
            }
            else {
                ruleSet[i] = 0;
            }
        }
    }
}
