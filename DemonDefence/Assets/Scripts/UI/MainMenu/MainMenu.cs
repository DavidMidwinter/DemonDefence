using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour
{
    public static MainMenu Instance;
    [SerializeField] private UIDocument _document;
    [SerializeField] private StyleSheet _styleSheet;
    VisualElement root => _document.rootVisualElement;
    VisualElement display => root.Q(className: "main");

    VisualElement gameSettings;

    VisualElement detachmentPage;

    VisualElement instructionsPages;

    VisualElement changeLog;

    VisualElement mainMenu;
    // Start is called before the first frame update

    void Start()
    {
        Instance = this;
        gameSettings = BattleSettings.getBattleSettingsPage(true);
        detachmentPage = UnitStats.getStatPage(true);
        instructionsPages = Instructions.getInstructionsPages(true);
        changeLog = Changelog.getChangeLog(true);
        StartCoroutine(setUpMenu());
    }

    private void OnValidate()
    {
        if (Application.isPlaying) return;
        StartCoroutine(setUpMenu());
    }

    public IEnumerator setUpMenu()
    {
        yield return null;
        root.Add(UITools.Create("container", "text-block", "main", "page-display"));
        root.styleSheets.Add(_styleSheet);
        generateMainMenu();
        display.Add(mainMenu);
    }
    
    void generateMainMenu()
    {
        mainMenu = UITools.Create("main-menu");
        Label header = UITools.Create<Label>("header-text");
        header.text = "Hell Broke Loose";
        Button skirmishMode = UITools.Create("Skirmish Mode", loadBattleUI, "main-menu-button", "skirmish-mode");
        Button unitInformation = UITools.Create("Unit Stats", loadDetachmentUI, "main-menu-button", "skirmish-mode");
        Button instructionsInformation = UITools.Create("Instructions", loadInstructionUI, "main-menu-button", "skirmish-mode");
        Button changeLog = UITools.Create("Changelog", loadChangelogUI, "main-menu-button", "changelog-page");
        Button exitGame = UITools.Create("Exit", exit, "main-menu-button", "exit-game");

        VisualElement buttonDisplay = UITools.Create("main-menu-button-display");
        buttonDisplay.Add(skirmishMode);
        buttonDisplay.Add(instructionsInformation);
        buttonDisplay.Add(unitInformation);
        buttonDisplay.Add(changeLog);
        buttonDisplay.Add(exitGame);

        mainMenu.Add(header);
        mainMenu.Add(buttonDisplay);
    }

    public Button backButton()
    {
        Button backButton = UITools.Create("Main Menu", back, "back-button");
        return backButton;
    }

    void back()
    {
        load(mainMenu);
    }

    void loadBattleUI()
    {
        load(gameSettings);
    }

    void loadInstructionUI()
    {
        load(instructionsPages);
    }

    void loadDetachmentUI()
    {
        load(detachmentPage);
    }
    void loadChangelogUI()
    {
        load(changeLog);
    }


    void load(VisualElement page)
    {
        Debug.Log($"Load {page}");
        display.Clear();
        display.Add(page);
    }

    void exit()
    {
        Application.Quit();
    }


}
