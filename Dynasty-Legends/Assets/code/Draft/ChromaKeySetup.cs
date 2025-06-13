using UnityEngine;
using UnityEngine.Video;

public class ChromaKeySetup : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public Material chromaKeyMaterial;
    public RenderTexture videoRenderTexture;
    
    void Start()
    {
        // Setup render texture
        videoRenderTexture = new RenderTexture(1920, 1080, 0);
        videoPlayer.targetTexture = videoRenderTexture;
        
        // Apply video texture to material
        chromaKeyMaterial.SetTexture("_VideoTexture", videoRenderTexture);
        
        // Apply material to renderer
        GetComponent<Renderer>().material = chromaKeyMaterial;
    }
}
