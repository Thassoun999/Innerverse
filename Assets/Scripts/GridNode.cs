using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridNode : MonoBehaviour {

    // ~ Instance and Variables ~

    // change variables to private later
    public int row;
    public int col;
    public int occupation = 0; // 0 for None, 1 for Mycelium, 2 for Human, 3 for Settlement
    private bool selected = false;
    private bool clickable = false;

    private Highlight gridHighlight; 

    private Mycelium myceliumSelect = null;

    public GameObject standing = null; // this will help with the special biome classification

    // Special Grid properties
    public int specialClassifier = 0; // Default to 0, 1 for biome 1, 2 for biome 2

    public int[] Coordinates {
        get {
            return new int[] {row, col};
        }
    }

    public int SpecialClassifier {
        get {
            return specialClassifier;
        }
    }

    public Mycelium MyceliumSelect {
        set {
            myceliumSelect = value;
        }
    }

    public Highlight GridHighlight {
        get {
            return gridHighlight;
        }
    }

    public int Occupation {
        get {
            return occupation;
        }
        set {
            if (!(value == 0 || value == 1 || value == 2 || value == 3))
                Debug.LogError("Occupation Error: Put in a correct value (0, 1, 2, 3).");
            
            occupation = value;
        }
    }

    public bool Clickable {
        get {
            return clickable;
        }
        set {
            clickable = value;
        }
    }

    public GameObject Standing {
        get {
            return standing;
        }
        set {
            standing = value;
        }
    }

    // ~ Methods ~

    void Awake()
    {
        gridHighlight = gameObject.GetComponent<Highlight>();
    }

    void Start()
    {
        row = (int)transform.localPosition.x;
        col = (int)transform.localPosition.z;
    }

    void OnMouseDown()
    {
        if(clickable) {
            if (!selected) {
                // From here we need to send the grid coordinates to Mycelium and allow it to pick an action!
                // This is where we pull up the wheel!
                if (myceliumSelect.GridSelect != null) {
                    if(myceliumSelect.GridSelect.Occupation == 0) {
                        myceliumSelect.GridSelect.GridHighlight.ToggleHighlightChoice(true, Color.green);
                    } else if (myceliumSelect.GridSelect.Occupation == 2 || myceliumSelect.GridSelect.Occupation == 3) {
                        myceliumSelect.GridSelect.GridHighlight.ToggleHighlightChoice(true, Color.red);
                    }
                }

                myceliumSelect.GridSelect = this;
                myceliumSelect.ActionReady = true;
                selected = true;
                gridHighlight.ToggleHighlightChoice(true, Color.blue);

            } else {
                // Disallow Mycelium from selecting an action and clear the coordinates it has previously received
                myceliumSelect.GridSelect = null;
                myceliumSelect.ActionReady = false;
                selected = false;

                if (occupation == 0){
                    GridHighlight.ToggleHighlightChoice(true, Color.green);
                } else if (occupation == 2 || occupation == 3){
                    GridHighlight.ToggleHighlightChoice(true, Color.red);

                }
            }
        }
    }

    public void OnMouseDownHumCall() {
        if(clickable) {
            if (!selected) {

                // Turn off the highlight on previous select if there is one
                if (myceliumSelect.GridSelect != null) {
                    if (myceliumSelect.GridSelect.Occupation == 0){
                        GridHighlight.ToggleHighlightChoice(true, Color.green);
                    } else if (myceliumSelect.GridSelect.Occupation == 2 || myceliumSelect.GridSelect.Occupation == 3){
                        GridHighlight.ToggleHighlightChoice(true, Color.red);

                    }
                }

                // From here we need to send the grid coordinates to Mycelium and allow it to pick an action!
                // This is where we pull up the wheel!
                myceliumSelect.GridSelect = this;
                myceliumSelect.ActionReady = true;
                selected = true;
                gridHighlight.ToggleHighlightChoice(true, Color.blue);

            } else {
                // Disallow Mycelium from selecting an action and clear the coordinates it has previously received
                myceliumSelect.GridSelect = null;
                myceliumSelect.ActionReady = false;
                selected = false;

                if (occupation == 0){
                    GridHighlight.ToggleHighlightChoice(true, Color.green);
                } else if (occupation == 2 || occupation == 3){
                    GridHighlight.ToggleHighlightChoice(true, Color.red);

                }
            }
        }
    }

    // Fix multiple blue grids
}