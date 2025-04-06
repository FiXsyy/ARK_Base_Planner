using UnityEngine;
using UnityEngine.UIElements;

public class StartingBlockController : MonoBehaviour
{
    public VisualElement ui;
    public VisualElement selector;

    public Button triangleButton;
    public Button squareButton;

    public GameObject trianglePrefab;
    public GameObject squarePrefab;

    public int count = 0;

    private void Awake(){
        ui = GetComponent<UIDocument>().rootVisualElement;
    }

    private void OnEnable(){
        triangleButton = ui.Q<Button>("TriangleSelect");
        triangleButton.clicked += OnTriangleClicked;
        
        squareButton = ui.Q<Button>("SquareSelect");
        squareButton.clicked += OnSquareClicked;

        selector = GameObject.Find("Block Selection").GetComponent<UIDocument>().rootVisualElement;

        OnNoBlocks();
    }

    public void OnNoBlocks(){
        ui.visible = true;
        selector.visible = false;
    }

    private void OnTriangleClicked(){
        ui.visible = false;
        selector.visible = true;

        Instantiate(trianglePrefab, new Vector2(0, 0), Quaternion.identity);

        count++;    //  adds to block count
    }
    private void OnSquareClicked(){
        ui.visible = false;
        selector.visible = true;

        Instantiate(squarePrefab, new Vector2(0, 0), Quaternion.identity);

        count++;    //  adds to block count
    }
}
