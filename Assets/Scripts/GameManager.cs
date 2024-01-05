using UnityEngine;

/// <summary>
/// Use this as an entry point for your game. Keep a reference to core game systems here.
/// </summary>
[DefaultExecutionOrder(-9999)]
public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public static GameManager Instance {
        get {
#if UNITY_EDITOR

            if (_instance == null)
            {
                //in editor, we can start any scene to test, so we are not sure the game manager will have been
                //created by the first scene starting the game. So we load it manually. This check is useless in
                //player build as the 1st scene will have created the GameManager so it will always exists.
                Instantiate(Resources.Load<GameManager>("GameManager"));
            }
#endif
            return _instance;
        }
    }

    // references to all the core systems here
    public PlayerStats PlayerStats { get; set; }
    public WorldClock WorldClock { get; set; }

    void Awake()
    {
        _instance = this;
    }

}
