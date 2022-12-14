using UnityEngine;

public class SkinnedMeshMaterialChanger : MonoBehaviour
{
    [SerializeField]
    private EPlaneType planeType = EPlaneType.Bomber;

    [SerializeField]
    private GameObject model = null;

    [SerializeField]
    private SkinnedMeshRenderer[] list = null;

    private void Awake()
    {
        Material mat = PlaneManager.Instance.GetPlaneMaterial(planeType);

        if (mat == null)
        {
            Debug.LogError("Material name is incorrect!");
            return;
        }
        if (list.Length == 0)
        {
            list = model.GetComponentsInChildren<SkinnedMeshRenderer>(true);
        }
        ChangeMaterial(mat);
    }

    public void Highlight(bool enable)
    {
        //ChangeMaterial(enable ? PlaneManager.Instance.TransparentMaterial : baseMaterial);
    }

    private void ChangeMaterial(Material mat)
    {
        foreach (var renderer in list)
        {
            if (!renderer.sharedMaterial.name.Contains("glass"))
            {
                renderer.material = mat;
            }
        }
    }
}
