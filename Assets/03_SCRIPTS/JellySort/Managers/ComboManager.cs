using Dylanng.Core;
using Dylanng.Core.Base;
using JellySort.Events;

namespace JellySort.Managers
{
    public class ComboManager : ManagerBase
    {
        private int _currentCombo;

        public override void Initialize()
        {
            ServiceLocator.Register<ComboManager>(this);
            
            EventBus.Subscribe<StackEndDragEvent>(OnStackDropped);
            EventBus.Subscribe<HexaMergeCompletedEvent>(OnMergeCompleted);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            EventBus.Unsubscribe<StackEndDragEvent>(OnStackDropped);
            EventBus.Unsubscribe<HexaMergeCompletedEvent>(OnMergeCompleted);
            ServiceLocator.Unregister<ComboManager>();
        }

        private void OnStackDropped(StackEndDragEvent evt)
        {
            _currentCombo = 0;
        }

        private void OnMergeCompleted(HexaMergeCompletedEvent evt)
        {
            _currentCombo++;
            
            int pointsEarned = evt.PoppedCount * _currentCombo;
            
            EventBus.Publish(new ComboAchievedEvent
            {
                ComboCount = _currentCombo,
                PointsEarned = pointsEarned,
                MergePosition = evt.MergePosition
            });
            
            GameLogger.Log($"Combo x{_currentCombo}! Tăng thêm {pointsEarned} điểm.");
        }
    }
}