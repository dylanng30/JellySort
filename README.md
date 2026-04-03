## 🎥 Gameplay Demo

[![Jelly Sort Gameplay Demo](https://img.youtube.com/vi/6NX5xH-LOsY/maxresdefault.jpg)](https://www.youtube.com/watch?v=6NX5xH-LOsY)

*(Click vào ảnh trên để xem video Demo trực tiếp trên YouTube)*

---

# 🧩 Jelly Sort - 3D Hexa Puzzle Game

**Jelly Sort** là một tựa game giải đố 3D dạng sắp xếp các khối lục giác (Hexa Puzzle), được phát triển bằng Unity. Dự án không chỉ tập trung vào gameplay cốt lõi mang tính giải trí cao mà còn được xây dựng dựa trên các nền tảng kiến trúc phần mềm chuẩn mực, đảm bảo hiệu năng, khả năng mở rộng và dễ dàng bảo trì.

## 🎮 Tính năng nổi bật (Features)

* **Core Gameplay:** Kéo thả và sắp xếp các khối thạch (Jelly) lục giác lên bàn cờ. Tự động hợp nhất (merge) các khối cùng màu.
* **Hệ thống Lưới Hexa (Hexa Grid):** Thuật toán tính toán tọa độ và quản lý node lục giác chính xác.
* **Vật phẩm bổ trợ (Boosters):** Đa dạng các loại booster để hỗ trợ người chơi như *Bomb* (bom), *Hammer* (búa), *Rocket* (tên lửa), và *Reverse* (hoàn tác).
* **Hệ thống Kinh tế & Tiến trình:** Quản lý tiền tệ (Coins), Mạng chơi (Lives), và Hệ thống Cửa hàng (Shop).
* **Tiến trình cấp độ (Level Progression):** Cấu hình màn chơi linh hoạt thông qua ScriptableObjects (Level_01 đến Level_07+).
* **Trải nghiệm Nghe nhìn (Audio/Visual):** Tích hợp DOTween cho hoạt ảnh UI/Gameplay mượt mà, hệ thống Particle VFX khi vỡ băng/hợp nhất, và Haptic Feedback (rung phản hồi).

---

## 🛠 Phân tích Kỹ thuật & Kiến trúc (Technical Architecture)

Dự án được xây dựng với tư duy hướng đối tượng (OOP) vững chắc và tuân thủ chặt chẽ các nguyên tắc **SOLID**, kết hợp cùng nhiều **Design Patterns** kinh điển trong phát triển game để tạo ra một codebase sạch (Clean Architecture).

### 1. Kiến trúc Hệ thống (System Architecture)
* **Event-Driven Architecture (EventBus):** Các module trong game giao tiếp với nhau thông qua hệ thống `EventBus` (ví dụ: `LevelCompletedEvent`, `GamePausedEvent`, `JellyEvents`). Điều này giúp giảm thiểu sự phụ thuộc lẫn nhau (loose coupling) giữa các hệ thống, giúp dự án dễ dàng mở rộng.
* **Service Locator Pattern:** Thay vì lạm dụng Singleton (anti-pattern), dự án sử dụng `ServiceLocator` để quản lý và phân phối các service (`IService`) và manager (`IManager`) một cách an toàn trong toàn bộ vòng đời của game.
* **Data-Driven Design (ScriptableObjects):** Tách biệt hoàn toàn dữ liệu và logic. Cấu hình level (`LevelConfig`, `LevelSetupSO`), dữ liệu vật phẩm (`Boosters`, `ShopConfigSO`), và âm thanh (`SoundLibrarySO`) đều được thiết kế dưới dạng ScriptableObjects, giúp Game Designer dễ dàng tinh chỉnh mà không cần chạm vào code.

### 2. Design Patterns & OOP
* **Object Pooling Pattern:** Tối ưu hóa bộ nhớ và tránh Garbage Collection (GC) spikes gây giật lag bằng cách tái sử dụng các object xuất hiện với tần suất cao như *HexaItem*, *VFX (IceBrokeVFX)*, *FloatingText* thông qua `PoolManager` và giao diện `IPoolable`.
* **State Machine Pattern:** Quản lý trạng thái logic của trò chơi rõ ràng và mạch lạc bằng `StateMachine` và các lớp `StateBase` (thông qua `GameplayStateController`).
* **Factory / Command Pattern:** Kiến trúc Core hỗ trợ khởi tạo đối tượng linh hoạt (`FactoryBase`) và đóng gói các hành động thành command (`CommandBase`), tạo nền tảng vững chắc cho hệ thống Undo và xử lý logic của các Booster.
* **Singleton Pattern:** Được sử dụng một cách rất hạn chế và có kiểm soát cho các hệ thống thực sự cần tồn tại duy nhất và xuyên suốt (như `GameManager`, `AudioManager`).

### 3. Nguyên tắc SOLID áp dụng
* **S (Single Responsibility Principle):** Mỗi Manager/Controller chịu trách nhiệm duy nhất cho một domain. Ví dụ: `EconomyManager` chỉ lo tiền tệ, `LivesManager` chỉ lo mạng sống, `ComboManager` chỉ tính chuỗi combo.
* **O (Open/Closed Principle):** Hệ thống Booster (`BaseBooster`) cho phép dễ dàng mở rộng thêm các kỹ năng mới (kế thừa thành `BombBooster`, `HammerBooster`,...) mà không cần sửa đổi mã nguồn logic cốt lõi.
* **I (Interface Segregation):** Codebase sử dụng các interface nhỏ, đặc thù như `ITickable`, `ILateTickable`, `IManager`, `IService` để các class chỉ phải implement những gì chúng thực sự cần.
* **D (Dependency Inversion):** Các class bậc cao giao tiếp với các module bậc thấp thông qua Interface và Service Locator thay vì phụ thuộc vào các class cụ thể (concrete classes).

---

## 📂 Cấu trúc Thư mục (Folder Structure)

```text
Assets/
├── 01_ART/                  # Chứa tài nguyên đồ họa (3D Models, Materials, Fonts, UI Sprites)
├── 02_SCENES/               # Chứa các Unity Scenes (Gameplay scene chính)
├── 03_SCRIPTS/              # Mã nguồn C# (Được phân chia theo Domain)
│   ├── Dylanng/Core/        # Core framework (Base classes, EventBus, ServiceLocator, Pooling)
│   └── JellySort/           # Game logic cụ thể của dự án
│       ├── Data/            # Các định nghĩa ScriptableObject
│       ├── Events/          # Định nghĩa Event cụ thể (JellyEvents)
│       ├── Gameplay/        # Logic bàn cờ (Grid, HexaStack, Boosters)
│       ├── Managers/        # Controllers & Managers (Camera, Level, Economy...)
│       └── UI/              # Hệ thống User Interface (HUD, Animations, Popups)
├── 04_PREFABS/              # Chứa các Prefab sẵn sàng sử dụng (HexaNode, Stack, VFX...)
├── 05_IMPORTED_PACKAGES/    # Các thư viện bên thứ 3 (DOTween, TextMeshPro)
├── Data/                    # Các file dữ liệu cấu hình .asset (Levels, Boosters, Shop)
└── Resources/               # Các tài nguyên cần load động lúc runtime
