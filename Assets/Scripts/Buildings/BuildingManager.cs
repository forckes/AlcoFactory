using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlacementMode
{
    Fixed,
    Valid,
    Invalid
}

public class BuildingManager : MonoBehaviour
{
    public Material validPlacementMaterial;
    public Material invalidPlacementMaterial;

    public MeshRenderer[] meshComponents;
    private Dictionary<MeshRenderer, List<Material>> initialMaterials;

    [HideInInspector] public bool hasValidPlacement;
    [HideInInspector] public bool isFixed;

    private int _nObstacles;

    private void Awake()
    {
        hasValidPlacement = true;
        isFixed = true;
        _nObstacles = 0;

        _InitializeMaterials();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isFixed) return;
        if (_IsGround(other.gameObject)) return;

        _nObstacles++;
        SetPlacementMode(PlacementMode.Invalid);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (isFixed) return;
        if (_IsGround(other.gameObject)) return;

        _nObstacles--;
        if (_nObstacles == 0)
            SetPlacementMode(PlacementMode.Valid);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        _InitializeMaterials();
    }
#endif

    public void SetPlacementMode(PlacementMode mode)
    {
        if (mode == PlacementMode.Fixed)
        {
            isFixed = true;
            hasValidPlacement = true;
            Debug.Log("[BuildingManager] Placement finalized (Fixed).");
        }
        else if (mode == PlacementMode.Valid)
        {
            if (!hasValidPlacement) Debug.Log("[BuildingManager] Placement VALID.");
            hasValidPlacement = true;
        }
        else
        {
            if (hasValidPlacement) Debug.Log("[BuildingManager] Placement INVALID.");
            hasValidPlacement = false;
        }
        SetMaterial(mode);
    }

    public void SetMaterial(PlacementMode mode)
    {
        if (mode == PlacementMode.Fixed)
        {
            foreach (MeshRenderer r in meshComponents)
                r.sharedMaterials = initialMaterials[r].ToArray();
        }
        else
        {
            Material matToApply = mode == PlacementMode.Valid
                ? validPlacementMaterial : invalidPlacementMaterial;

            Material[] m; int nMaterials;
            foreach (MeshRenderer r in meshComponents)
            {
                nMaterials = initialMaterials[r].Count;
                m = new Material[nMaterials];
                for (int i = 0; i < nMaterials; i++)
                    m[i] = matToApply;
                r.sharedMaterials = m;
            }
        }
    }

    private void _InitializeMaterials()
    {
        if (initialMaterials == null)
            initialMaterials = new Dictionary<MeshRenderer, List<Material>>();
        if (initialMaterials.Count > 0)
        {
            foreach (var l in initialMaterials) l.Value.Clear();
            initialMaterials.Clear();
        }

        foreach (MeshRenderer r in meshComponents)
        {
            initialMaterials[r] = new List<Material>(r.sharedMaterials);
        }
    }

    private bool _IsGround(GameObject o)
    {
        return ((1 << o.layer) & BuildingPlacer.instance.groundLayerMask.value) != 0;
    }

}