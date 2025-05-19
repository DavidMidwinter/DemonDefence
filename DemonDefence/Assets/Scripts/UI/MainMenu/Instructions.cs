using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

public static class Instructions
{
    /// <summary>
    /// Control all UI functionality for the Tactical UI
    /// </summary>
    public static int pageNumber;
    public static List<VisualElement> pages;
    private static VisualElement instructionsPages;

    public static VisualElement getInstructionsPages(bool forceGenerate = false)
    {
        if (forceGenerate || instructionsPages is null) GenerateInstructionUI();
        return instructionsPages;
    }
    private static void GenerateInstructionUI()
    {
        /// Generate the instruction UI
        TextAsset mytxtData = (TextAsset)Resources.Load("instructionpages");
        List<Texture2D> images = new List<Texture2D>(Resources.LoadAll<Texture2D>(Path.Combine("Images", "instructions")));
        var txt = mytxtData.text.Split("- ");
        Debug.Log($"[Instructions]: Generate instruction UI");

        instructionsPages = UITools.Create("page-display");

        VisualElement textBlock = UITools.Create();

        createPages(txt, images);
        Debug.Log($"[Instructions]: {pages.Count}");
        pageNumber = 0;

        instructionsPages.Add(textBlock);
        loadPage();

    }

    private static VisualElement createButtonDisplay()
    {
        VisualElement buttons = UITools.Create("buttons");
        Button prevPage = UITools.Create("Previous", loadPreviousPage, "instruction-ui-button", "previous-button");
        Button nextPage = UITools.Create("Next", loadNextPage, "instruction-ui-button", "next-button");
        buttons.Add(prevPage);
        buttons.Add(nextPage);
        buttons.Add(MainMenu.Instance.backButton());
        return buttons;
    }
    private static void createPages(string[] txt, List<Texture2D> images)
    {
        pages = new List<VisualElement>();
        for (int i = 0; i < txt.Length; i++)
        {
            var page = UITools.Create("page", "white-border");
            if (i < images.Count)
            {
                VisualElement imgDisplay = UITools.Create("image-display");
                Image img = UITools.Create<Image>("instruction-image");
                img.scaleMode = ScaleMode.ScaleToFit;
                img.image = images[i];
                imgDisplay.Add(img);
                page.Add(imgDisplay);
            }


            ScrollView instructionText = new ScrollView(ScrollViewMode.Vertical);
            instructionText.verticalScrollerVisibility = ScrollerVisibility.AlwaysVisible;
            instructionText.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
            instructionText.AddToClassList("instruction-box");

            foreach(string line in txt[i].Split("\n"))
            {
                var instruction = UITools.Create<TextElement>("instructions");
                instruction.text = line;
                instructionText.Add(instruction);
            }

            page.Add(instructionText);
            var page_number = createPageNumberDisplay(i + 1, txt.Length);
            page.Add(page_number);
            pages.Add(page);
        }
    }

    
    private static VisualElement createPageNumberDisplay(int pageNumber, int maxPages)
    {
        /// Creates a page number display
        /// Args:
        ///     int pageNumber: This page's Page Number
        ///     int maxPages: The total number of pages
        ///     
        VisualElement pageNumberDisplay = UITools.Create("page-number", "white-border");
        var page_number = UITools.Create<TextElement>("instructions", "page-number-text");
        page_number.text = $"{pageNumber}/{maxPages}";
        VisualElement buttons = createButtonDisplay();
        pageNumberDisplay.Add(page_number);
        pageNumberDisplay.Add(buttons);
        return pageNumberDisplay;
    }

    private static void loadPage()
    {
        Debug.Log($"[Instructions]: {pageNumber}");
        instructionsPages.Clear();
        instructionsPages.Add(pages[pageNumber]);
    }
    private static void loadNextPage()
    {
        if (pageNumber >= pages.Count - 1)
            return;

        pageNumber++;
        loadPage();
    }

    private static void loadPreviousPage()
    {
        Debug.Log($"[Instructions]: {pageNumber}");
        if (pageNumber <= 0)
            return;

        pageNumber--;
        loadPage();
    }
}