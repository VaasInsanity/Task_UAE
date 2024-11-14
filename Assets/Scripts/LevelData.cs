using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "NewLevelData", menuName = "Game/Level Data")]
public class LevelData : ScriptableObject
{
    [Header("Level Information")]
    public int levelIndex;
    public int cardAmount;
    public bool locked = true;
    public float timeLimit = 60f;

    [Header("GridLayoutGroup Settings")]
    public GridLayoutGroup.Constraint constraint = GridLayoutGroup.Constraint.FixedColumnCount;
    public int constraintCount = 2;
    public Vector2 cellSize = new Vector2(100f, 100f);

    private const string LockedKeyPrefix = "LevelLocked_";
    private const string CardAmountKeyPrefix = "LevelCardAmount_";
    private const string TimeLimitKeyPrefix = "LevelTimeLimit_";
    private const string ConstraintKeyPrefix = "LevelConstraint_";
    private const string ConstraintCountKeyPrefix = "LevelConstraintCount_";
    private const string CellSizeXKeyPrefix = "LevelCellSizeX_";
    private const string CellSizeYKeyPrefix = "LevelCellSizeY_";

    public void SaveLevelData()
    {
        PlayerPrefs.SetInt(LockedKeyPrefix + levelIndex, locked ? 1 : 0);
        PlayerPrefs.SetInt(CardAmountKeyPrefix + levelIndex, cardAmount);
        PlayerPrefs.SetFloat(TimeLimitKeyPrefix + levelIndex, timeLimit);

        PlayerPrefs.SetInt(ConstraintKeyPrefix + levelIndex, (int)constraint);
        PlayerPrefs.SetInt(ConstraintCountKeyPrefix + levelIndex, constraintCount);
        PlayerPrefs.SetFloat(CellSizeXKeyPrefix + levelIndex, cellSize.x);
        PlayerPrefs.SetFloat(CellSizeYKeyPrefix + levelIndex, cellSize.y);

        PlayerPrefs.Save();
    }

    public void LoadLevelData()
    {
        if (PlayerPrefs.HasKey(LockedKeyPrefix + levelIndex))
        {
            locked = PlayerPrefs.GetInt(LockedKeyPrefix + levelIndex) == 1;
        }

        if (PlayerPrefs.HasKey(CardAmountKeyPrefix + levelIndex))
        {
            cardAmount = PlayerPrefs.GetInt(CardAmountKeyPrefix + levelIndex);
        }

        if (PlayerPrefs.HasKey(TimeLimitKeyPrefix + levelIndex))
        {
            timeLimit = PlayerPrefs.GetFloat(TimeLimitKeyPrefix + levelIndex);
        }

        if (PlayerPrefs.HasKey(ConstraintKeyPrefix + levelIndex))
        {
            constraint = (GridLayoutGroup.Constraint)PlayerPrefs.GetInt(ConstraintKeyPrefix + levelIndex);
        }

        if (PlayerPrefs.HasKey(ConstraintCountKeyPrefix + levelIndex))
        {
            constraintCount = PlayerPrefs.GetInt(ConstraintCountKeyPrefix + levelIndex);
        }

        if (PlayerPrefs.HasKey(CellSizeXKeyPrefix + levelIndex))
        {
            cellSize.x = PlayerPrefs.GetFloat(CellSizeXKeyPrefix + levelIndex);
        }

        if (PlayerPrefs.HasKey(CellSizeYKeyPrefix + levelIndex))
        {
            cellSize.y = PlayerPrefs.GetFloat(CellSizeYKeyPrefix + levelIndex);
        }
    }
    public void ClearLevelData()
    {
        PlayerPrefs.DeleteKey(LockedKeyPrefix + levelIndex);
        PlayerPrefs.DeleteKey(CardAmountKeyPrefix + levelIndex);
        PlayerPrefs.DeleteKey(TimeLimitKeyPrefix + levelIndex);

        PlayerPrefs.DeleteKey(ConstraintKeyPrefix + levelIndex);
        PlayerPrefs.DeleteKey(ConstraintCountKeyPrefix + levelIndex);
        PlayerPrefs.DeleteKey(CellSizeXKeyPrefix + levelIndex);
        PlayerPrefs.DeleteKey(CellSizeYKeyPrefix + levelIndex);
    }
}
