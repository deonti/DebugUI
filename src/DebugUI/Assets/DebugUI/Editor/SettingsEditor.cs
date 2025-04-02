using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace DebugUI.Editor
{
    [CustomEditor(typeof(Settings))]
    public class SettingsEditor : UnityEditor.Editor
    {
        const string SettingsTitle = "Debug UI";
        const string SettingsPath = "Project/Debug UI";
        const string EditorBuildSettingsKey = "com.annulusgames.debug-ui.settings";
        const string DefaultPanelSettingsGuid = "0667eda26153c6547901736dde39a7e2";
        const string DefaultAssetPath = "Assets/DebugUI.settings.asset";

        static readonly IEnumerable<string> _keywords = new[]
        {
            "Debug", "UI",
        };

        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            root.Add(new Button(OpenSettingsWindow)
            {
                text = "Open Settings Window",
            });
            return root;
        }

        [OnOpenAsset(OnOpenAssetAttributeMode.Execute)]
        public static bool OnOpenAsset(int instanceID)
        {
            if (AssetDatabase.GetMainAssetTypeAtPath(AssetDatabase.GetAssetPath(instanceID)) != typeof(Settings))
                return false;

            OpenSettingsWindow();
            return true;
        }

        [InitializeOnLoadMethod]
        public static void InitializeOnLoadMethod() =>
            ValidateSettingsAsset();

        [SettingsProvider]
        public static SettingsProvider CreateRootSettingsProvider() =>
            new(SettingsPath, SettingsScope.Project)
            {
                label = SettingsTitle,
                keywords = _keywords,
                activateHandler = ActivateHandler,
            };

        protected override bool ShouldHideOpenButton() =>
            true;

        static void OpenSettingsWindow() =>
            SettingsService.OpenProjectSettings(SettingsPath);

        static void ActivateHandler(string _, VisualElement rootElement)
        {
            ValidateSettingsAsset();
            rootElement.contentContainer.Add(CreateContent());
        }

        static VisualElement CreateContent()
        {
            var settings = Settings.Current;
            SerializedObject settingsSerializedObject = new(settings);

            var content = new VisualElement
            {
                style =
                {
                    paddingTop = 2,
                    paddingBottom = 2,
                    paddingLeft = 10,
                    paddingRight = 0,
                },
            };
            content.Add(CreateTitle());
            content.Add(CreateProperties(settingsSerializedObject));
            content.Add(CreateUtilsPanel());
            content.Bind(settingsSerializedObject);
            return content;
        }

        static Label CreateTitle() =>
            new()
            {
                text = SettingsTitle,
                style =
                {
                    fontSize = 18,
                    unityFontStyleAndWeight = FontStyle.Bold,
                    marginBottom = 10,
                },
            };

        static VisualElement CreateProperties(SerializedObject settings)
        {
            var properties = new VisualElement
            {
                style =
                {
                    marginBottom = 6,
                },
            };
            InspectorElement.FillDefaultInspector(properties, settings, null);
            properties.Q<PropertyField>("PropertyField:m_Script")?.RemoveFromHierarchy();
            return properties;
        }

        static VisualElement CreateUtilsPanel()
        {
            var panel = new Foldout
            {
                text = "Utils",
                value = true,
            };

            var pingAssetButton = new Button(PingSettingsAsset)
            {
                text = "Ping Settings asset",
            };
            var addToPreloadedAssetButton = new Button
            {
                text = "Include Settings asset to \"Player/Preloaded Assets\"",
            };
            var removeFromPreloadedAssetButton = new Button
            {
                text = "Exclude Settings asset from \"Player/Preloaded Assets\"",
            };

            addToPreloadedAssetButton.clicked += AddSettingsToPreloadedAssets;
            removeFromPreloadedAssetButton.clicked += RemoveSettingsToPreloadedAssets;
            UpdateButtonsState();

            panel.Add(pingAssetButton);
            panel.Add(addToPreloadedAssetButton);
            panel.Add(removeFromPreloadedAssetButton);

            return panel;

            void PingSettingsAsset() =>
                EditorGUIUtility.PingObject(Settings.Current);

            void AddSettingsToPreloadedAssets()
            {
                var preloadedAssets = PlayerSettings.GetPreloadedAssets();
                if (preloadedAssets.Contains(Settings.Current)) return;

                PlayerSettings.SetPreloadedAssets(preloadedAssets.Append(Settings.Current).ToArray());
                UpdateButtonsState();
            }

            void RemoveSettingsToPreloadedAssets()
            {
                var preloadedAssetList = PlayerSettings.GetPreloadedAssets().ToList();
                if (preloadedAssetList.RemoveAll(Settings.Current.Equals) == 0) return;

                PlayerSettings.SetPreloadedAssets(preloadedAssetList.ToArray());
                UpdateButtonsState();
            }

            void UpdateButtonsState()
            {
                bool isPreloadedAssetsHasSettings = PlayerSettings.GetPreloadedAssets().Contains(Settings.Current);
                addToPreloadedAssetButton.SetEnabled(!isPreloadedAssetsHasSettings);
                removeFromPreloadedAssetButton.SetEnabled(isPreloadedAssetsHasSettings);
            }
        }

        static void ValidateSettingsAsset()
        {
            if (EditorBuildSettings.TryGetConfigObject<Settings>(EditorBuildSettingsKey, out _)) return;

            var settings = CreateInstance<Settings>();
            string defaultPanelSettingsPath = AssetDatabase.GUIDToAssetPath(DefaultPanelSettingsGuid);
            if (!string.IsNullOrEmpty(defaultPanelSettingsPath))
                settings.DefaultDocumentOptions.PanelSettings = AssetDatabase.LoadAssetAtPath<PanelSettings>(defaultPanelSettingsPath);
            AssetDatabase.CreateAsset(settings, DefaultAssetPath);
            EditorBuildSettings.AddConfigObject(EditorBuildSettingsKey, settings, true);

            var preloadedAssets = PlayerSettings.GetPreloadedAssets().ToList();
            preloadedAssets.RemoveAll(asset => asset is Settings);
            preloadedAssets.Add(settings);

            PlayerSettings.SetPreloadedAssets(preloadedAssets.ToArray());
        }
    }
}