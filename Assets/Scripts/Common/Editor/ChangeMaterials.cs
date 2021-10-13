using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ChangeMaterials : ScriptableWizard
{
    public GameObject targetGo;
    public Shader toShader;

    public Material fromMaterial;
    public Material toMaterial;

    [Serializable]
    public class ChangeMaterialInfo
    {
        public List<NamePairInfo> names = new List<NamePairInfo>();
    }
    [Serializable]
    public class NamePairInfo
    {
        public string form;
        public string to;
    }

    public EditorPrefsData<ChangeMaterialInfo> names;
    private List<ShaderPropertyInfo> properties;

    [MenuItem("Assets/Change Materials")]

    static void Init()
    {
        ChangeMaterials instance = DisplayWizard<ChangeMaterials>("Change Materials", "닫기");
        instance.InitVariable();
    }

    private void InitVariable()
    {
        names = new EditorPrefsData<ChangeMaterialInfo>("names");
    }

    protected override bool DrawWizardGUI()
    {
        base.DrawWizardGUI();

        if (GUILayout.Button("속성 적용"))
        {
            ApplyPropertie();
        }

        if (GUILayout.Button("쉐이더 교체"))
        {
            ChangeShader();
        }

        return true;
    }

    private void ChangeShader()
    {
        var renderers = targetGo.GetComponentsInChildren<Renderer>();
        foreach(var renderer in renderers)
        {
            //모든 메테리얼 가져오자.
            var materials = renderer.sharedMaterials;
            List<Material> newMaterials = new List<Material>();

            foreach (var oldMat in materials)
            {
                Material newMat = new Material(toShader);
                newMat.name = toShader.name + "_" + toShader.name;

                properties = CopyProperties(oldMat, names);
                PasteProperties(properties, newMat);
                newMaterials.Add(newMat);

                string oldPath = AssetDatabase.GetAssetPath(oldMat);
                string newPath = oldPath.Replace(".mat", $"_{toShader.name}.mat");

                AssetDatabase.CreateAsset(newMat, newPath);
            }

            renderer.sharedMaterials = newMaterials.ToArray();
        }
    }

    class ShaderPropertyInfo
    {
        public string toName;
        public string formName;
        public ShaderUtil.ShaderPropertyType type;
        internal Color colorValue;
        internal Vector4 vectorValue;
        internal float floatValue;
        internal Texture textureValue;
        internal List<float> floatArray = new List<float>();

        public ShaderPropertyInfo(string toName, string fromName, ShaderUtil.ShaderPropertyType type)
        {
            this.toName = toName;
            this.formName = fromName;
            this.type = type;
        }
    }

    private void ApplyPropertie()
    {
        properties = CopyProperties(fromMaterial,names);

        PasteProperties(properties, toMaterial);

        names.SaveData();
    }

    static private void PasteProperties(List<ShaderPropertyInfo> properties, Material toMat)
    {
        //foreach (var toMat in toMaterials)
        {
            foreach (var item in properties)
            {
                switch (item.type)
                {
                    case ShaderUtil.ShaderPropertyType.Color:
                        toMat.SetColor(item.toName, item.colorValue);
                        break;
                    case ShaderUtil.ShaderPropertyType.Vector:
                        toMat.SetVector(item.toName, item.vectorValue);
                        break;
                    case ShaderUtil.ShaderPropertyType.Float:
                        toMat.SetFloat(item.toName, item.floatValue);
                        break;
                    case ShaderUtil.ShaderPropertyType.Range:
                        toMat.SetFloatArray(item.toName, item.floatArray); // 테스트 안해봤음.
                        break;
                    case ShaderUtil.ShaderPropertyType.TexEnv:
                        toMat.SetTexture(item.toName, item.textureValue);
                        break;
                }
            }
        }
    }

    //private List<Material> GetSelectedMaterials()
    //{
    //    var result = new List<Material>();
    //    result.Add(toMaterial);
    //    return result;
    //}

    static private List<ShaderPropertyInfo> CopyProperties(Material fromMaterial, EditorPrefsData<ChangeMaterialInfo> names)
    {
        List<ShaderPropertyInfo> result = new List<ShaderPropertyInfo>();
        int count = ShaderUtil.GetPropertyCount(fromMaterial.shader);
        result = new List<ShaderPropertyInfo>(count);
        for (var i = 0; i < count; ++i)
        {
            var name = ShaderUtil.GetPropertyName(fromMaterial.shader, i);
            var namePair = names.data.names.Find(x => x.form == name);
            if (namePair == null)
                continue;

            var type = ShaderUtil.GetPropertyType(fromMaterial.shader, i);
            result.Add(new ShaderPropertyInfo(namePair.to, name, type));
        }

        foreach (var item in result)
        {

            switch (item.type)
            {
                case ShaderUtil.ShaderPropertyType.Color:
                    item.colorValue = fromMaterial.GetColor(item.formName);
                    break;
                case ShaderUtil.ShaderPropertyType.Vector:
                    item.vectorValue = fromMaterial.GetVector(item.formName);
                    break;
                case ShaderUtil.ShaderPropertyType.Float:
                    item.floatValue = fromMaterial.GetFloat(item.formName);
                    break;
                case ShaderUtil.ShaderPropertyType.Range:
                    fromMaterial.GetFloatArray(item.formName, item.floatArray);
                    break;
                case ShaderUtil.ShaderPropertyType.TexEnv: // 텍스쳐는 복사할 필요 없으므로 제외
                    item.textureValue = fromMaterial.GetTexture(item.formName);
                    break;
            }
        }

        return result;
    }
}
