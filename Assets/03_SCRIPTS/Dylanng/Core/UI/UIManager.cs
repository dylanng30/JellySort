using System;
using System.Collections.Generic;
using UnityEngine;
using Dylanng.Core.Base;
using Dylanng.Core.UI.Popups;
using Dylanng.Core.UI.Screens;

namespace Dylanng.Core.UI
{
    public class UIManager : ManagerBase
    {
        private readonly Stack<UIScreen> _screenStack = new Stack<UIScreen>();
        private readonly Stack<UIPopup> _popupStack = new Stack<UIPopup>();
        private readonly Dictionary<Type, UIBase> _registeredUIs = new Dictionary<Type, UIBase>();

        public override void Initialize()
        {
            ServiceLocator.Register<UIManager>(this);

            UIBase[] uiViews = GetComponentsInChildren<UIBase>(true);
            foreach (var view in uiViews)
            {
                RegisterUI(view);
                view.Initialize();
                view.Hide();
            }
        }

        private void RegisterUI(UIBase uiView)
        {
            var type = uiView.GetType();
            if (!_registeredUIs.ContainsKey(type))
            {
                _registeredUIs.Add(type, uiView);
            }
        }

        public T OpenScreen<T>(object data = null) where T : UIScreen
        {
            var type = typeof(T);
            if (!_registeredUIs.TryGetValue(type, out var view))
            {
                GameLogger.LogError($"UI Screen {type.Name} chưa được đăng ký!");
                return null;
            }

            if (_screenStack.Count > 0)
            {
                _screenStack.Peek().Hide();
            }

            var screen = view as T;
            screen.Setup(data);
            screen.Show();
            _screenStack.Push(screen);

            return screen;
        }

        public void CloseCurrentScreen()
        {
            if (_screenStack.Count > 1)
            {
                var screen = _screenStack.Pop();
                screen.Hide();

                _screenStack.Peek().Show();
            }
        }

        public T OpenPopup<T>(object data = null) where T : UIPopup
        {
            var type = typeof(T);
            if (!_registeredUIs.TryGetValue(type, out var view))
            {
                GameLogger.LogError($"UI Popup {type.Name} chưa được đăng ký!");
                return null;
            }

            var popup = view as T;
            popup.Setup(data);
            popup.Show();
            _popupStack.Push(popup);

            return popup;
        }

        public void CloseCurrentPopup()
        {
            if (_popupStack.Count > 0)
            {
                var popup = _popupStack.Pop();
                popup.Hide();
            }
        }

        public void CloseAllPopups()
        {
            while (_popupStack.Count > 0)
            {
                CloseCurrentPopup();
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                HandleBackButton();
            }
        }

        public void HandleBackButton()
        {
            if (_popupStack.Count > 0)
            {
                CloseCurrentPopup();
            }
            else if (_screenStack.Count > 1)
            {
                CloseCurrentScreen();
            }
            else
            {
                GameLogger.Log("Không còn UI nào để đóng. Có thể hiển thị Popup hỏi 'Bạn có muốn thoát Game không?'");
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            ServiceLocator.Unregister<UIManager>();
        }
    }
}