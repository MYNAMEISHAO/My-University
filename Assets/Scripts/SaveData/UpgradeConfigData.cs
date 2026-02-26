using System.Collections.Generic;
using System;
using UnityEngine; // Cần cho [Serializable]

public class UpgradeConfigData
{
    // Dữ liệu chung của người chơi
    public int playerMoney = 0;
    public int gameTimePlayed = 0;

    public UpgradeConfig staffUpgrades;
    public UpgradeConfig bookUpgrades;
    public UpgradeConfig roomUpgrades;
    public UpgradeConfig seatUpgrades;
    public static UpgradeConfigData instance;

    public static UpgradeConfigData Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new UpgradeConfigData();
            }
            return instance;
        }
    }

    //public UpgradeConfigData()
    //{
    //    Initialize();
    //}

    //void Initialize()
    //{
    //    // Khởi tạo dữ liệu mặc định nếu cần
    //    playerMoney = 1000;
    //    gameTimePlayed = 0;

    //    staffUpgrades = new UpgradeConfig(
    //        "UC01",
    //        "Staff Upgrade",
    //        5,
    //        1.0f,
    //        new AnimationCurve(new Keyframe(1, 2), new Keyframe(5, 32)),
    //        new AnimationCurve(new Keyframe(1, 10), new Keyframe(5, 30))
    //        );

    //    // 2. Nâng cấp Phòng/Cơ sở vật chất (Room/Facility)
    //    bookUpgrades = new UpgradeConfig(
    //        "UC02",
    //        "Book Upgrade",
    //        60,
    //        1.0f,
    //        new AnimationCurve(new Keyframe(1, 150), new Keyframe(20, 5000), new Keyframe(40, 40000), new Keyframe(60, 250000)),
    //        new AnimationCurve(new Keyframe(1, 15), new Keyframe(20, 150), new Keyframe(40, 700), new Keyframe(60, 850))
    //        );

    //    // 3. Nâng cấp Sách/Kiến thức (Book/Knowledge)
    //    roomUpgrades = new UpgradeConfig(
    //        "UC03",
    //        "Room Upgrade",
    //        60,
    //        1.0f,
    //        new AnimationCurve(new Keyframe(1, 300), new Keyframe(20, 5000), new Keyframe(40, 50000), new Keyframe(60, 500000)),
    //        new AnimationCurve(new Keyframe(1, 50), new Keyframe(20, 150), new Keyframe(40, 300), new Keyframe(60, 1500))
    //        );

    //    seatUpgrades = new UpgradeConfig(
    //        "UC04",
    //        "Seat Upgrade",
    //        16,
    //        1.0f,
    //        new AnimationCurve(new Keyframe(1, 200), new Keyframe(5,3000), new Keyframe(9,25000), new Keyframe(16,200000)),
    //        new AnimationCurve(new Keyframe(1, 1), new Keyframe(16, 16))
    //        );
    //}


}