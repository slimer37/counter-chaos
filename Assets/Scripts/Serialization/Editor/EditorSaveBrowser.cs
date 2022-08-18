using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Serialization
{
    public class EditorSaveBrowser : EditorWindow
    {
        SaveData viewedSave;
        bool showFileList = true;
        bool showProperties = true;
        bool showSaveCreator = true;
	
        string newSaveName;
        string newSavePlayerName;
	
        [MenuItem("Tools/Editor Save Browser")]
        static void ShowWindow() => GetWindow<EditorSaveBrowser>("Editor Save Browser").Show();

        static void ListInfo<TInfo>(string label, TInfo[] infoSet, Func<TInfo, object> valueRetriever,
            Func<TInfo, bool> itemConditionChecker = null) where TInfo : MemberInfo
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(label, EditorStyles.boldLabel);

            if (infoSet.Length == 0)
            {
                EditorGUILayout.LabelField($"No {label.ToLower()}.");
                return;
            }

            var style = new GUIStyle(EditorStyles.label) { richText = true };

            foreach (var info in infoSet)
            {
                if (!itemConditionChecker?.Invoke(info) ?? false) continue;
                
                var value = valueRetriever(info);
                var isString = value is string;
                
                EditorGUILayout.LabelField(info.Name + ':',
                    value == null ? "<color=red>Null</color>" :
                        (isString ? $"\"{value}\"" : value.ToString()),
                    style);
            }
        }

        bool VerifyFileName(out string modifiedName)
        {
            modifiedName = newSaveName;
            if (SaveSystem.NameIsValid(newSaveName)) return true;
            if (string.IsNullOrWhiteSpace(newSaveName))
            {
                EditorGUILayout.HelpBox(
                    "Cannot use empty or whitespace file name.",
                    MessageType.Error);
                return false;
            }

            modifiedName = SaveSystem.ToValidFileName(newSaveName);
            EditorGUILayout.HelpBox(
                $"Save name contains invalid file characters. It will be saved as '{modifiedName + SaveSystem.SaveFileEnding}'",
                MessageType.Warning);
            return true;
        }

        void OnGUI()
        {
            EditorStyles.label.wordWrap = true;
            showSaveCreator = EditorGUILayout.BeginFoldoutHeaderGroup(showSaveCreator, "Save Creator");
            if (showSaveCreator)
            {
                EditorGUILayout.BeginHorizontal();
                newSaveName = EditorGUILayout.TextField("File Name", newSaveName);
                EditorGUILayout.LabelField(SaveSystem.SaveFileEnding, GUILayout.Width(40));
                EditorGUILayout.EndHorizontal();
			
                newSavePlayerName = EditorGUILayout.TextField("Player Name", newSavePlayerName);
			
                if (VerifyFileName(out var willSaveAs))
                {
                    if (GUILayout.Button(SaveSystem.NameExists(willSaveAs) ? "Format Save" : "Create New Save"))
                        new SaveData(willSaveAs, newSavePlayerName).Save();
                }

                if (viewedSave && willSaveAs == viewedSave.baseFileName)
                    SaveSystem.LoadFromPath(viewedSave.AccessPath, out viewedSave);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
		
            if (!Directory.Exists(SaveSystem.SaveFolderLocation))
            {
                EditorGUILayout.HelpBox("Save folder does not exist. Create a new save to create the folder.", MessageType.Info);
                return;
            }

            showFileList = EditorGUILayout.BeginFoldoutHeaderGroup(showFileList, "Save Folder Contents");
            if (showFileList)
            {
                var folderContents = Directory.GetFiles(SaveSystem.SaveFolderLocation);

                if (folderContents.Length == 0)
                    EditorGUILayout.LabelField("Save folder empty.");
			
                foreach (var filePath in folderContents)
                {
                    var fileName = filePath.Substring(filePath.LastIndexOf(Path.DirectorySeparatorChar) + 1);
                    var isSaveFile = fileName.EndsWith(SaveSystem.SaveFileEnding);
                    
                    EditorGUILayout.BeginHorizontal();
                    EditorGUI.BeginDisabledGroup(!isSaveFile);
                    
                    EditorGUILayout.LabelField(fileName);

                    var isFocused = viewedSave && viewedSave.FileName == fileName;

                    if (isFocused)
                        EditorGUILayout.LabelField("Focused", EditorStyles.boldLabel, GUILayout.Width(52));
                    else if (GUILayout.Button("Focus", GUILayout.ExpandWidth(false)))
                        SaveSystem.LoadFromPath(filePath, out viewedSave);

                    if (GUILayout.Button("Delete", GUILayout.ExpandWidth(false)))
                    {
                        File.Delete(filePath);
                        if (isFocused)
                            viewedSave = null;
                    }
                    
                    EditorGUI.EndDisabledGroup();
                    EditorGUILayout.EndHorizontal();
                }

                if (GUILayout.Button("Open In Explorer"))
                    Process.Start(SaveSystem.SaveFolderLocation);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
		
            if (viewedSave != null)
            {
                showProperties = EditorGUILayout.BeginFoldoutHeaderGroup(showProperties,
                    $"'{viewedSave.saveName}' ({viewedSave.FileName}) Properties");
                if (showProperties)
                {
                    ListAllInfo(viewedSave);

                    if (GUILayout.Button("Refresh"))
                        SaveSystem.LoadFromPath(viewedSave.AccessPath, out viewedSave);
                }
                EditorGUILayout.EndFoldoutHeaderGroup();
            }

            if (!EditorApplication.isPlaying || SaveSystem.LoadedSave == null) return;
		
            EditorGUILayout.LabelField("Loaded Save", EditorStyles.boldLabel);
            ListAllInfo(SaveSystem.LoadedSave);
        }

        static void ListAllInfo(SaveData data)
        {
            ListInfo("Properties", typeof(SaveData).GetProperties(),
                info => info.GetValue(data),
                info => !info.GetMethod.IsStatic);
            ListInfo("Fields", typeof(SaveData).GetFields(),
                info => info.GetValue(data),
                info => !info.IsStatic);
        }
    }
}