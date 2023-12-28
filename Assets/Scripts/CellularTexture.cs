using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class CellularTexture : MonoBehaviour {

    CA2D ca;

    SpriteRenderer renderer;

    void Start() {
        ca = new CA2D(32, 32);
        ca.setLambdaRuleset();
        ca.setRandomStates();
        ca.update(3);
        renderer = GetComponent<SpriteRenderer>();
        Texture2D tex = new Texture2D(32, 32);
        Color color = new Color();
        for (int i = 0; i < tex.width; i++) {
            for (int j = 0; j < tex.height; j++) {
                color.r = ca.cells[i, j];
                color.g = 60 - ca.cells[i, j];
                color.b = 50 + (ca.cells[i, j] * 2);
                // todo - abstract the finer processing away to a collection of functions
                tex.SetPixel(i, j, color);
            }
        }
        tex.Apply();

        Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f);

        renderer.sprite = sprite;
    }

    void Update() {

    }
}
