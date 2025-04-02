using UnityEngine;

namespace DebugUI
{
    public sealed class DebugWindowOptions : IDebugUIOptions
    {
        public string Title { get; set; }
        public bool Draggable { get; set; }
        public bool Expanded { get; set; }

        public static DebugWindowOptions Default { get; private set; }

        public DebugWindowOptions Clone() =>
            (DebugWindowOptions)MemberwiseClone();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Initialize() =>
            Default = new DebugWindowOptions
            {
                Title = Settings.Current.DefaultWindowOptions.Title,
                Draggable = Settings.Current.DefaultWindowOptions.Draggable,
                Expanded = Settings.Current.DefaultWindowOptions.Expanded,
            };
    }
}