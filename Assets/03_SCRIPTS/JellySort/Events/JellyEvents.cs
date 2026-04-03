using Dylanng.Core.Base.Interfaces;
using JellySort.Gameplay.Grid;
using JellySort.Gameplay.HexaStack;
using JellySort.Data;
using UnityEngine;

namespace JellySort.Events
{
    public struct StackSpawnRequestedEvent : IEvent { }

    public struct StackSpawnCompletedEvent : IEvent { }

    public struct StackBeginDragEvent : IEvent
    {
        public DraggableStack Stack;
    }

    public struct StackEndDragEvent : IEvent
    {
        public DraggableStack Stack;
        public HexaNode TargetNode;
    }

    public struct HexaMergeCompletedEvent : IEvent
    {
        public int PoppedCount;
        public Vector3 MergePosition;
    }

    public struct LevelLoadedEvent : IEvent
    {
        public int LevelId;
    }
    
    public struct LevelCompletedEvent : IEvent
    {
        public int LevelId;
        public int Score;
        public int RewardCoins;
    }

    public struct LevelFailedEvent : IEvent
    {
        public string FailReason;
    }

    // Navigation
    public struct ReturnToMainMenuRequestedEvent : IEvent { }
    public struct RestartLevelRequestedEvent : IEvent { }

    // Levels
    public struct LevelStartRequestedEvent : IEvent
    {
        public int LevelId;
        public bool ForceRestart;
    }

    public struct ScoreChangedEvent : IEvent
    {
        public int NewScore;
        public int TargetScore;
    }

    public struct MovesChangedEvent : IEvent
    {
        public int CurrentMoves;
        public int MoveLimit;
    }

    public struct TimeChangedEvent : IEvent
    {
        public int TimeRemaining;
        public int TimeLimit;
    }

    // Booster Events
    public struct BoosterActivatedEvent : IEvent
    {
        public BoosterType Type;
    }

    public struct BoosterCanceledEvent : IEvent { }

    public struct BoosterCompletedEvent : IEvent { }

    public struct BoosterTargetModeStateChangedEvent : IEvent
    {
        public bool IsActive;
        public BoosterType ActiveBoosterType;
    }

    public struct TargetNodeSelectedForBoosterEvent : IEvent
    {
        public HexaNode Node;
    }

    public struct ForceMergeCheckEvent : IEvent
    {
        public HexaNode TargetNode;
    }

    // Economy Events
    public struct ShopOpenRequestedEvent : IEvent { }
    public struct CoinsChangedEvent : IEvent
    {
        public int NewAmount;
    }

    public struct ShopPurchaseEvent : IEvent
    {
        public bool Success;
        public BoosterType Type;
        public string Message;
    }

    public struct LivesChangedEvent : IEvent
    {
        public int CurrentLives;
        public int MaxLives;
    }

    public struct LivesRegenTimerTickEvent : IEvent
    {
        public int SecondsRemaining;
    }
    
    //Combo
    public struct ComboAchievedEvent : IEvent
    {
        public int ComboCount;
        public int PointsEarned;
        public Vector3 MergePosition;
    }
    
    public struct IceBrokenEvent : IEvent
    {
        public HexaNode Node;
        public Vector3 Position;
    }
}
