using UnityEngine;

[RequireComponent(typeof(BaseConstructionSite))]
public class BaseConstructionSiteView : MonoBehaviour
{
    [SerializeField] private Renderer _previewBox;
    [SerializeField] private Material _validMaterial;
    [SerializeField] private Material _invalidMaterial;

    private BaseConstructionSite _constructionSite;

    private void Awake()
    {
        _constructionSite = GetComponent<BaseConstructionSite>();
    }

    private void OnEnable()
    {
        _constructionSite.SelectionPreviewChanged += ChangePreviewMaterial;
        _constructionSite.BuildingPlaced += DisablePreview;
    }

    private void OnDisable()
    {
        _constructionSite.SelectionPreviewChanged -= ChangePreviewMaterial;
        _constructionSite.BuildingPlaced -= DisablePreview;
    }

    private void ChangePreviewMaterial(bool isValid)
    {
        _previewBox.material = isValid ? _validMaterial : _invalidMaterial;
    }

    private void DisablePreview()
    {
        _previewBox.gameObject.SetActive(false);
    }

    private void Reset()
    {
        _previewBox.gameObject.SetActive(true);
    }
}
