using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;

public static class Changelog
{
    static string bulletpoints = "^[*-]";
    static VisualElement changeLog;

    public static VisualElement getChangeLog()
    {
        if (changeLog is null) generateChangelog();
        return changeLog;
    }

    static void generateChangelog()
    {
        changeLog = UITools.Create("page", "white-border");
        Regex rg = new Regex(bulletpoints);
        TextAsset logData = (TextAsset)Resources.Load("changelog");

        string[] logFileText = logData.text.Split("\n");

        ScrollView changelogWindow = new ScrollView(ScrollViewMode.Vertical);
        changelogWindow.verticalScrollerVisibility = ScrollerVisibility.AlwaysVisible;
        changelogWindow.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
        changelogWindow.AddToClassList("change-log");

        foreach (string line in logFileText)
        {
            Label logText = UITools.Create<Label>("changelog-text");
            if(rg.IsMatch(line))
            {
                logText.AddToClassList("subheader-text");
                logText.text = line.Substring(1);
            }
            else
            {
                logText.text = line;
            }
            changelogWindow.Add(logText);
        }

        Label header = UITools.Create<Label>("header-text");

        header.text = $"Changelog - Version {Application.version}";

        changeLog.Add(header);
        changeLog.Add(changelogWindow);
        changeLog.Add(MainMenu.Instance.backButton());
    }
}
