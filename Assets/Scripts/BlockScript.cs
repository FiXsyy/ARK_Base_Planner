using UnityEngine;
using UnityEngine.EventSystems;

public class BlockScript : MonoBehaviour, IPointerClickHandler
{
    SpriteRenderer spriteRenderer;
    GameObject selectionObject;
    BlockSelectionController selectionScript;
    GameObject startingBlock;
    StartingBlockController startingBlockScript;
    
    public Material defaultMat;
    public Material selectedMat;
    public GameObject triangleSlotPrefab;
    public GameObject squareSlotPrefab;

    [HideInInspector] public int shape;     //  triangle = 3, square = 4

    //  Distance from edge to the center of mass of a triangle
    [HideInInspector] public float centerMassDistance = Mathf.Sqrt(3) / 6f;
    
    void Awake(){
        spriteRenderer = GetComponent<SpriteRenderer>();

        selectionObject = GameObject.Find("Block Selection");
        selectionScript = selectionObject.GetComponent<BlockSelectionController>();
        
        startingBlock = GameObject.Find("Starting Block");
        startingBlockScript = startingBlock.GetComponent<StartingBlockController>();

        shape = GetComponent<PolygonCollider2D>() != null ? 3 : 4;
    }

    public void OnPointerClick(PointerEventData eventData){
        if(eventData.button == PointerEventData.InputButton.Left){
            //  Select block and spawn plots around it on left click
            if(spriteRenderer.sharedMaterial == defaultMat){
                //  If a selected block exists, select the one clicked instead
                if(GameObject.FindGameObjectWithTag("Selected")){
                    BlockScript selectedScript = GameObject.FindGameObjectWithTag("Selected").GetComponent<BlockScript>();

                    selectedScript.DestroyPlots();
                    selectedScript.UnselectBlock();
                }

                SpawnPlots();
            }
            //  Unselect block and destroy plots on repeated click
            else if(spriteRenderer.sharedMaterial == selectedMat){
                DestroyPlots();
                UnselectBlock();
            }
        }
        //  Destroy the block on right click
        else if(eventData.button == PointerEventData.InputButton.Right && spriteRenderer.sharedMaterial == defaultMat){
            Destroy(gameObject);
            
            startingBlockScript.count--;    //  substracts from block count
            
            //  Tells the Select Starting Block UI to pop up if there are no blocks left
            if(startingBlockScript.count == 0){
                startingBlockScript.OnNoBlocks();
            }
        }
    }

    /// <summary>
    /// Instantiates <paramref name="n"/> plots adjacent to each side of a block.
    /// Shape of plots depends on <paramref name="isPlotSquare"/> value.
    /// </summary>
    private void SpawnAndMove(int n, bool isPlotSquare){
        GameObject prefab;  //  Declaring a temporary gameobject so we don't need to do it every loop later  
        float offset = (isPlotSquare == true ? 0.5f : centerMassDistance) + (n == 4 ? 0.5f : centerMassDistance);   //  Offset depending on shapes used

        for(int i = 0; i < n; ++i){
            //  Instantiates n plots, rotates them so each faces different direction, and moves them by an offset
            prefab = Instantiate(isPlotSquare == true ? squareSlotPrefab : triangleSlotPrefab, transform.position, Quaternion.Euler(0, 0, i * 360 / n + 180 + transform.rotation.eulerAngles.z));            
            prefab.transform.localPosition += prefab.transform.up * offset;
        }

        spriteRenderer.sharedMaterial = selectedMat;
        transform.gameObject.tag = "Selected";
    }

    /// <summary>
    /// Destroys every plot.
    /// </summary>
    public void DestroyPlots(){
        GameObject[] slots = GameObject.FindGameObjectsWithTag("Destroy");
        foreach(GameObject obj in slots) Destroy(obj);
    }

    /// <summary>
    /// Unselects a block.
    /// </summary>
    public void UnselectBlock(){
        spriteRenderer.sharedMaterial = defaultMat;
        transform.gameObject.tag = "Unselected";
    }

    /// <summary>
    /// Spawns plots.
    /// </summary>
    public void SpawnPlots(){
        if(selectionScript.squareSelect.value == true){
            SpawnAndMove(shape, true);
        }
        else if(selectionScript.triangleSelect.value == true){
            SpawnAndMove(shape, false);
        }
    }
}
