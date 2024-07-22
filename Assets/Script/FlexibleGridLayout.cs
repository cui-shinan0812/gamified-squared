using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlexibleGridLayout : LayoutGroup
{
    [SerializeField] private enum FitType
    {
        Uniform,
        Width,
        Height,
        FixedRows,
        FixedColumns,
    }

    [SerializeField] private FitType fitType;
    
    [SerializeField] private int rows;
    [SerializeField] private int columns;
    [SerializeField] private Vector2 cellSize;
    [SerializeField] private Vector2 spacing;
    [SerializeField] private bool fitX;
    [SerializeField] private bool fitY;
    
    public override void CalculateLayoutInputVertical()
    {
    }
    
    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal();

        if (fitType == FitType.Width || fitType == FitType.Height || fitType == FitType.Uniform)
        {
            float sqrRt = Mathf.Sqrt(transform.childCount);

            rows = Mathf.CeilToInt(sqrRt);
            columns = Mathf.CeilToInt(sqrRt);   
        }

        if (fitType == FitType.Width || fitType == FitType.FixedColumns)
        {
            rows = Mathf.CeilToInt(transform.childCount / (float)columns);
        }

        if (fitType == FitType.Height || fitType == FitType.FixedRows)
        {
            columns = Mathf.CeilToInt(transform.childCount / (float)rows);
        }

        float parentWidth = rectTransform.rect.width;
        float parentHeight = rectTransform.rect.height;

        float cellWidth = parentWidth / (float)columns - ((spacing.x / (float)columns) * 2) - (padding.left / (float) columns) - (padding.right / (float) columns);
        float cellHeight = parentHeight / (float)rows - ((spacing.y / (float)rows) * 2) - (padding.top / (float) rows) - (padding.bottom / (float) rows);

        cellSize.x = fitX ? cellWidth : cellSize.x;
        cellSize.y = fitX ? cellHeight : cellSize.y;

        int columnsCount = 0;
        int rowsCount = 0;

        for (int i = 0; i < rectChildren.Count; i++)
        {
            rowsCount = i / columns;
            columnsCount = i % columns;

            var item = rectChildren[i];

            var xPos = (cellSize.x * columnsCount) + (spacing.x * columnsCount) + padding.left;
            var yPos = (cellSize.y * rowsCount) + (spacing.y * rowsCount) + padding.top;

            SetChildAlongAxis(item, 0, xPos, cellSize.x);
            SetChildAlongAxis(item, 1, yPos, cellSize.y);
        }
    }

    public override void SetLayoutHorizontal()
    {
    }

    public override void SetLayoutVertical()
    {
    }
}
