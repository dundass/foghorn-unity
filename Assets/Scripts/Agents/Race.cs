using System.Collections;

public class Race {

    // for rendering -> modular textures -> https://www.youtube.com/watch?v=cIIaKdlZ4Cw or maybe further into the series

    public enum skinColours {
        red,
        blue,
        purple,
        green,
        grey
    }

    public enum hairColours {
        red,
        blue,
        purple,
        grey,
        brown,
        black,
        yellow,
        orange,
        tan
    }

    public string name { get; set; }

    public skinColours skinColour;
    public hairColours hairColour;

    public ProceduralIsland origin;   // y/n? it might be that a race is much older than the current arrangement of islands so maybe not this

    public Race(string name, skinColours skinColour, hairColours hairColour) {
        this.name = name;
        this.skinColour = skinColour;
        this.hairColour = hairColour;
    }

}
