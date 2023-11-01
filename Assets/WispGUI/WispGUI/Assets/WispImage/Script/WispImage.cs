using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Events;
using WispExtensions;

public class WispImage : WispVisualComponent
{
    private Image image;
    private string filePath = "";
    private string url = "";
    private GameObject hourGlass;
    private UnityEvent onDownloadComplet = new UnityEvent();

    public Image Base
    {
        get
        {
            return GetComponent<Image>();
        }
    }
    
    public Image Image
    {
        get
        {
            return image;
        }
        set
        {
            image = value;
        }
    }

    public string FileName
    {
        get
        {
            return filePath;
        }
    }

    public UnityEvent OnDownloadComplet { get => onDownloadComplet; }

    // Start is called before the first frame update
    void Awake()
    {
        Initialize();
    }

    //...
    void Start()
    {
        ApplyStyle();
    }

    // ...
    public override bool Initialize()
    {
        if (isInitialized)
            return true;

        base.Initialize();

        // ---------------------------------------------------------------------

        image = GetComponent<Image>();
        hourGlass = transform.Find("Hourglass").gameObject;

        // ---------------------------------------------------------------------------------------------------------------------------

        isInitialized = true;

        return true;
    }

    public override void ApplyStyle()
    {
        if (CheckIgnoreApplyStyle(false))
            return;
        
        base.ApplyStyle();

        if (isSelected)
        {
            image.ApplyStyle_Selected(style, Opacity, subStyleRule);
        }
        else
        {
            image.ApplyStyle(style, Opacity, subStyleRule);
        }

        hourGlass.GetComponent<Image>().color = colop(style.ImageColor);
    }

    /// <summary>
    /// ...
    /// </summary>
    public static WispImage Create(Transform ParamTransform)
    {
        GameObject go;

        if (ParamTransform != null)
        {
            go = Instantiate(WispPrefabLibrary.Default.Image, ParamTransform);
        }
        else
        {
            go = Instantiate(WispPrefabLibrary.Default.Image);
        }

        return go.GetComponent<WispImage>();
    }

    /// <summary>
    /// Get image file path or URL.
    /// </summary>
    public override string GetValue()
    {
        if (filePath != "")
        {
            return filePath;
        }
        else if (url != "")
        {
            return url;
        }
        else
        {
            return "";
        }
    }

    /// <summary>
    /// Load image from a file or URL.
    /// </summary>
    public override void SetValue(string ParamValue)
    {
        if (Uri.IsWellFormedUriString(ParamValue, UriKind.Absolute))
        {
            url = ParamValue;
            StartCoroutine(DownloadImage(ParamValue));
        }
        else if (File.Exists(ParamValue))
        {
            filePath = ParamValue;
            LoadImageFromFile(filePath);
        }
    }

    /// <summary>
    /// Load image from a sprite.
    /// </summary>
    public void SetValue(Sprite ParamSprite)
    {
        if (ParamSprite == null)
            return;
        
        image.sprite = ParamSprite;
        filePath = "";
    }

    /// <summary>
    /// Load image from a Texture2D.
    /// </summary>
    public void SetValue(Texture2D ParamTexture)
    {
        image.overrideSprite = Sprite.Create(ParamTexture, new Rect(0, 0, ParamTexture.width, ParamTexture.height), Vector2.zero);
    }

    // ...
    IEnumerator DownloadImage(string ParamUrl, bool ParamPrintUrl = false)
    {
        SetBusyMode(true);

        image.enabled = false;

        Texture2D tex;

        if (ParamPrintUrl)
            print("GET : " + ParamUrl);

        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(ParamUrl))
        {
            yield return uwr.SendWebRequest();

            if (uwr.result != UnityWebRequest.Result.Success)
            {
                LogError("Unable to download image.");
                image.enabled = true;
                SetBusyMode(false);
                yield break;
            }

            tex = DownloadHandlerTexture.GetContent(uwr);
            image.enabled = true;
            SetValue(tex);

            onDownloadComplet.Invoke();
        }

        ApplyStyle();

        SetBusyMode(false);

        yield break;
    }

    /// <summary>
    /// ...
    /// </summary>
    protected void LoadImageFromFile(string ParamFilePath)
    {
        Texture2D tex = null;
        byte[] fileData;

        if (File.Exists(ParamFilePath))
        {
            fileData = File.ReadAllBytes(ParamFilePath);
            tex = new Texture2D(2, 2);
            tex.LoadImage(fileData); // This will auto-resize the texture dimensions.
            image.overrideSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
        }
    }

    public override void SetBusyMode(bool ParamState)
    {
        if (ParamState)
        {
            hourGlass.SetActive(true);
            image.color = Color.clear;
        }
        else
        {
            hourGlass.SetActive(false);

            ApplyStyle();
        }
    }

    //...
    public Texture2D GetTexture2D()
    {
        return image.mainTexture as Texture2D;
    }
}