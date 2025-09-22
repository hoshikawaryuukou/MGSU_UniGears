using UnityEngine;
using UnityEngine.UI;
using Alchemy.Inspector;

namespace UniGears.UGUIKit.Layouts
{
    public sealed class FlexibleGridLayoutGroup : LayoutGroup
    {
        public enum FitType
        {
            FixedRows,
            FixedColumns,
        }

        [Title("Layout Settings")]
        public FitType fitType;

        [ShowIf(nameof(ShowRowsField))]
        [Min(1)]
        [LabelText("Fixed Rows")]
        public int rows = 1;
        
        [ShowIf(nameof(ShowColumnsField))]
        [Min(1)]
        [LabelText("Fixed Columns")]
        public int columns = 1;

        [Title("Cell Configuration")]
        [LabelText("Spacing")]
        public Vector2 spacing;

        // Cache values to avoid repeated calculations
        [Title("Debug Info"), ReadOnly, SerializeField]
        private int m_CachedChildCount = -1;
        
        [ReadOnly, SerializeField]
        private Vector2 m_CachedRectSize = Vector2.zero;
        
        [ReadOnly, SerializeField]
        private bool m_LayoutDirty = true;

        [ReadOnly, SerializeField]
        private Vector2 m_CalculatedCellSize = Vector2.zero;

        // Helper properties for conditional display
        private bool ShowRowsField => fitType == FitType.FixedRows;
        private bool ShowColumnsField => fitType == FitType.FixedColumns;

        protected override void OnEnable()
        {
            base.OnEnable();
            m_LayoutDirty = true;
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            m_LayoutDirty = true;
        }

        public override void CalculateLayoutInputVertical()
        {
            CalculateLayoutInputHorizontal();

            int childCount = transform.childCount;
            Vector2 rectSize = rectTransform.rect.size;
            
            // Check if we need to recalculate
            if (!m_LayoutDirty && 
                m_CachedChildCount == childCount && 
                m_CachedRectSize == rectSize)
            {
                return;
            }

            m_CachedChildCount = childCount;
            m_CachedRectSize = rectSize;
            m_LayoutDirty = false;

            // Early exit if no children
            if (childCount == 0)
            {
                return;
            }

            // Calculate grid dimensions based on fit type
            CalculateGridDimensions(childCount);

            // Calculate cell dimensions (always auto-fit)
            CalculateCellDimensions(rectSize.x, rectSize.y);

            // Position all children
            PositionChildren();
        }

        private void CalculateGridDimensions(int childCount)
        {
            switch (fitType)
            {
                case FitType.FixedColumns:
                    rows = Mathf.CeilToInt(childCount / (float)columns);
                    break;

                case FitType.FixedRows:
                    columns = Mathf.CeilToInt(childCount / (float)rows);
                    break;
            }

            // Ensure minimum values
            rows = Mathf.Max(1, rows);
            columns = Mathf.Max(1, columns);
        }

        private void CalculateCellDimensions(float parentWidth, float parentHeight)
        {
            // Cache padding calculations
            float horizontalPadding = padding.left + padding.right;
            float verticalPadding = padding.top + padding.bottom;
            
            // Calculate available space for content
            float availableWidth = parentWidth - horizontalPadding;
            float availableHeight = parentHeight - verticalPadding;
            
            // Calculate spacing totals
            float totalHorizontalSpacing = spacing.x * (columns - 1);
            float totalVerticalSpacing = spacing.y * (rows - 1);
            
            // Always auto-calculate cell dimensions to fill available space
            float cellWidth = (availableWidth - totalHorizontalSpacing) / columns;
            float cellHeight = (availableHeight - totalVerticalSpacing) / rows;

            m_CalculatedCellSize = new Vector2(cellWidth, cellHeight);
        }

        private void PositionChildren()
        {
            int childrenCount = rectChildren.Count;
            
            for (int i = 0; i < childrenCount; i++)
            {
                int rowIndex = i / columns;
                int columnIndex = i % columns;

                var item = rectChildren[i];

                // Calculate position using auto-calculated cell size
                float xPos = (m_CalculatedCellSize.x + spacing.x) * columnIndex + padding.left;
                float yPos = (m_CalculatedCellSize.y + spacing.y) * rowIndex + padding.top;

                SetChildAlongAxis(item, 0, xPos, m_CalculatedCellSize.x);
                SetChildAlongAxis(item, 1, yPos, m_CalculatedCellSize.y);
            }
        }

        public override void SetLayoutHorizontal() 
        {
            m_LayoutDirty = true;
        }

        public override void SetLayoutVertical() 
        {
            m_LayoutDirty = true;
        }

        private void MarkDirty()
        {
            m_LayoutDirty = true;
            SetDirty();
        }

        public void SetFitType(FitType newFitType)
        {
            if (fitType != newFitType)
            {
                fitType = newFitType;
                MarkDirty();
            }
        }

        public void SetGridSize(int newRows, int newColumns)
        {
            if (rows != newRows || columns != newColumns)
            {
                rows = Mathf.Max(1, newRows);
                columns = Mathf.Max(1, newColumns);
                MarkDirty();
            }
        }

        public void SetSpacing(Vector2 newSpacing)
        {
            if (spacing != newSpacing)
            {
                spacing = newSpacing;
                MarkDirty();
            }
        }

        // Public properties to access calculated values
        public Vector2 CalculatedCellSize => m_CalculatedCellSize;
        public int CalculatedRows => rows;
        public int CalculatedColumns => columns;

#if UNITY_EDITOR
        [Title("Debug Actions")]
        [Button]
        private void ForceRecalculateLayout()
        {
            m_LayoutDirty = true;
            CalculateLayoutInputVertical();
            UnityEditor.EditorUtility.SetDirty(this);
        }

        [Button]
        private void ResetToDefault()
        {
            fitType = FitType.FixedColumns;
            rows = 1;
            columns = 1;
            spacing = Vector2.zero;
            MarkDirty();
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }
}