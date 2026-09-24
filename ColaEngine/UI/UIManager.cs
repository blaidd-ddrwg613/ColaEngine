using System.IO;
using Iguina;

namespace ColaEngine.UI;

public class UIManager
{
    public string ThemeFolderPath { get; set; }
    public RaylibRenderer UIRenderer { get;}
    
    public RaylibInput UIInput { get;}
    
    public UISystem System { get; private set; }
    
    // TODO Pass ThemeFolder Path in constructor
    public UIManager(string themeFolderPath = "resources/UI/DefaultTheme")
    {
        ThemeFolderPath = themeFolderPath;

        UIRenderer = new RaylibRenderer(ThemeFolderPath);
        UIInput = new RaylibInput();
        System = new UISystem(Path.Combine(ThemeFolderPath, "system_style.json"), UIRenderer, UIInput);
        
    }
}