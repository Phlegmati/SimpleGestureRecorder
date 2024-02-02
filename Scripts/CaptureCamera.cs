using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Camera))]
public class CaptureCamera : MonoBehaviour
{
    public Image gestureImage;

    [SerializeField]
    GameObject m_MainCamera;

    const int DISPLAY_1 = 0;
    const int DISPLAY_2 = 1;

    Camera m_Cam;
    Texture2D m_Screenshot;

    event Action OnPostRenderEvent;

    public void TakeScreenshot()
    {
        m_Cam.targetDisplay = DISPLAY_1;
        OnPostRenderEvent += CreateTexture;
    }
    public void SaveTexture(string dirPath)
    {
#if UNITY_EDITOR
        byte[] bytes = m_Screenshot.EncodeToPNG();
        System.IO.File.WriteAllBytes(dirPath, bytes);
#endif
    }

    void Start()
    {
#if !UNITY_EDITOR
            Destroy(this);
#endif
        m_MainCamera = GameObject.Find("Main Camera");
        if (!m_MainCamera)
            m_MainCamera = GameObject.FindGameObjectWithTag("MainCamera");

        m_Cam = GetComponent<Camera>();
    }

    void Update()
    {
        if(m_MainCamera != null)
        {
            transform.position = m_MainCamera.transform.position;
            transform.rotation = m_MainCamera.transform.rotation;
            transform.localScale = m_MainCamera.transform.localScale;
        }
    }

    void OnPostRender()
    {
        OnPostRenderEvent?.Invoke();
    }

    void CreateTexture()
    {
        int screenShotWidth = Screen.width;
        int screenShotHeight = Screen.height;

        m_Screenshot = new Texture2D(screenShotWidth, screenShotHeight, TextureFormat.RGB24, false);
        var rect = new Rect(0, 0, screenShotWidth, screenShotHeight);
        m_Screenshot.ReadPixels(rect, 0, 0);
        m_Screenshot.Apply();

        gestureImage.sprite = Sprite.Create(m_Screenshot, rect, new Vector2(0.5f, 0.5f));
        gestureImage.enabled = true;

        m_Cam.targetDisplay = DISPLAY_2;

        OnPostRenderEvent -= CreateTexture;
    }

}