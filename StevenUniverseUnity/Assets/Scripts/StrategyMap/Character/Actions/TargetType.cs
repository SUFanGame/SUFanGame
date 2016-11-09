
/// <summary>
/// Target Type determines which selections are valid when choosing the target for an action( moving, attacking, etc )
/// </summary>
[System.Flags]
public enum TargetType
{
    NONE =  0,
    POINT = 1 << 0,
    SELF =  1 << 1,
    ENEMY = 1 << 2,
    ALLY =  1 << 3,
    ALL =   ~0,
}