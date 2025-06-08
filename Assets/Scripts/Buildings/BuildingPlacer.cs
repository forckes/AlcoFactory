using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingPlacer : MonoBehaviour
{
    public static BuildingPlacer instance;

    public LayerMask groundLayerMask;

    private GameObject _buildingPrefab;
    private GameObject _toBuild;

    private Camera _mainCamera;

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

        Vector2 mouseWorldPos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);

        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero, 0f, groundLayerMask);
        if (hit.collider != null)
        {
            _toBuild.SetActive(true);
            _toBuild.transform.position = hit.point;
            Debug.Log($"[BuildingPlacer] Valid ground at {hit.point}");

            if (Input.GetMouseButtonDown(0))
            {
                
                BuildingManager m = _toBuild.GetComponent<BuildingManager>();
                if (m.hasValidPlacement)
                {
                    m.SetPlacementMode(PlacementMode.Fixed);

                    _buildingPrefab = null;
                    _toBuild = null;
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
        PrepareBuilding();
    }

    private void PrepareBuilding()
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

    private void PlaceBuilding(Vector2 position)
    {
        GameObject placed = Instantiate(_buildingPrefab, position, Quaternion.identity);
        Debug.Log($"[BuildingPlacer] Building placed at {position}");
    }
}
