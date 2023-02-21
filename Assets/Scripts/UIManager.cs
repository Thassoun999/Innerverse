using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{

    // ~ Instances, Variables, and Prefabs ~
    private static UIManager _instance;

    [SerializeField] public float alphaThreshold = 0.1f;
    public GameObject ActionBar; // 15 bars (the mushroom being the last bar!)
    public GameObject GameWheel;
    public GameObject ThornsIcon;
    public GameObject EndGamePanel;
    public GameObject HumanWin;
    public GameObject MyceliumWin;
    private Dictionary<string, GameObject> _GameWheelItems;

    private float[] actionBarFillAmount = new float[] { 0f, 0.335f, 0.382f, 0.429f, 0.48f, 0.527f, 0.574f, 0.621f, 0.669f, 0.716f, 0.764f, 0.815f, 0.858f, 0.909f, 0.957f, 1.0f };


    // ~ Properties ~

    public static UIManager Instance
    {
        get
        {
            if (_instance is null)
                Debug.LogError("UI Manager is NULL");

            return _instance;
        }
    }

    // ~ Methods ~

    void Awake()
    {
        if (_instance != null)
        { // need to ensure that this remains a singleton!
            Destroy(gameObject);
            return;
        }

        _instance = this;

        _GameWheelItems = new Dictionary<string, GameObject>();

        // Ensures that click radius is the image and not based on rectangle around mouse cursor (checks for alpha fields)
        // If transparent, does not click on anything -- even if the image is there
        // Here we're iterating through all child game objects of the Game Wheel to enable this feature
        foreach (Transform child in GameWheel.transform)
        {
            child.gameObject.GetComponent<Image>().alphaHitTestMinimumThreshold = alphaThreshold;
            _GameWheelItems[child.name] = child.gameObject;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        DisableGameWheel();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.GameOver)
        {
            if ((GameManager.Instance.PlayerTurn && GameManager.Instance.NoHumanMovement()))
                EndTurnButton(true);
        }
        else
        { // If the game's over don't accept any input!
            DisableGameWheel();
            EndTurnButton(false);
        }


        // Update how many action points we have left
        ActionBar.GetComponent<Image>().fillAmount = actionBarFillAmount[GameManager.Instance.ActionPoints];

        // Inform the player if they have the thorns passive or not
        if (GameManager.Instance.MyceliumCountBiome2 > 0)
        {
            ThornsIcon.GetComponent<Image>().color = new Color(1.0f, 0.53f, 0.76f, 1.0f);
        }
        else
        {
            ThornsIcon.GetComponent<Image>().color = new Color(0.0f, 0.0f, 0.0f, .7f);
        }
    }

    // Call this when selecting a Mycelium (also works for updating game wheel)
    public void EnableAndUpdateGameWheel()
    {
        // Don't enable the wheel if it's not your turn + if humans are still doing their animation
        if (!(GameManager.Instance.PlayerTurn && GameManager.Instance.NoHumanMovement()))
            return;

        int currActionPoints = GameManager.Instance.ActionPoints;

        // Reinforce = 3AP -- Mycelium only allowed to reinforce once! Adds temp health!
        if (currActionPoints >= 3 && GameManager.Instance.IsSelecting.Reinforced == false)
        {
            _GameWheelItems["Reinforce"].GetComponent<Button>().interactable = true;
        }
        else
        {
            _GameWheelItems["Reinforce"].GetComponent<Button>().interactable = false;
        }

        // Grow = 5AP
        if (currActionPoints >= 5 && GameManager.Instance.IsSelecting.ActionReady
            && GameManager.Instance.IsSelecting.GridSelect.Occupation == 0)
        {
            _GameWheelItems["Grow"].GetComponent<Button>().interactable = true;
        }
        else
        {
            _GameWheelItems["Grow"].GetComponent<Button>().interactable = false;
        }

        // Attack = 6AP
        if (currActionPoints >= 6 && GameManager.Instance.IsSelecting.ActionReady
            && (GameManager.Instance.IsSelecting.GridSelect.Occupation == 2 || GameManager.Instance.IsSelecting.GridSelect.Occupation == 3))
        {
            _GameWheelItems["Attack"].GetComponent<Button>().interactable = true;
        }
        else
        {
            _GameWheelItems["Attack"].GetComponent<Button>().interactable = false;
        }

        // Self Destruct = 15AP - This should only be toggled if there is a Mycelium on Special Biome 1
        if (currActionPoints == 15 && GameManager.Instance.MyceliumCountBiome1 > 0)
        {
            _GameWheelItems["Self Destruct"].GetComponent<Button>().interactable = true;
        }
        else
        {
            _GameWheelItems["Self Destruct"].GetComponent<Button>().interactable = false;
        }

        return;
    }

    public void DisableGameWheel()
    {
        _GameWheelItems["Reinforce"].GetComponent<Button>().interactable = false;
        _GameWheelItems["Grow"].GetComponent<Button>().interactable = false;
        _GameWheelItems["Attack"].GetComponent<Button>().interactable = false;
        _GameWheelItems["Self Destruct"].GetComponent<Button>().interactable = false;

        return;
    }

    public void EndTurnButton(bool value)
    {
        _GameWheelItems["End Turn"].GetComponent<Button>().interactable = value;
    }

    // These are to attach to buttons -- the IsSelecting param or GameManager allows us to target specific Mycelium for their actions
    public void Reinforce()
    {
        GameManager.Instance.IsSelecting.Reinforce();
    }

    public void Attack()
    {
        GameManager.Instance.IsSelecting.Attack();
    }

    public void SelfDestruct()
    {
        GameManager.Instance.IsSelecting.SelfDestruct();
    }

    public void Grow()
    {
        GameManager.Instance.IsSelecting.Grow();
    }

    public void EndGame(string outcome)
    {
        EndGamePanel.SetActive(true);
        if (outcome == "Mycelium")
        {
            Invoke("myceliumWin", 1.5f);
        }
        else if (outcome == "Human")
        {
            Invoke("humanWin", 1.5f);

        }
    }

    public void ResetInstance()
    {
        DisableGameWheel();
    }


    public void humanWin()
    {
        HumanWin.SetActive(true);
    }

    public void myceliumWin()
    {
        MyceliumWin.SetActive(true);
    }
}