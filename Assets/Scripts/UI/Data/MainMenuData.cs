using UI.Controllers;
using UnityEngine.UIElements;

namespace UI.Data
{
    internal static class MainMenuData
    {
        public static UIDocument Document 
        { 
            set
            {
                PlayButton = value.rootVisualElement.Q<Button>(name: "PlayButton");
                OptionsButton = value.rootVisualElement.Q<Button>(name: "OptionsButton");
                QuitButton = value.rootVisualElement.Q<Button>(name: "QuitButton");
            } 
        }

        public static Button PlayButton 
        {
            set { value.clicked += MainMenuController.StartGame; } 
        }

        public static Button OptionsButton;

        public static Button QuitButton 
        {
            set { value.clicked += MainMenuController.ExitGame; }
        }
    }
}
