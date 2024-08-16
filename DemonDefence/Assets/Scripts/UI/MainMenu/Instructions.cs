using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class Instructions : MonoBehaviour
{
    /// <summary>
    /// Control all UI functionality for the Tactical UI
    /// </summary>
    public static Instructions Instance;
    [SerializeField] private UIDocument _document;
    [SerializeField] private StyleSheet _styleSheet;
    VisualElement root => _document.rootVisualElement;
    public int pageNumber;
    public List<VisualElement> pages;

    VisualElement gameSettings;

    VisualElement detachmentPage;
    VisualElement pageDisplay => root.Q<VisualElement>(className: "instruction-pages");

    Button startButton => root.Q<Button>(className: "start-button");

    private void Awake()
    {
        pages = new List<VisualElement>();
        Instance = this;
        pageNumber = 0;
        StartCoroutine(GenerateInstructionUI());


    }
    private void OnValidate()
    {
        if (Application.isPlaying) return;
        pages = new List<VisualElement>();
        Instance = this;
        pageNumber = 0;
        StartCoroutine(GenerateInstructionUI(-1));
    }

    public IEnumerator GenerateInstructionUI(int startpage = 0)
    {
        /// Generate the instruction UI
        TextAsset mytxtData = (TextAsset)Resources.Load("instructionpages");
        List<Texture2D> images = new List<Texture2D>(Resources.LoadAll<Texture2D>(Path.Combine("Images", "instructions")));
        var txt = mytxtData.text.Split("- ");
        Debug.Log($"Generate instruction UI");
        yield return null;
        root.Clear();

        root.styleSheets.Add(_styleSheet);

        var container = UITools.Create("container", "text-block");

        VisualElement textBlock = UITools.Create("instruction-pages");

        Label header = UITools.Create<Label>("header-text");
        header.text = "Hell Broke Loose";
        container.Add(header);
        createPages(txt, images);
        Debug.Log(pages.Count);
        detachmentPage = UnitStats.getStatPage();
        pages.Add(detachmentPage);
        gameSettings = BattleSettings.getBattleSettingsPage();
        pages.Add(gameSettings);
        container.Add(textBlock);
        VisualElement buttons = createButtonDisplay();
        pageNumber = startpage >= 0 ? startpage : pages.Count + startpage;
        container.Add(buttons);
        root.Add(container);
        startButton.SetEnabled(false);
        loadPage();

    }

    private VisualElement createButtonDisplay()
    {
        VisualElement buttons = UITools.Create("buttons");
        Button startButton = UITools.Create("Start", startGame, "instruction-ui-button", "start-button");
        Button prevPage = UITools.Create("Previous", loadPreviousPage, "instruction-ui-button", "previous-button");
        Button nextPage = UITools.Create("Next", loadNextPage, "instruction-ui-button", "next-button");
        buttons.Add(prevPage);
        buttons.Add(startButton);
        buttons.Add(nextPage);
        return buttons;
    }
    private void createPages(string[] txt, List<Texture2D> images)
    {
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

            var instruction = UITools.Create<TextElement>("instructions");
            instruction.text = txt[i];
            page.Add(instruction);
            var page_number = createPageNumberDisplay(i + 1, txt.Length + 2);
            page.Add(page_number);
            pages.Add(page);
        }
    }

    private VisualElement createPageNumberDisplay(int pageNumber, int maxPages)
    {
        /// Creates a page number display
        /// Args:
        ///     int pageNumber: This page's Page Number
        ///     int maxPages: The total number of pages
        var page_number = UITools.Create<TextElement>("instructions", "page-number");
        page_number.text = $"{pageNumber}/{maxPages}";
        return page_number;
    }
    private void startGame()
    {
        /// Start the game
        /// 
        SceneManager.LoadScene("Tactical");
    }

    public void canStart()
    {
        try
        {
            if (pageNumber < pages.Count - 1)
            {
                startButton.SetEnabled(false);
                return;
            }

            if (BattleSettings.numberOfEnemyDetachments <= 0
                    || BattleSettings.numberOfEnemyDetachments > BattleSettings.maxEnemyDetachments)
            {
                startButton.SetEnabled(false);
                return;
            }

            if (BattleSettings.numberOfPlayerDetachments <= 0
                    || BattleSettings.numberOfPlayerDetachments > BattleSettings.maxPlayerDetachments)
            {
                startButton.SetEnabled(false);
                return;

            }

            startButton.SetEnabled(true);
        }
        catch (NullReferenceException) { 
            return; 
        }

    }

    private void loadPage()
    {
        Debug.Log(pageNumber);
        pageDisplay.Clear();
        pageDisplay.Add(pages[pageNumber]);
        canStart();
    }
    private void loadNextPage()
    {
        if (pageNumber >= pages.Count - 1)
            return;

        pageNumber++;
        loadPage();
    }

    private void loadPreviousPage()
    {
        Debug.Log(pageNumber);
        if (pageNumber <= 0)
            return;

        pageNumber--;
        loadPage();
    }
}