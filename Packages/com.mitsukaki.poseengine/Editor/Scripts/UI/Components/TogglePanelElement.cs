using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

namespace com.mitsukaki.poseengine.editor.ui.Components
{
    public class TogglePanelElement : VisualElement
    {
        private List<VisualElement> _elements;
        private int _selectedIndex;

        public TogglePanelElement(List<VisualElement> elements)
        {
            _elements = elements;
            _selectedIndex = -1;

            foreach (var element in _elements)
            {
                element.style.display = DisplayStyle.None;
                Add(element);
            }
        }

        public void SelectElement(int index)
        {
            if (index < 0 || index >= _elements.Count)
            {
                Debug.LogWarning("Index out of range");
                return;
            }

            if (_selectedIndex != -1)
            {
                _elements[_selectedIndex].style.display = DisplayStyle.None;
            }

            _selectedIndex = index;
            _elements[_selectedIndex].style.display = DisplayStyle.Flex;
        }

        public void AddElement(VisualElement element)
        {
            element.style.display = DisplayStyle.None;
            _elements.Add(element);
            Add(element);
        }
    }
}