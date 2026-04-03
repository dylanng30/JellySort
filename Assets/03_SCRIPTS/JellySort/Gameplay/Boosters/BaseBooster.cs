using System;
using UnityEngine;
using JellySort.Data;

namespace JellySort.Gameplay.Boosters
{
    public abstract class BaseBooster : ScriptableObject
    {
        [Header("Booster Config")]
        public BoosterType BoosterType;
        public string BoosterName;
        [TextArea] public string Description;
        public Sprite Icon;
        public bool RequiresTargetSelection;

        [Header("Visuals")]
        public ParticleSystem UseVfxPrefab;

        public abstract void Execute(BoosterContext context, Action onComplete);
    }
}
