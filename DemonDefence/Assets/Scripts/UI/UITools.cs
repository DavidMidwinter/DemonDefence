using System;
using UnityEngine.UIElements;
using System.Collections.Generic;
using UnityEngine;

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

    public static ScrollView Create(ScrollViewMode mode, params string[] classNames)
    {
        ScrollView scrollView = new ScrollView(mode);
        switch (mode)
        {
            case ScrollViewMode.Horizontal:
                scrollView.verticalScrollerVisibility = ScrollerVisibility.Hidden;
                scrollView.horizontalScrollerVisibility = ScrollerVisibility.AlwaysVisible;
                break;
            case ScrollViewMode.Vertical:
                scrollView.verticalScrollerVisibility = ScrollerVisibility.AlwaysVisible;
                scrollView.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
                break;
            default:
                scrollView.verticalScrollerVisibility = ScrollerVisibility.AlwaysVisible;
                scrollView.horizontalScrollerVisibility = ScrollerVisibility.AlwaysVisible;
                break;
        }
        scrollView.AddToClassList("unity-scroll-view__content-container");

        foreach (var className in classNames)
        {
            scrollView.AddToClassList(className);
        }

        return scrollView;
    }

    public static DropdownField CreateDropdown(string name, string initial, params string[] classNames)
    {

        DropdownField dropDown = new DropdownField(name, new List<string>{ initial }, 0);

        foreach (var className in classNames)
        {
            dropDown.AddToClassList(className);
        }
        dropDown.Q(className: "unity-base-popup-field__text").style.color = Color.black;

        return dropDown;
    }
}
