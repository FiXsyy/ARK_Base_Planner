using System;
using UnityEngine;
using UnityEngine.EventSystems;

[Serializable] 
public class Drawer{
    public bool drawRays = false;
    public bool drawMainRay = true;
    public bool drawBorderRays = true;
}

public class SlotScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    SpriteRenderer spriteRenderer;   
    GameObject selectedObject;
    BlockScript selectedScript;
    GameObject startingBlock;
    StartingBlockController startingBlockScript;

    public Material defaultMat;
    public Material overlapMat;
    public GameObject squarePrefab;
    public GameObject trianglePrefab;

    private Vector2 rayDirection;
    private float rayDistance;
    private Vector2 startPos;

    private int shape;  //  triangle = 3, square = 4

    public Drawer drawer;
    
    //  Offset used for size of Raycasts
    [SerializeField] private float offset = 0.9f;

    //  Distance from edge to the center of mass of a triangle
    [HideInInspector] public float centerMassDistance = Mathf.Sqrt(3) / 6f;

    void Awake(){
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        startingBlock = GameObject.Find("Starting Block");
        startingBlockScript = startingBlock.GetComponent<StartingBlockController>();

        shape = GetComponent<PolygonCollider2D>() != null ? 3 : 4;
    }

    void Start(){
        selectedObject = GameObject.FindGameObjectWithTag("Selected");
        selectedScript = selectedObject.GetComponent<BlockScript>();

        rayDirection = new Vector2(transform.position.x - selectedObject.transform.position.x, transform.position.y - selectedObject.transform.position.y).normalized;
        startPos = new Vector2(transform.position.x, transform.position.y) - rayDirection * (shape == 4 ? 0.5f : centerMassDistance) + rayDirection.Rotate(90) * 0.5f + (1 - offset) * (((shape == 4 ? 0.5f : centerMassDistance) * rayDirection) + 0.5f * rayDirection.Rotate(-90));

        DestroyOnOverlap();
    }

    //  Draws border rays, and a center line ray   |   for debug purposes
    void FixedUpdate(){
        if(drawer.drawRays == true){
            DrawOverlapRays(drawer.drawMainRay, drawer.drawBorderRays);
        }
    }

    //  Makes the slots visible on hover, and hides them otherwise
    public void OnPointerEnter(PointerEventData eventData){
        spriteRenderer.enabled = true;
    }
    public void OnPointerExit(PointerEventData eventData){
        spriteRenderer.enabled = false;
    }

    //  Creates a new square on click, destroys all plots, and unselects the selected block
    public void OnPointerClick(PointerEventData eventData){
        if(selectedObject){
            Instantiate(shape == 3 ? trianglePrefab : squarePrefab, transform.position, transform.rotation);

            startingBlockScript.count++;    //  adds to block count

            selectedScript.DestroyPlots();
            selectedScript.UnselectBlock();
        }
    }

    /// <summary>
    /// Checks if an overlap is present on ray hit.
    /// </summary>
    private void CheckOverlap(RaycastHit2D[] rayHit){
        foreach(var item in rayHit){
            if(item){
                Destroy(gameObject);
            }
        }
    }

    /// <summary>
    /// Sends raycasts and destroys the overlapping slot on RaycastHit.
    /// </summary>
    private void DestroyOnOverlap(){
        rayDistance = offset * (shape == 4 ? 0.5f : centerMassDistance);
        Vector2 startPosRay = startPos;

        //  Sends center line raycasts and checks for hit
        RaycastHit2D[] hit = Physics2D.RaycastAll(transform.position - new Vector3(rayDirection.x, rayDirection.y) * rayDistance, rayDirection, shape == 4 ? 2 * rayDistance : 3 * rayDistance);

        CheckOverlap(hit);
        
        //  Sends raycasts in the shape of a border and checks for hits
        if(shape == 4){
            for(int i = 0; i < shape; ++i){
                startPosRay += i != 0 ? rayDirection.Rotate(-90 * (i - 1)) * offset : Vector2.zero;

                RaycastHit2D[] border = Physics2D.RaycastAll(startPosRay, rayDirection.Rotate(-90 * i), offset);

                CheckOverlap(border);
            }
        }
        else if(shape == 3){
            for(int i = 0; i < shape; ++i){
                startPosRay += i != 0 ? rayDirection.Rotate(-30 - 120 * (i - 1)) * offset : Vector2.zero;

                RaycastHit2D[] border = Physics2D.RaycastAll(startPosRay, rayDirection.Rotate(-30 - 120 * i), offset);

                CheckOverlap(border);
            }
        }
    }

    /// <summary>
    /// Draws border rays dependant on a specified shape of the plot.
    /// </summary>
    private void DrawBorderRays(int shape){
        Vector2 startPosDraw = startPos;

        if(shape == 4){
            //  Draws border rays of a square plot
            for(int i = 0; i < shape; ++i){
                startPosDraw += i != 0 ? rayDirection.Rotate(-90 * (i - 1)) * offset : Vector2.zero;

                Debug.DrawRay(startPosDraw, rayDirection.Rotate(-90 * i) * offset, Color.white);
            }
        }
        else if(shape == 3){
            //  Draws border rays of a triangle plot
            for(int i = 0; i < shape; ++i){
                startPosDraw += i != 0 ? rayDirection.Rotate(-30 - 120 * (i - 1)) * offset : Vector2.zero;

                Debug.DrawRay(startPosDraw, rayDirection.Rotate(-30 - 120 * i) * offset, Color.white);
            }
        }
    }

    /// <summary>
    /// Draws border (togglable) and center line rays.
    /// Should be used with FixedUpdate function.
    /// </summary>
    private void DrawOverlapRays(bool mainRay, bool borderRays){
        if(mainRay == true)
            Debug.DrawRay(transform.position - new Vector3(rayDirection.x, rayDirection.y) * rayDistance, rayDirection * (shape == 4 ? 2 * rayDistance : 3 * rayDistance), Color.green);
        if(borderRays == true){
            if(shape == 4){
                DrawBorderRays(shape);
            }
            else if(shape == 3){
                DrawBorderRays(shape);
            }
        }
    }
}

public static class Extensions{
    /// <summary>
    /// Rotates a Vector2 by a specified angle.
    /// </summary>
    public static Vector2 Rotate(this Vector2 vector, float angle){
        if(angle == 0){
            return vector;
        }
        else if(angle == 90){
            return new Vector2(-vector.y, vector.x);
        }
        else if(angle == -90){
            return new Vector2(vector.y, -vector.x);
        }
        else{
            return Quaternion.AngleAxis(angle, Vector3.forward) * vector;
        }
    }
}