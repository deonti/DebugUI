using UnityEngine.UIElements;

namespace DebugUI
{
    public static class VisualElementExtensions
    {
        public static VisualElement AddToDebugUIDefaultDocument(this VisualElement element)
        {
            element.style.position = Position.Absolute;
            element.RegisterCallback<PointerDownEvent>(BringWindowToFront);
            element.RegisterCallback<FocusInEvent>(BringWindowToFront);

            Settings.Current.DefaultDocument.rootVisualElement.Add(element);
            return element;

            void BringWindowToFront(EventBase _)
                => element.BringToFront();
        }
    }
}