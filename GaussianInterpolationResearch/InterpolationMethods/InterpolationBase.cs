﻿using System.Drawing;
using ZedGraph;

namespace Interpolation {
	public abstract class InterpolationBase : IInterpolation {

        protected InterpolationBase(PointPairList inputPoints) => InputPoints = inputPoints;

        public virtual PointPairList InputPoints { get; }

        public abstract PointPair GetPoint(double X);

        public abstract string Name { get; protected set; }

        public abstract Color CurveColor { get; protected set; }

        public abstract SymbolType Symbol { get; protected set; }
    }

}
