#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

public class PrefabIconBaker : EditorWindow
{
    [SerializeField] private GameObject[] prefabs;
    [SerializeField] private int size = 256;
    [SerializeField] private string saveFolder = "Assets/GeneratedIcons";

    private Camera cam;
    private Light light;

    [MenuItem("Tools/Prefab Icon Baker")]
    public static void Open()
    {
        GetWindow<PrefabIconBaker>("Prefab Icon Baker");
    }

    private void OnGUI()
    {
        SerializedObject so = new SerializedObject(this);
        EditorGUILayout.PropertyField(so.FindProperty("prefabs"), true);
        size = EditorGUILayout.IntField("Size", size);
        saveFolder = EditorGUILayout.TextField("Save Folder", saveFolder);
        so.ApplyModifiedProperties();

        if (GUILayout.Button("Bake Icons"))
        {
            BakeAll();
        }
    }

    private void BakeAll()
    {
        if (!Directory.Exists(saveFolder))
            Directory.CreateDirectory(saveFolder);

        SetupCamera();

        try
        {
            foreach (var prefab in prefabs)
            {
                if (prefab == null) continue;

                GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                Texture2D tex = null;

                try
                {
                    instance.transform.position = new Vector3(0f, 0f, 10000f);
                    instance.transform.rotation = Quaternion.Euler(0, 0, 0f);

                    FitCameraToObject(instance);

                    tex = Capture();

                    string path = $"{saveFolder}/{prefab.name}.png";
                    File.WriteAllBytes(path, tex.EncodeToPNG());

                    AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
                    TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;

                    if (importer != null)
                    {
                        importer.textureType = TextureImporterType.Sprite;
                        importer.spriteImportMode = SpriteImportMode.Single;
                        importer.SaveAndReimport();
                    }
                }
                finally
                {
                    if (instance != null)
                        DestroyImmediate(instance);

                    if (tex != null)
                        DestroyImmediate(tex);
                }
            }

            AssetDatabase.Refresh();
            Debug.Log("Icon baking complete.");
        }
        finally
        {
            CleanupCamera();
        }
    }

    private void SetupCamera()
    {
        GameObject camObj = new GameObject("Icon Camera");
        cam = camObj.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0, 0, 0, 0);
        cam.orthographic = true;
        cam.cullingMask = ~0;

        GameObject lightObj = new GameObject("Icon Light");
        light = lightObj.AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 1.2f;
        light.transform.rotation = Quaternion.Euler(45f, -30f, 0f);
    }

    private void FitCameraToObject(GameObject target)
    {
        Bounds bounds = GetBounds(target);

        cam.transform.position = bounds.center + new Vector3(0f, 0f, -5f);
        cam.transform.LookAt(bounds.center);

        float maxSize = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
        cam.orthographicSize = maxSize * 0.75f;
    }

    private Texture2D Capture()
    {
        RenderTexture rt = new RenderTexture(size, size, 24, RenderTextureFormat.ARGB32);
        cam.targetTexture = rt;

        RenderTexture prev = RenderTexture.active;
        RenderTexture.active = rt;

        cam.Render();

        Texture2D tex = new Texture2D(size, size, TextureFormat.ARGB32, false);
        tex.ReadPixels(new Rect(0, 0, size, size), 0, 0);
        tex.Apply();

        cam.targetTexture = null;
        RenderTexture.active = prev;
        rt.Release();

        return tex;
    }

    private Bounds GetBounds(GameObject obj)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0)
            return new Bounds(obj.transform.position, Vector3.one);

        Bounds bounds = renderers[0].bounds;

        foreach (Renderer r in renderers)
            bounds.Encapsulate(r.bounds);

        return bounds;
    }

    private void CleanupCamera()
    {
        if (cam != null)
            DestroyImmediate(cam.gameObject);

        if (light != null)
            DestroyImmediate(light.gameObject);
    }
}
#endif
