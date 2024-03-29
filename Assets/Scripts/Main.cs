using Pelki.Configs;
using Pelki.Gameplay;
using Pelki.Gameplay.Camera;
using Pelki.Gameplay.Inputs;
using Pelki.Gameplay.Inventories;
using Pelki.Gameplay.SaveSystem;
using Pelki.UI;
using Pelki.UI.Screens;
using UnityEngine;

namespace Pelki
{
    public class Main : MonoBehaviour, IMain
    {
        [SerializeField] private MainSettingsConfig mainSettingsConfig;
        [SerializeField] private ScreenSwitcher screenSwitcher;
        [SerializeField] private CameraDistributor cameraDistributor;

        private readonly GameProgressStorage gameProgressStorage = new GameProgressStorage();

        private Game game;
        private IInput input;
        private LevelProgress levelProgress;
        private InventoryProgress inventoryProgress;

        private void Awake()
        {
#if UNITY_EDITOR
            input = new InputBySimpleInputAndKeyboard(mainSettingsConfig.InputConfig);
#else
            input = new InputBySimpleInput(mainSettingsConfig.InputConfig);
#endif

            LevelProgress.Factory levelProgressFactory = new LevelProgress.Factory(gameProgressStorage);
            levelProgress = levelProgressFactory.Create(mainSettingsConfig.LevelsConfig.DebugLevelPrefab);

            InventoryProgress.Factory inventoryProgressFactory = new InventoryProgress.Factory(gameProgressStorage);
            inventoryProgress = inventoryProgressFactory.Create(mainSettingsConfig.LevelsConfig.DebugLevelPrefab);
        }

        private void Start()
        {
            var menuScreen = screenSwitcher.ShowScreen<MenuScreen>();
            menuScreen.Construct((IMain)this);

            game = new Game(mainSettingsConfig.LevelsConfig, mainSettingsConfig.CharactersConfig, screenSwitcher,
                input, levelProgress, cameraDistributor, inventoryProgress);

            //SichTM: temporal addition until game menu will be done
            StartGame();
        }

        private void Update()
        {
            game.ThisUpdate();
        }

        public void StartGame()
        {
            game.StartGame();
        }
    }
}