using System.Linq;
using Pelki.Configs;
using Pelki.Gameplay.Camera;
using Pelki.Gameplay.Characters;
using Pelki.Gameplay.Inputs;
using Pelki.Gameplay.Inventories;
using Pelki.Gameplay.SaveSystem;
using Pelki.UI;
using Pelki.UI.Screens;
using UnityEngine;

namespace Pelki.Gameplay
{
    public class Game
    {
        private readonly LevelsConfig _levelsConfig;
        private readonly ScreenSwitcher _screenSwitcher;
        private readonly IInput _input;
        private readonly CharactersConfig _charactersConfig;

        private Level _level;
        private LevelProgress _levelProgress;
        private CameraDistributor _cameraDistributor;
        private PlayerCharacter _playerCharacter;
        private InventoryProgress _inventoryProgress;
        private Vector3 _spawnPosition;

        public Game(LevelsConfig levelsConfig, CharactersConfig charactersConfig, ScreenSwitcher screenSwitcher,
            IInput input, LevelProgress progress, CameraDistributor cameraDistributor, 
            InventoryProgress inventoryProgress)
        {
            _charactersConfig = charactersConfig;
            _input = input;
            _screenSwitcher = screenSwitcher;
            _levelsConfig = levelsConfig;
            _levelProgress = progress;
            _cameraDistributor = cameraDistributor;
            _inventoryProgress = inventoryProgress;
        }

        public void ThisUpdate()
        {
        }

        public void StartGame()
        {
            Level levelPrefab = _levelsConfig.DebugLevelPrefab;
            _level = Object.Instantiate(levelPrefab);
            
            InitializeSavePoints();

            foreach (var pickUpPuzzleKey in _inventoryProgress.PickedUpPuzzleKeys)
            {
                if (_level.PuzzleKeysRegister.ContainsKey(pickUpPuzzleKey))
                {
                    _level.PuzzleKeysRegister[pickUpPuzzleKey].Destroy();
                }
            }
            Inventory inventory = new Inventory(_inventoryProgress);

            _playerCharacter = Object.Instantiate(_charactersConfig.PlayerCharacterPrefab,
                _spawnPosition,
                Quaternion.identity, _level.transform);
            _playerCharacter.Construct(_input, inventory);

            _cameraDistributor.SetTargetFollow(_playerCharacter);

            _screenSwitcher.ShowScreen<GameScreen>();
        }

        private void InitializeSavePoints()
        {
            SavePoint spawnSavePoint = _level.SavePointsRegister[_levelProgress.LastSavePointId];
            _spawnPosition = spawnSavePoint.transform.position;
            foreach (var savePointItem in _level.SavePointIdsRegister)
            {
                if (savePointItem.Key.Equals(spawnSavePoint)
                    //sttrox: ActivatedSavePoints является list, что не супер оптимизированно, но она вроде как тут и не нужна
                    && _levelProgress.ActivatedSavePoints.Contains(savePointItem.Value))
                {
                    spawnSavePoint.ActivateState();
                }
                else
                {
                    savePointItem.Key.NotActivateState();
                    savePointItem.Key.Saved += OnSaved;
                }
            }
        }

        private void OnSaved(SavePoint savePoint)
        {
            var savePointId = _level.SavePointIdsRegister[savePoint];
            _levelProgress.AddActivatedSavePoint(savePointId);
            _levelProgress.Save();
            Debug.Log("Save on savepoint with ID: " + savePointId);
        }
    }
}