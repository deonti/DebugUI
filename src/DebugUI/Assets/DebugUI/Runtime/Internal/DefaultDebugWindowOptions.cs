using System;
using UnityEngine;

namespace DebugUI
{
    [Serializable]
    public class DefaultDebugWindowOptions
    {
        [field: SerializeField] public string Title { get; private set; } = "Debug";
        [field: SerializeField] public bool Draggable { get; private set; } = true;
        [field: SerializeField] public bool Expanded { get; private set; } = true;
    }
}