using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridNode : MonoBehaviour {

    // ~ Instance and Variables ~

    public int row;
    public int col;
    public int occupation = 0; // 0 for None, 1 for Mycelium, 2 for Human
    public bool selected = false;
    public bool clickable = false;

    private Highlight gridHighlight; 

    private Mycelium myceliumSelect = null;

    private GameObject standing = null;

    // ~ Properties ~

    public int[] Coordinates {
        get {
            return new int[] {row, col};
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
            if (!(value == 0 || value == 1 || value == 2))
                Debug.LogError("Occupation Error: Put in a correct value (0, 1, 2).");
            
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
                } else if (occupation == 2){
                    GridHighlight.ToggleHighlightChoice(true, Color.red);

                }
            }
        }
    }

    public void OnMouseDownHumCall() {
        if(clickable) {
            if (!selected) {
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
                } else if (occupation == 2){
                    GridHighlight.ToggleHighlightChoice(true, Color.red);

                }
            }
        }
    }
}