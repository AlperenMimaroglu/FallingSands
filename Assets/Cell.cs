using System;
using UnityEngine;

[Serializable]
public class Cell
{
    public CellType Type { get; set; }
    [SerializeField] Color emptyColor = UnityEngine.Color.black;
    [SerializeField] Color sandColor = UnityEngine.Color.yellow;
    [SerializeField] Color invalidColor = UnityEngine.Color.magenta;

    public Cell(CellType cellType)
    {
        Type = cellType;
    }

    public Color Color()
    {
        Color cellColor = Type switch
        {
            CellType.Empty => emptyColor,
            CellType.Sand => sandColor,
            CellType.Invalid => invalidColor,
            _ => throw new ArgumentOutOfRangeException()
        };

        return cellColor;
    }
}