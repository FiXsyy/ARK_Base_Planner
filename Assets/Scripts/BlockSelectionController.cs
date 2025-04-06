using UnityEngine;
using UnityEngine.UIElements;

public class BlockSelectionController : MonoBehaviour
{
    public VisualElement ui;

    public RadioButton triangleSelect;
    public RadioButton squareSelect;

    private void Awake(){
        ui = GetComponent<UIDocument>().rootVisualElement;
    }

    private void OnEnable(){
        triangleSelect = ui.Q<RadioButton>("TriangleSelect");
        squareSelect = ui.Q<RadioButton>("SquareSelect");
    }
}