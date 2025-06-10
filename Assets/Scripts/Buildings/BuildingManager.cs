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
    public SpriteRenderer[] spriteRenderers;

    [HideInInspector] public bool hasValidPlacement;
    [HideInInspector] public bool isFixed;

    private int _nObstacles;
    private Dictionary<SpriteRenderer, Material> initialMaterials;

    private void Awake()
    {
        hasValidPlacement = true;
        isFixed = true;
        _nObstacles = 0;
        _InitializeMaterials();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isFixed || _IsGround(other.gameObject)) return;
        _nObstacles++;
        SetPlacementMode(PlacementMode.Invalid);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (isFixed || _IsGround(other.gameObject)) return;
        _nObstacles--;
        if (_nObstacles == 0)
            SetPlacementMode(PlacementMode.Valid);
    }

    public void SetPlacementMode(PlacementMode mode)
    {
        if (mode == PlacementMode.Fixed)
        {
            isFixed = true;
            hasValidPlacement = true;
        }
        else
        {
            hasValidPlacement = (mode == PlacementMode.Valid);
        }
        _SetMaterial(mode);
    }

    private void _SetMaterial(PlacementMode mode)
    {
        if (mode == PlacementMode.Fixed)
        {
            foreach (var renderer in spriteRenderers)
            {
                if (renderer != null && initialMaterials.ContainsKey(renderer))
                {
                    renderer.sharedMaterial = initialMaterials[renderer];
                }
            }
        }
        else
        {
            Material matToApply = mode == PlacementMode.Valid
                ? validPlacementMaterial : invalidPlacementMaterial;

            foreach (var renderer in spriteRenderers)
            {
                if (renderer != null)
                {
                    renderer.sharedMaterial = matToApply;
                }
            }
        }
    }

    private void _InitializeMaterials()
    {
        initialMaterials = new Dictionary<SpriteRenderer, Material>();
        foreach (var renderer in spriteRenderers)
        {
            if (renderer != null)
            {
                initialMaterials[renderer] = renderer.sharedMaterial;
            }
        }
    }

    private bool _IsGround(GameObject o)
    {
        return ((1 << o.layer) & BuildingPlacer.instance.groundLayerMask.value) != 0;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        _InitializeMaterials();
    }
#endif
}