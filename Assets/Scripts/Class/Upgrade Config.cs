using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "NewSubjectConfig", menuName = "Configs/Config Upgrade")]
public class UpgradeConfig : ScriptableObject
{
    public string id;              // ID duy nhất
    public string name;            // Tên nâng cấp
    public float baseValue;        // Giá trị cơ bản, dùng cho Level 0 hoặc làm hệ số nhân

    [Tooltip("Trục X là Cấp độ, Trục Y là Chi phí để đạt cấp độ đó")]
    public AnimationCurve costCurve;

    [Tooltip("Trục X là Cấp độ, Trục Y là Tổng giá trị (Value) tại cấp độ đó")]
    public AnimationCurve valueCurve;
}