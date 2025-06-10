using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingGridPlacer : BuildingPlacer
{
    public float cellSize;
    public Vector2 gridOffset;
    public Renderer gridRenderer;

    //-----------------

    [SerializeField]
    private GameObject cellIndicator;
    [SerializeField]
    private Grid grid;

    //-----------------

//#if UNITY_EDITOR
//    private void OnValidate()
//    {
//        _UpdateGridVisual();
//    }
//#endif

    //private void Start()
    //{
    //    _UpdateGridVisual();
    //    _EnableGridVisual(false);
    //}

    private void Update()
    {
        if (_buildingPrefab == null || _toBuild == null)
            return;

        if (Input.GetMouseButtonDown(1))
        {
            Destroy(_toBuild);
            _toBuild = null;
            _buildingPrefab = null;
            return;
        }

        if (EventSystem.current.IsPointerOverGameObject(-1))
        {
            _toBuild.SetActive(false);
            return;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            _toBuild.transform.Rotate(0f, 0f, 90f);
        }

        Vector2 mouseWorldPos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        

        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero, 0f, groundLayerMask);
        if (hit.collider != null)
        {
            _toBuild.SetActive(true);
            _toBuild.transform.position = _ClampToNearest(hit.point, cellSize);


            if (Input.GetMouseButtonDown(0))
            {

                BuildingManager m = _toBuild.GetComponent<BuildingManager>();
                if (m.hasValidPlacement)
                {
                    m.SetPlacementMode(PlacementMode.Fixed);

                    //_buildingPrefab = null; //place only once

                    _toBuild = null;
                    _PrepareBuilding();
                }
            }
        }
        else
        {
            _toBuild.SetActive(false);
        }
    }


    //protected override void _PrepareBuilding()
    //{
    //    base._PrepareBuilding();
    //    //_EnableGridVisual(true);
    //}

    private Vector3 _ClampToNearest(Vector3 pos, float threshold)
    {
        float t = 1f / threshold;
        Vector3 v = ((Vector3)Vector3Int.FloorToInt(pos * t)) / t;
        float s = threshold * 0.5f;
        v.x += s + gridOffset.x; // (recenter in middle of cells)
        v.y += s + gridOffset.y; 
        return v;
    }

    //private void _EnableGridVisual(bool on)
    //{
    //    if (gridRenderer == null) return;
    //    gridRenderer.gameObject.SetActive(on);
    //}

    //private void _UpdateGridVisual()
    //{
    //    if (gridRenderer == null) return;
    //    gridRenderer.sharedMaterial.SetVector(
    //        "_Cell_Size", new Vector4(cellSize, cellSize, 0, 0));
    //}

}

//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.EventSystems;

//public class BuildingGridPlacer : BuildingPlacer
//{
//    public float cellSize;
//    public Vector2 gridOffset;
//    public Renderer gridRenderer;

//    [Header("Gizmo Settings")]
//    public bool drawGizmos = true;
//    public Color gizmoColor = new Color(0.5f, 0.5f, 0.5f, 0.3f);
//    public Vector2Int gridSize = new Vector2Int(10, 10);
//    public bool drawInEditMode = true;
//    public bool drawInPlayMode = true;

//#if UNITY_EDITOR
//    private void OnValidate()
//    {
//        _UpdateGridVisual();
//    }
//#endif

//    private void Start()
//    {
//        _UpdateGridVisual();
//        _EnableGridVisual(false);
//    }

//    private void Update()
//    {
//        if (_buildingPrefab == null || _toBuild == null)
//            return;

//        if (Input.GetMouseButtonDown(1))
//        {
//            Destroy(_toBuild);
//            _toBuild = null;
//            _buildingPrefab = null;
//            return;
//        }

//        if (EventSystem.current.IsPointerOverGameObject(-1))
//        {
//            _toBuild.SetActive(false);
//            return;
//        }

//        if (Input.GetKeyDown(KeyCode.R))
//        {
//            _toBuild.transform.Rotate(0f, 0f, 90f);
//        }

//        Vector2 mouseWorldPos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);

//        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero, 0f, groundLayerMask);
//        if (hit.collider != null)
//        {
//            _toBuild.SetActive(true);
//            _toBuild.transform.position = _ClampToNearest(hit.point, cellSize);


//            if (Input.GetMouseButtonDown(0))
//            {
//                BuildingManager m = _toBuild.GetComponent<BuildingManager>();
//                if (m.hasValidPlacement)
//                {
//                    m.SetPlacementMode(PlacementMode.Fixed);
//                    _toBuild = null;
//                    _PrepareBuilding();
//                }
//            }
//        }
//        else
//        {
//            _toBuild.SetActive(false);
//        }
//    }

//    protected override void _PrepareBuilding()
//    {
//        base._PrepareBuilding();
//        _EnableGridVisual(true);
//    }

//    private Vector3 _ClampToNearest(Vector3 pos, float threshold)
//    {
//        float t = 1f / threshold;
//        Vector3 v = ((Vector3)Vector3Int.FloorToInt(pos * t)) / t;
//        float s = threshold * 0.5f;
//        v.x += s + gridOffset.x;
//        v.y += s + gridOffset.y;
//        return v;
//    }


//    //For Grid showing in the editor
//    private void _EnableGridVisual(bool on)
//    {
//        if (gridRenderer == null) return;
//        gridRenderer.enabled = on;
//    }

//    private void _UpdateGridVisual()
//    {
//        if (gridRenderer == null) return;
//        gridRenderer.sharedMaterial.SetVector(
//            "_Cell_Size", new Vector4(cellSize, cellSize, 0, 0));
//    }

//    private void OnDrawGizmos()
//    {
//        if (!drawGizmos) return;
//        if (!drawInEditMode && !Application.isPlaying) return;
//        if (!drawInPlayMode && Application.isPlaying) return;

//        Gizmos.color = gizmoColor;

//        Vector3 origin = transform.position + new Vector3(gridOffset.x, gridOffset.y, 0);
//        float width = gridSize.x * cellSize;
//        float height = gridSize.y * cellSize;

//        // Draw grid boundaries
//        Gizmos.DrawWireCube(origin + new Vector3(width * 0.5f, height * 0.5f, 0),
//                           new Vector3(width, height, 0));

//        // Draw vertical lines
//        for (int x = 0; x <= gridSize.x; x++)
//        {
//            Vector3 start = origin + new Vector3(x * cellSize, 0, 0);
//            Vector3 end = start + new Vector3(0, height, 0);
//            Gizmos.DrawLine(start, end);
//        }

//        // Draw horizontal lines
//        for (int y = 0; y <= gridSize.y; y++)
//        {
//            Vector3 start = origin + new Vector3(0, y * cellSize, 0);
//            Vector3 end = start + new Vector3(width, 0, 0);
//            Gizmos.DrawLine(start, end);
//        }
    
//    }
//}
