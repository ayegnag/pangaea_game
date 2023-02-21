using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace KOI
{
    public class GameStateManager : MonoBehaviour
    {
        public static GameStateManager Instance { get; private set; }
        public MapSystem MapSystem { get; private set; }
        public EntitySystem EntitySystem { get; private set; }

        public static event EventHandler<OnTickArgs> OnTick;
        private int _tick;
        private float _tickTimer;

        private void Awake()
        {
            EnforceSingletonInstance();
            MapSystem = new MapSystem();
            EntitySystem = new EntitySystem();
            _tick = 0;
            _tickTimer = 0;
        }

        private void Start() {
            MapSystem.Init();
            EntitySystem.Init();
        }

        private void EnforceSingletonInstance()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else{
                Instance = this;
            }
        }

        void Update()
        {
            _tickTimer += Time.deltaTime;
            if (_tickTimer >= GameConfig.TickDuration)
            {
                _tick ++;
                _tickTimer -= GameConfig.TickDuration;

                OnTick?.Invoke(this, new OnTickArgs {Tick = _tick});
            }
        }

        private void OnDisable() {
            MapSystem.Quit();
            EntitySystem.Quit();
        }
    }
}