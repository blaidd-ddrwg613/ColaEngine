//Code for MainScreen
using Gum;
using Gum.Converters;
using Gum.DataTypes;
using Gum.GueDeriving;
using Gum.Managers;
using Gum.Wireframe;
using GumRuntime;
using RenderingLibrary.Graphics;
using System.Linq;
using TestGame.Components.Controls;
namespace TestGame.Screens;
partial class MainScreenRuntime : Gum.Wireframe.GraphicalUiElement
{
    [System.Runtime.CompilerServices.ModuleInitializer]
    public static void RegisterRuntimeType()
    {
        GumRuntime.ElementSaveExtensions.RegisterGueInstantiationType("MainScreen", typeof(MainScreenRuntime));
    }
    public ButtonConfirmRuntime ButtonStartGame { get; protected set; }
    public ButtonConfirmRuntime ButtonCloseGame { get; protected set; }

    public MainScreenRuntime(bool fullInstantiation = true, bool tryCreateFormsObject = true)
    {
        if(fullInstantiation)
        {
            var element = ObjectFinder.Self.GetElementSave("MainScreen");
            element?.SetGraphicalUiElement(this, global::RenderingLibrary.SystemManagers.Default);
        }
    }
    public override void AfterFullCreation()
    {
        ButtonStartGame = this.GetGraphicalUiElementByName("ButtonStartGame") as TestGame.Components.Controls.ButtonConfirmRuntime;
        ButtonCloseGame = this.GetGraphicalUiElementByName("ButtonCloseGame") as TestGame.Components.Controls.ButtonConfirmRuntime;
        CustomInitialize();
    }
    //Not assigning variables because Object Instantiation Type is set to By Name rather than Fully In Code
    partial void CustomInitialize();
}
