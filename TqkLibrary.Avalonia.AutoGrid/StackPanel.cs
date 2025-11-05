using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Data;

namespace TqkLibrary.Avalonia.AutoGrid
{
    public class StackPanel : Panel
    {
        public static readonly StyledProperty<Orientation> OrientationProperty
            = AvaloniaProperty.Register<StackPanel, Orientation>(
                nameof(Orientation),
                defaultValue: Orientation.Vertical
                );
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }


        public static readonly StyledProperty<double> MarginBetweenChildrenProperty
            = AvaloniaProperty.Register<StackPanel, double>(
                nameof(MarginBetweenChildren),
                defaultValue: 0.0
                );
        public double MarginBetweenChildren
        {
            get { return (double)GetValue(MarginBetweenChildrenProperty); }
            set { SetValue(MarginBetweenChildrenProperty, value); }
        }


        public static readonly AttachedProperty<StackPanelFill> FillProperty =
            AvaloniaProperty.RegisterAttached<StackPanel, Control, StackPanelFill>(
                "Fill",
                defaultValue: StackPanelFill.Auto
                );

        public static void SetFill(Control element, StackPanelFill value)
        {
            element.SetValue(FillProperty, value);
        }

        public static StackPanelFill GetFill(Control element)
        {
            return (StackPanelFill)element.GetValue(FillProperty);
        }

        static StackPanel()
        {
            AffectsArrange<StackPanel>(OrientationProperty, MarginBetweenChildrenProperty);
            AffectsMeasure<StackPanel>(OrientationProperty, MarginBetweenChildrenProperty);

            AffectsMeasure<Control>(FillProperty);
            AffectsArrange<Control>(FillProperty);
            AffectsParentArrange<StackPanel>(FillProperty);
            AffectsParentArrange<StackPanel>(FillProperty);
        }



