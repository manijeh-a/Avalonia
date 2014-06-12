﻿// -----------------------------------------------------------------------
// <copyright file="Decorator.cs" company="Steven Kirk">
// Copyright 2014 MIT Licence. See licence.md for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Perspex.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Linq;

    public class Decorator : Control, IVisual
    {
        public static readonly PerspexProperty<Control> ContentProperty =
            PerspexProperty.Register<Decorator, Control>("Content");

        public static readonly PerspexProperty<Thickness> PaddingProperty =
            PerspexProperty.Register<Decorator, Thickness>("Padding");

        public Decorator()
        {
            this.GetObservableWithHistory(ContentProperty).Subscribe(x =>
            {
                if (x.Item1 != null)
                {
                    ((IVisual)x.Item1).VisualParent = null;
                    ((ILogical)x.Item1).LogicalParent = null;
                }

                if (x.Item2 != null)
                {
                    ((IVisual)x.Item2).VisualParent = this;
                    ((ILogical)x.Item2).LogicalParent = this;
                }
            });
        }

        public Control Content
        {
            get { return this.GetValue(ContentProperty); }
            set { this.SetValue(ContentProperty, value); }
        }

        public Thickness Padding
        {
            get { return this.GetValue(PaddingProperty); }
            set { this.SetValue(PaddingProperty, value); }
        }

        IEnumerable<IVisual> IVisual.ExistingVisualChildren
        {
            get { return ((IVisual)this).VisualChildren; }
        }

        IEnumerable<IVisual> IVisual.VisualChildren
        {
            get 
            {
                Control content = this.Content;
                return Enumerable.Repeat(content, content != null ? 1 : 0);
            }
        }

        protected override Size ArrangeContent(Size finalSize)
        {
            Control content = this.Content;

            if (content != null)
            {
                content.Arrange(new Rect(finalSize).Deflate(this.Padding));
            }

            return finalSize;
        }

        protected override Size MeasureContent(Size availableSize)
        {
            double width = 0;
            double height = 0;

            if (this.Visibility != Visibility.Collapsed)
            {
                Control content = this.Content;

                if (content != null)
                {
                    content.Measure(availableSize);
                    Size s = content.DesiredSize.Value.Inflate(this.Padding);
                    width = s.Width;
                    height = s.Height;
                }

                if (this.Width > 0)
                {
                    width = this.Width;
                }

                if (this.Height > 0)
                {
                    height = this.Height;
                }
            }

            return new Size(width, height);
        }
    }
}
