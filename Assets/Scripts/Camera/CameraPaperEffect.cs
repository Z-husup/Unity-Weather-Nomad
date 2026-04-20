using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public class CameraPaperEffect : MonoBehaviour
{
    [SerializeField] private Material paperMaterial;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (paperMaterial != null)
        {
            Graphics.Blit(source, destination, paperMaterial);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }
}