        protected override Size MeasureOverride(Size constraint)
        {
            Controls children = Children;

            double parentWidth = 0;
            double parentHeight = 0;
            double accumulatedWidth = 0;
            double accumulatedHeight = 0;

            var isHorizontal = Orientation == Orientation.Horizontal;
            var totalMarginToAdd = CalculateTotalMarginToAdd(children, MarginBetweenChildren);

            for (int i = 0; i < children.Count; i++)
            {
                Control child = children[i];

                if (child == null) { continue; }

                // Handle only the Auto's first to calculate remaining space for Fill's
                if (GetFill(child) != StackPanelFill.Auto) { continue; }

                // Child constraint is the remaining size; this is total size minus size consumed by previous children.
                var childConstraint = new Size(Math.Max(0.0, constraint.Width - accumulatedWidth),
                                               Math.Max(0.0, constraint.Height - accumulatedHeight));

                // Measure child.
                child.Measure(childConstraint);
                var childDesiredSize = child.DesiredSize;

                if (isHorizontal)
                {
                    accumulatedWidth += childDesiredSize.Width;
                    parentHeight = Math.Max(parentHeight, accumulatedHeight + childDesiredSize.Height);
                }
                else
                {
                    parentWidth = Math.Max(parentWidth, accumulatedWidth + childDesiredSize.Width);
                    accumulatedHeight += childDesiredSize.Height;
                }
            }

            // Add all margin to accumulated size before calculating remaining space for
            // Fill elements.
            if (isHorizontal)
            {
                accumulatedWidth += totalMarginToAdd;
            }
            else
            {
                accumulatedHeight += totalMarginToAdd;
            }

            var totalCountOfFillTypes = children
                .OfType<Control>()
                .Count(x => GetFill(x) == StackPanelFill.Fill && x.IsVisible);

            var availableSpaceRemaining = isHorizontal
                ? Math.Max(0, constraint.Width - accumulatedWidth)
                : Math.Max(0, constraint.Height - accumulatedHeight);

            var eachFillTypeSize = totalCountOfFillTypes > 0
                ? availableSpaceRemaining / totalCountOfFillTypes
                : 0;

            for (int i = 0; i < children.Count; i++)
            {
                Control child = children[i];

                if (child == null) { continue; }

                // Handle all the Fill's giving them a portion of the remaining space
                if (GetFill(child) != StackPanelFill.Fill) { continue; }

                // Child constraint is the remaining size; this is total size minus size consumed by previous children.
                var childConstraint = isHorizontal
                    ? new Size(eachFillTypeSize, Math.Max(0.0, constraint.Height - accumulatedHeight))
                    : new Size(Math.Max(0.0, constraint.Width - accumulatedWidth), eachFillTypeSize);

                // Measure child.
                child.Measure(childConstraint);
                var childDesiredSize = child.DesiredSize;

                if (isHorizontal)
                {
                    accumulatedWidth += childDesiredSize.Width;
                    parentHeight = Math.Max(parentHeight, accumulatedHeight + childDesiredSize.Height);
                }
                else
                {
                    parentWidth = Math.Max(parentWidth, accumulatedWidth + childDesiredSize.Width);
                    accumulatedHeight += childDesiredSize.Height;
                }
            }

            // Make sure the final accumulated size is reflected in parentSize. 
            parentWidth = Math.Max(parentWidth, accumulatedWidth);
            parentHeight = Math.Max(parentHeight, accumulatedHeight);
            var parent = new Size(parentWidth, parentHeight);

            return parent;
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            Controls children = Children;
            int totalChildrenCount = children.Count;

            double accumulatedLeft = 0;
            double accumulatedTop = 0;

            var isHorizontal = Orientation == Orientation.Horizontal;
            var marginBetweenChildren = MarginBetweenChildren;

            var totalMarginToAdd = CalculateTotalMarginToAdd(children, marginBetweenChildren);

            double allAutoSizedSum = 0.0;
            int countOfFillTypes = 0;
            foreach (var child in children.OfType<Control>())
            {
                var fillType = GetFill(child);
                if (fillType != StackPanelFill.Auto)
                {
                    if (child.IsVisible && fillType != StackPanelFill.Ignored)
                        countOfFillTypes += 1;
                }
                else
                {
                    var desiredSize = isHorizontal ? child.DesiredSize.Width : child.DesiredSize.Height;
                    allAutoSizedSum += desiredSize;
                }
            }

            var remainingForFillTypes = isHorizontal
                ? Math.Max(0, arrangeSize.Width - allAutoSizedSum - totalMarginToAdd)
                : Math.Max(0, arrangeSize.Height - allAutoSizedSum - totalMarginToAdd);
            var fillTypeSize = remainingForFillTypes / countOfFillTypes;

            for (int i = 0; i < totalChildrenCount; ++i)
            {
                Control child = children[i];
                if (child == null) { continue; }
                Size childDesiredSize = child.DesiredSize;
                var fillType = GetFill(child);
                var isCollapsed = !child.IsVisible || fillType == StackPanelFill.Ignored;
                var isLastChild = i == totalChildrenCount - 1;
                var marginToAdd = isLastChild || isCollapsed ? 0 : marginBetweenChildren;

                double x = accumulatedLeft;
                double y = accumulatedTop;
                double w = Math.Max(0.0, arrangeSize.Width - accumulatedLeft);
                double h = Math.Max(0.0, arrangeSize.Height - accumulatedTop);
                if (isHorizontal)
                {
                    w = fillType == StackPanelFill.Auto || isCollapsed ? childDesiredSize.Width : fillTypeSize;
                    h = arrangeSize.Height;
                    accumulatedLeft += w + marginToAdd;
                }
                else
                {
                    w = arrangeSize.Width;
                    h = fillType == StackPanelFill.Auto || isCollapsed ? childDesiredSize.Height : fillTypeSize;
                    accumulatedTop += h + marginToAdd;
                }

                Rect rcChild = new Rect(x, y, w, h);
                child.Arrange(rcChild);
            }

            return arrangeSize;
        }

        static double CalculateTotalMarginToAdd(Controls children, double marginBetweenChildren)
        {
            var visibleChildrenCount = children
                .OfType<Control>()
                .Count(x => x.IsVisible && GetFill(x) != StackPanelFill.Ignored);
            var marginMultiplier = Math.Max(visibleChildrenCount - 1, 0);
            var totalMarginToAdd = marginBetweenChildren * marginMultiplier;
            return totalMarginToAdd;
        }
    }
    public enum StackPanelFill
    {
        Auto,
        Fill,
        Ignored
    }
}
