using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using Pelki.Gameplay.Inventories.Items;
using Pelki.Gameplay.Puzzles;
using Pelki.Gameplay.SaveSystem;
using UnityEngine;

namespace Pelki
{
    public class Level : MonoBehaviour, ISerializationCallbackReceiver
    {
        [SerializeField] private List<SavePointDto> savePoints;
        [SerializeField] private List<PuzzleKeyDto> puzzleKeys;

        [Dropdown(nameof(GetAllSavepointIds))]
        [SerializeField] private string characterSpawnSavePointId;

        private Dictionary<SavePoint, string> savePointIdsRegister;
        private Dictionary<string, SavePoint> savePointsRegister;
        private Dictionary<string, PickUpItem> puzzleKeysRegister;
        private Dictionary<string, IPuzzle> puzzleRegister;

        public IReadOnlyDictionary<SavePoint, string> SavePointIdsRegister => savePointIdsRegister;
        public IReadOnlyDictionary<string, SavePoint> SavePointsRegister => savePointsRegister;
        public IReadOnlyDictionary<string, PickUpItem> PuzzleKeysRegister => puzzleKeysRegister;
        public IReadOnlyDictionary<string, IPuzzle> PuzzlesRegister => puzzleRegister;

        public string CharacterSpawnSavePointId => characterSpawnSavePointId;

        private void Awake()
        {
            foreach (var (id, item) in puzzleKeysRegister)
            {
                item.Initialize(id);
            }
        }

        public IEnumerable<string> GetAllSavepointIds()
        {
            IEnumerable<string> result = SavePointIdsRegister.Values.ToList();
            return result;
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            //do nothing
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            savePointIdsRegister = savePoints.ToDictionary(dto => dto.SavePoint, dto => dto.ID);
            savePointsRegister = savePoints.ToDictionary(dto => dto.ID, dto => dto.SavePoint);

            puzzleKeysRegister = puzzleKeys.ToDictionary(dto => dto.ID, dto => dto.PuzzleKey);
            puzzleRegister = puzzleKeys.ToDictionary(dto => dto.ID, dto => dto.Puzzle);
        }

        [Serializable]
        private class SavePointDto
        {
            [SerializeField] private SavePoint savePoint;
            [SerializeField] private string id;

            public SavePoint SavePoint => savePoint;
            public string ID => id;
        }

        [Serializable]
        private class PuzzleKeyDto
        {
            [SerializeField] private PickUpItem puzzleKey;
            [SerializeField] private CirclePuzzle puzzle;
            [SerializeField] private string id;

            public PickUpItem PuzzleKey => puzzleKey;
            public IPuzzle Puzzle => puzzle;
            public string ID => id;
        }
    }
}