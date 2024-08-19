using System;
using UnityEngine.UIElements;

public static class UITools
{
    public static Button Create(string buttonText, Action methodToCall, params string[] classNames)
    {
        /// Create a button element
        /// Args:
        ///     string buttonText: The text for the button
        ///     Action methodToCall: The method to attach to the button
        ///     params string[] classNames: List of class names
        /// Returns:
        ///     Button with the given classes, text and method
        Button btn = Create<Button>(classNames);
        btn.text = buttonText;
        btn.RegisterCallback<MouseUpEvent>((evt) => methodToCall());
        return btn;
    }
    public static VisualElement Create(params string[] classNames)
    {
        /// Create a visual element
        /// Args:
        ///     params string[] classNames: List of class names
        /// Returns:
        ///     VisualElement with the given classes
        return Create<VisualElement>(classNames);
    }

    public static T Create<T>(params string[] classNames) where T : VisualElement, new()
    {
        /// Create a UI element
        /// Args:
        ///     params string[] classNames: List of class names
        /// Returns:
        ///     Element of type T with the given classes
        var ele = new T();
        foreach (var className in classNames)
        {
            ele.AddToClassList(className);
        }

        return ele;
    }
}
