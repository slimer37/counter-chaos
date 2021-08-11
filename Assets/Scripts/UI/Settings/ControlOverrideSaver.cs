using System;
using System.Collections.Generic;
using Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UI.Settings
{
    public class ControlOverrideSaver : MonoBehaviour
    {
        [Serializable]
        struct BindingList { public List<BindingSerializable> bindings; }
	
        [Serializable]
        struct BindingSerializable {
            public string id;
            public string path;
        }

        static InputActionAsset asset;

        public static void ClearOverrides()
        {
            foreach (var map in asset.actionMaps)
                map.RemoveAllBindingOverrides();
		
            PlayerPrefs.DeleteKey("Controls");
        }

        public static void Save()
        {
            var bindingList = new BindingList { bindings = new List<BindingSerializable>() };
		
            foreach (var map in asset.actionMaps)
            foreach (var binding in map.bindings)
                if (!string.IsNullOrEmpty(binding.overridePath))
                    bindingList.bindings.Add(new BindingSerializable
                        {id = binding.id.ToString(), path = binding.overridePath});
		
            PlayerPrefs.SetString("Controls", JsonUtility.ToJson(bindingList));
            PlayerPrefs.Save();
        }

        [RuntimeInitializeOnLoadMethod]
        static void InitControlsAsset()
        {
            var controls = new Controls();
            asset = controls.asset;
            controls.Dispose();
        }
        
        [RuntimeInitializeOnLoadMethod]
        static void Load()
        {
            if (!PlayerPrefs.HasKey("Controls")) return;

            var bindingList = JsonUtility.FromJson<BindingList>(PlayerPrefs.GetString("Controls"));

            var overrides = new Dictionary<Guid, string>();
            foreach (var binding in bindingList.bindings)
                overrides[new Guid(binding.id)] = binding.path;

            foreach (var map in asset.actionMaps)
            {
                var bindings = map.bindings;
                for (var i = 0; i < bindings.Count; ++i)
                    if (overrides.TryGetValue(bindings[i].id, out var overridePath))
                        map.ApplyBindingOverride(i, new InputBinding {overridePath = overridePath});
            }
        }
    }
}
