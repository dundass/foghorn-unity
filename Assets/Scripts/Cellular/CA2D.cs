public class CA2D {

    public int numStates { get; }
    public int genCount { get; set; }
    public int[] ruleSet { get; set; }
    public int[,] cells { get; set; }
    private int[,] buffer;
    public int[,] neighbourTotals;

    public CA2D(int num_states, int size) : this(num_states, size, size) {}

    public CA2D(int num_states, int x_size, int y_size) {

        numStates = num_states;
        cells = new int[x_size, y_size];
        buffer = new int[x_size, y_size];
        neighbourTotals = new int[x_size, y_size];
        ruleSet = new int[num_states * 8];
        genCount = 0;

    }

    public void Update() {
        int xSize = GetXsize();
        int ySize = GetYsize();
        int tot = 0;

        int i = 0, j = 0, k = 0, l = 0, m = 0, n = 0;

        for (i = 0; i < xSize; i++) {
            for (j = 0; j < ySize; j++) {
                tot = 0;
                // iterate over kernel
                for (k = i - 1; k < i + 2; k++) {
                    for (l = j - 1; l < j + 2; l++) {
                        if (k < 0) m = xSize - 1;
                        else m = k % xSize;
                        if (l < 0) n = ySize - 1;
                        else n = l % ySize;
                        if (!(i == k && j == l)) tot += cells[m, n];
                    }
                }
                neighbourTotals[i, j] = tot;
                for (int r = 0; r < ruleSet.Length; r++) {
                    if (tot == r) {
                        buffer[i, j] = ruleSet[r];
                        break;
                    }
                }
            }
        }
        (cells, buffer) = (buffer, cells);
        genCount++;
    }

    public void Update(int iterations) {
        for(int i = 0; i < iterations; i++) Update();
    }

    public int GetXsize() {
        return cells.GetLength(0);
    }

    public int GetYsize() {
        return cells.GetLength(1);
    }

    public int GetLiveNeighbours(int x, int y) {
        int tot = 0;
        for(int i = -1; i < 2; i++) {
            for (int j = -1; j < 2; j++) {
                if (x + i < 0 || x + i >= GetXsize() || y + j < 0 || y + j >= GetYsize()) continue;
                if (cells[x + i, y + j] > 0) tot++;
            }
        }
        return tot;
    }

    public void SetRandomStates(double p = 0.5d) {
        System.Random rnd = new System.Random(System.DateTime.Now.Millisecond);
        double r = 0;
        for (int i = 0; i < GetXsize(); i++) {
            for (int j = 0; j < GetYsize(); j++) {
                r = rnd.NextDouble();
                if (r < p) cells[i,j] = 1 + rnd.Next(0, numStates - 1);
                else cells[i,j] = 0;
            }
        }
    }

    public void SetLambdaRuleset(double p = 0.38d) {
        System.Random rnd = new System.Random(System.DateTime.Now.Millisecond);
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

    public void Clear() {
        for (int i = 0; i < GetXsize(); i++) {
            for (int j = 0; j < GetYsize(); j++) {
                cells[i, j] = 0;
            }
        }
    }
}
