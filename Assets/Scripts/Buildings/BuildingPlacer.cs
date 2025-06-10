using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingPlacer : MonoBehaviour
{
    public static BuildingPlacer instance;

    public LayerMask groundLayerMask;

    protected GameObject _buildingPrefab;
    protected GameObject _toBuild;

    protected Camera _mainCamera;

    private void Awake()
    {
        instance = this;
        _mainCamera = Camera.main;
        _buildingPrefab = null;
    }

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

        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero, 1000f, groundLayerMask);
        if (hit.collider != null)
        {
            

            _toBuild.transform.position = hit.point;
            _toBuild.SetActive(true);

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

    public void SetBuildingPrefab(GameObject prefab)
    {
        _buildingPrefab = prefab;
        _PrepareBuilding();
        EventSystem.current.SetSelectedGameObject(null); // Deselect any UI elements
    }

    protected virtual void _PrepareBuilding()
    {
        if (_toBuild) Destroy(_toBuild);

        _toBuild = Instantiate(_buildingPrefab);
        _toBuild.SetActive(true);

        BuildingManager m = _toBuild.GetComponent<BuildingManager>();
        m.isFixed = false;
        m.SetPlacementMode(PlacementMode.Valid);

        Vector3 spawnPos = _mainCamera.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x, Input.mousePosition.y, -_mainCamera.transform.position.z)
        );
        _toBuild.transform.position = spawnPos;
    }

    //private void PlaceBuilding(Vector2 position)
    //{
    //    GameObject placed = Instantiate(_buildingPrefab, position, Quaternion.identity);
        
    //}
}
