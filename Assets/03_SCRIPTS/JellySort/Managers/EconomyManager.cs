using Dylanng.Core;
using Dylanng.Core.Base;
using JellySort.Events;
using JellySort.Data;
using UnityEngine;

namespace JellySort.Managers
{
    public class EconomyManager : ManagerBase
    {
        public int CurrentCoins => ServiceLocator.Get<SaveLoadManager>().Data.Coins;

        public override void Initialize()
        {
            ServiceLocator.Register<EconomyManager>(this);
            EventBus.Subscribe<LevelCompletedEvent>(OnLevelCompleted);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            EventBus.Unsubscribe<LevelCompletedEvent>(OnLevelCompleted);
            ServiceLocator.Unregister<EconomyManager>();
        }

        private void OnLevelCompleted(LevelCompletedEvent evt)
        {
            var levelManager = ServiceLocator.Get<LevelManager>();
            if (levelManager != null && levelManager.CurrentLevel != null)
            {
                int rewardCoins = levelManager.CurrentLevel.RewardCoins;
                
                if (rewardCoins > 0)
                {
                    AddCoins(rewardCoins);
                }
            }
        }

        public void AddCoins(int amount)
        {
            var saveManager = ServiceLocator.Get<SaveLoadManager>();
            saveManager.Data.Coins += amount;
            saveManager.Save();
            
            GameLogger.Log($"EconomyManager: Added {amount} coins. Current: {saveManager.Data.Coins}");
            
            EventBus.Publish(new CoinsChangedEvent { NewAmount = saveManager.Data.Coins });
        }

        public bool TryPurchaseBooster(BoosterType type, int price, int amount)
        {
            var saveManager = ServiceLocator.Get<SaveLoadManager>();
            if (saveManager.Data.Coins < price)
            {
                GameLogger.LogWarning("EconomyManager: Trình trạng thiếu tiền!");
                EventBus.Publish(new ShopPurchaseEvent { Success = false, Type = type, Message = "Không đủ tiền!" });
                return false;
            }
            
            saveManager.Data.Coins -= price;
            
            saveManager.UpdateBoosterCount(type, amount);
            
            saveManager.Save();
            EventBus.Publish(new CoinsChangedEvent { NewAmount = saveManager.Data.Coins });
            EventBus.Publish(new ShopPurchaseEvent { Success = true, Type = type, Message = "Mua thành công!" });
            
            GameLogger.Log($"EconomyManager: Purchased {amount}x {type} for {price} coins.");
            return true;
        }
    }
}
