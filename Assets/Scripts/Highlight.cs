using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlight : MonoBehaviour
{
    // Assign all the renderers here through the inspector
    [SerializeField] private List<Renderer> renderers;
    [SerializeField] private Color color = Color.white;
    
    // helper list to cache all the materials of the object
    private List<Material> materials;


    private void Awake()
    {   
        materials = new List<Material>();

        foreach (var renderer in renderers) {
            // A single child-object might have multiple materials on it
            materials.AddRange(new List<Material>(renderer.materials));
        }
    }

    public void ToggleHighlight(bool val) {
        if (val) {
            foreach (var material in materials) {
                // We need to enable the emission and set the color
                material.EnableKeyword("_EMISSION");
                material.SetColor("_EmissionColor", color);
            }
        } else {
            foreach (var material in materials) {
                // disable the emission if we don't use the emission color anywhere else
                material.DisableKeyword("_EMISSION");
            }
        }
    }

    public void ToggleHighlightChoice(bool val, Color newcolor) {
        if (val) {
            foreach (var material in materials) {
                // We need to enable the emission and set the color
                material.EnableKeyword("_EMISSION");
                material.SetColor("_EmissionColor", newcolor);
            }
        } else {
            foreach (var material in materials) {
                // disable the emission if we don't use the emission color anywhere else
                material.DisableKeyword("_EMISSION");
            }
        }
    }
}
