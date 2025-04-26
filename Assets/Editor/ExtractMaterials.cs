#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

public class ExtractMaterials : EditorWindow
{
    private string targetFolderPath = "Assets/模型"; // 修改为目标文件夹路径
    private string shaderName = "SimpleURPToonLitExample(With Outline)"; // 修改为指定的Shader名称

    [MenuItem("Tools/Extract Materials")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(ExtractMaterials));
    }

    void OnGUI()
    {
        GUILayout.Label("Extract Materials", EditorStyles.boldLabel);

        targetFolderPath = EditorGUILayout.TextField("Target Folder Path", targetFolderPath);
        shaderName = EditorGUILayout.TextField("Shader Name", shaderName);

        if (GUILayout.Button("Extract"))
        {
            ExtractAndChangeShader();
        }
    }

    void ExtractAndChangeShader()
    {
        // 获取所有模型文件
        string[] modelGuids = AssetDatabase.FindAssets("t:GameObject", new string[] { targetFolderPath });
        foreach (string modelGuid in modelGuids)
        {
            string modelPath = AssetDatabase.GUIDToAssetPath(modelGuid);
            GameObject model = AssetDatabase.LoadAssetAtPath<GameObject>(modelPath);

            // 遍历所有子对象的Renderer组件
            Renderer[] renderers = model.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                Material[] materials = renderer.sharedMaterials;
                foreach (Material material in materials)
                {
                    if (material != null)
                    {
                        // 保存材质到模型同一目录下
                        Directory.CreateDirectory(Path.GetDirectoryName(modelPath) + "\\Materials\\");
                        string materialPath = Path.GetDirectoryName(modelPath) + "\\Materials\\" + material.name + ".mat";
                        Material newMaterial = new Material(material);
                        AssetDatabase.CreateAsset(newMaterial, materialPath);

                        // 更改Shader
                        Shader targetShader = Shader.Find(shaderName);
                        if (targetShader != null)
                        {
                            newMaterial.shader = targetShader;
                            AssetDatabase.SaveAssets();
                        }
                        else
                        {
                            Debug.LogError("Shader not found: " + shaderName);
                        }

                        // 更新Renderer的材质
                        int materialIndex = System.Array.IndexOf(renderer.sharedMaterials, material);
                        Material[] newMaterials = renderer.sharedMaterials;
                        newMaterials[materialIndex] = newMaterial;
                        renderer.sharedMaterials = newMaterials;
                    }
                }
            }
        }

        AssetDatabase.SaveAssets();
        Debug.Log("Materials extracted and Shader changed.");
    }
}
#endif
