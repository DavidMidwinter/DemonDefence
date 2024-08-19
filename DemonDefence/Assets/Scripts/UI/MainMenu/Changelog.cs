using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

public static class Changelog
{
    static VisualElement changeLog;

    public static VisualElement getChangeLog()
    {
        if (changeLog is null) generateChangelog();
        return changeLog;
    }

    static void generateChangelog()
    {
        changeLog = UITools.Create("page", "white-border");

        TextAsset logData = (TextAsset)Resources.Load("changelog");

        ScrollView changelogWindow = UITools.Create<ScrollView>("change-log");

        Label logText = UITools.Create<Label>("instruction-text");
        logText.text = logData.text;
        changelogWindow.Add(logText);

        Label header = UITools.Create<Label>("header-text");

        header.text = $"Changelog - Version x";

        changeLog.Add(header);
        changeLog.Add(changelogWindow);
        changeLog.Add(MainMenu.Instance.backButton());
    }
}
