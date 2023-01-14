using System;
using System.Collections;
using System.Collections.Generic;
using Booble.Managers;
using UnityEngine;

namespace Booble.Player
{
    public class SpawnPosition : MonoBehaviour
    {
        [SerializeField] private Controller _controller;
        [SerializeField] private List<Triplet> _spawnPositions;

        private void Start()
        {
            int i = 0;
            while (i < _spawnPositions.Count && !_spawnPositions[i].IsSatisfied)
            {
                i++;
            }

            if (i < _spawnPositions.Count)
            {
                _controller.transform.position = _spawnPositions[i].SpawnPosition;
                _controller.StopMovement();
            }
        }
    }

    [System.Serializable]
    public class Triplet
    {
        public bool IsSatisfied => SceneLoader.Instance.CheckScenes(CurrentScene, PreviousScene);

        [field: SerializeField] public string PreviousScene { get; private set; }
        [field: SerializeField] public string CurrentScene { get; private set; }
        [field: SerializeField] public Vector3 SpawnPosition { get; private set; }
    }
}
