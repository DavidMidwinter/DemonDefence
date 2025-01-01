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

    VisualElement loadFileMenu;

    VisualElement newFileMenu;

    VisualElement optionsMenu;

    VisualElement mainMenu;
    // Start is called before the first frame update

    void Start()
    {
        Instance = this;
        optionsMenu = OptionsMenu.getOptionsMenu(backButton());
        loadFileMenu = MapLoad.getMapLoadPage(true);
        newFileMenu = MapNew.getMapNewPage(true);
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
        root.Add(UITools.Create("container", "menu-container", "main-menu-container", "main", "page-display"));
        root.styleSheets.Add(_styleSheet);
        generateMainMenu();
        display.Add(mainMenu);
    }
    
    void generateMainMenu()
    {
        mainMenu = UITools.Create("main-menu");
        Label header = UITools.Create<Label>("subheader-text");
        header.text = "Hell Broke Loose";
        Label subheader = UITools.Create<Label>("header-text");
        subheader.text = "Map Maker";
        Button newFileMenu = UITools.Create("New File", loadNewMapPage, "main-menu-button", "load-file-page");
        Button loadFileMenu = UITools.Create("Load File", loadLoadMapPage, "main-menu-button", "load-file-page");
        Button optionsMenu = UITools.Create("Options", loadOptionsPage, "main-menu-button", "options-page");
        Button exitGame = UITools.Create("Exit", Utils.exit, "main-menu-button", "exit-game");

        VisualElement buttonDisplay = UITools.Create("main-menu-button-display");
        buttonDisplay.Add(newFileMenu);
        buttonDisplay.Add(loadFileMenu);
        buttonDisplay.Add(optionsMenu);
        buttonDisplay.Add(exitGame);

        mainMenu.Add(header);
        mainMenu.Add(subheader);
        mainMenu.Add(buttonDisplay);
    }

    public Button backButton()
    {
        Button backButton = UITools.Create("Main Menu", back, "back-button");
        return backButton;
    }
    public Button loadEditorButton(string name = "Load Editor")
    {
        Button button = UITools.Create(name, loadEditor, "back-button", "load-file-button");
        return button;
    }

    void loadEditor()
    {
        Utils.loadEditor();
    }

    void back()
    {
        load(mainMenu);
    }

    void loadOptionsPage()
    {
        load(optionsMenu);
    }
    void loadLoadMapPage()
    {
        load(loadFileMenu);
    }
    void loadNewMapPage()
    {
        load(newFileMenu);
    }


    void load(VisualElement page)
    {
        Debug.Log($"Load {page}");
        display.Clear();
        display.Add(page);
    }
    public void setValue(string lookup, int value)
    {
        Debug.Log(lookup);
        PaintStartData.setPaintSettingValues(lookup, value);
    }
    public void setValue(string lookup, string value)
    {
        Debug.Log(lookup);
        PaintStartData.setPaintSettingValues(lookup, value);
    }


}
