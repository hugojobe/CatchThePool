using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class CutoutUiMask : Image
{
    private Material cachedMaterial;

    public override Material materialForRendering
    {
        get
        {
            if (cachedMaterial == null)
            {
                cachedMaterial = new Material(base.materialForRendering);
                cachedMaterial.SetInt("_StencilComp", (int)CompareFunction.NotEqual);
            }
            return cachedMaterial;
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        SetMaterialDirty();
    }
}