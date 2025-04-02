using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

[assembly: InternalsVisibleTo("DebugUI.Editor")]

namespace DebugUI
{
    internal sealed class Settings : ScriptableObject
    {
        [SerializeField] public DefaultDebugWindowOptions DefaultWindowOptions = new();
        [SerializeField] public DefaultDocumentSettings DefaultDocumentOptions = new();

        public static Settings Current { get; private set; }

        public UIDocument DefaultDocument
        {
            get => GetDefaultDocument();
        }

        UIDocument _defaultDocument;

        void OnEnable() =>
            Current = this;

        void OnDisable() =>
            Current = null;

        UIDocument GetDefaultDocument()
        {
            if (_defaultDocument) return _defaultDocument;

            var documentSettings = Current.DefaultDocumentOptions;
            _defaultDocument = new GameObject(documentSettings.GameObjectName, typeof(UIDocument)).GetComponent<UIDocument>();
            _defaultDocument.panelSettings = documentSettings.PanelSettings;
            DontDestroyOnLoad(_defaultDocument);

            return _defaultDocument;
        }
    }
}