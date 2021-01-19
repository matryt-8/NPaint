﻿using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace NPaint.Figures
{
    class NEllipse : Figure
    {
        protected Point CenterPoint;
        public NEllipse(Path adaptedPath) : base(adaptedPath)
        {
            adaptedGeometry = new EllipseGeometry();
            adaptedPath.Data = adaptedGeometry;
        }

        public override void SetFields(Path path)
        {
            adaptedPath = path;
            adaptedGeometry = path.Data;

            CenterPoint = ((EllipseGeometry)adaptedGeometry).Center;

            SetPointCollection();
        }
        public override void ChangeBorderThickness(double value)
        {
            if (((EllipseGeometry)adaptedGeometry).RadiusX == 0 || ((EllipseGeometry)adaptedGeometry).RadiusY == 0){}
            else if (this.GetCenterPoint().Y - this.GetRadiusY() - GetBorderThickness() / 2 <= 0 && value > adaptedPath.StrokeThickness)
                value = adaptedPath.StrokeThickness;

            adaptedPath.StrokeThickness = value;
        }
        public override void ChangeBorderThicknessInsideGroup(double value, PointCollection pointCollectionOfSelection)
        {
            if (GetCenterPoint().X + GetRadiusX() + value / 2 > pointCollectionOfSelection[1].X
                || GetCenterPoint().X - GetRadiusX() - value / 2 < pointCollectionOfSelection[0].X
                || GetCenterPoint().Y + GetRadiusY() + value / 2 > pointCollectionOfSelection[1].Y
                || GetCenterPoint().Y - GetRadiusY() - value / 2 < pointCollectionOfSelection[0].Y)
            {
                return;
            }
            else
            {
                adaptedPath.StrokeThickness = value;
            }
        }
        public override void Draw(Point startPoint, Point currentPoint)
        {
            // obliczenie polozenia elipsy na osi XY
            CenterPoint = MidPoint(currentPoint, startPoint);

            // obliczenie wysokosci i szerokosci elipsy
            double width = Math.Max(currentPoint.X, startPoint.X) - CenterPoint.X;
            double height = Math.Max(currentPoint.Y, startPoint.Y) - CenterPoint.Y;

            // przypisanie wyliczonych wartosci do zmiennej (geometrii)
            EllipseGeometry tmp = adaptedGeometry as EllipseGeometry;
            tmp.Center = CenterPoint;
            tmp.RadiusX = width;
            tmp.RadiusY = height;

            Repaint();
        }
        public override void MoveBy(Point point)
        {
             double x = point.X;
             double y = point.Y;
            
            CenterPoint.X = x;
            CenterPoint.Y = y;
            ((EllipseGeometry)adaptedGeometry).Center = CenterPoint;

            Repaint();
        }
        public override void MoveByInsideGroup(Vector shiftVector)
        {
            double x = CenterPoint.X - shiftVector.X;
            double y = CenterPoint.Y - shiftVector.Y;
            CenterPoint.X = x;
            CenterPoint.Y = y;
            ((EllipseGeometry)adaptedGeometry).Center = CenterPoint;

            Repaint();
        }
        public override void IncreaseSize()
        {
            EllipseGeometry tmp = adaptedGeometry as EllipseGeometry;

            // zabezpieczenie, zebysmy nie weszli na Menu
            if (tmp.Center.Y - tmp.RadiusY - adaptedPath.StrokeThickness/2 > 0)
            {
                tmp.RadiusX++;
                tmp.RadiusY++;
                Repaint();
            }
        }
        public override void DecreaseSize()
        {
            EllipseGeometry tmp = adaptedGeometry as EllipseGeometry;

            // zabezpieczenie, zeby rozmiary elipsy nie spadly ponizej 0
            if(tmp.RadiusX >= 1 && tmp.RadiusY >= 1)
            {
                tmp.RadiusX--;
                tmp.RadiusY--;

                Repaint();
            }
        }

        protected override void Repaint()
        {
            adaptedPath.Data = adaptedGeometry;
            SetPointCollection();
        }
        protected override void SetPointCollection()
        {
            // do zaznaczania elipsy wystarcza dwa rogi
            Rect rect = ((EllipseGeometry)adaptedGeometry).Bounds;// protokat w ktory jest "wpisana" elipsa
            PointsList.Clear();
            PointsList.Add(rect.TopLeft);     // lewy gorny
            PointsList.Add(rect.BottomRight); // prawy dolny
        }

        public Point GetCenterPoint()
        {
            return ((EllipseGeometry)this.adaptedGeometry).Center;
        }

        protected Point MidPoint(Point a, Point b)
        {
            Point tmp;
            tmp.X = (a.X + b.X) / 2;
            tmp.Y = (a.Y + b.Y) / 2;
            return tmp;
        }
        private double GetRadiusX()
        {
            return ((EllipseGeometry)adaptedGeometry).RadiusX;
        }
        private double GetRadiusY()
        {
            return ((EllipseGeometry)adaptedGeometry).RadiusY;
        }
    }
}
