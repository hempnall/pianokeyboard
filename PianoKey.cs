using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Media3D;

namespace PianoKeyboard
{


    public enum Note : byte
    {
        A =0,
        AS,
        B,
        C,
        CS,
        D,
        DS,
        E,
        F,
        FS,
        G,
        GS
    };


    class PianoKey
    {

        private float _origin;
        private float _height;
        private float _length;
        private float _width;

        private bool _isKeyDown = false;

        private TranslateTransform3D _keyDown = null;
        private TranslateTransform3D _keyUp = null;

        private Model3DGroup _cube = new Model3DGroup();
        private ModelVisual3D _model = new ModelVisual3D();

        private Model3DGroup CreateTriangleModel(Point3D p0, Point3D p1, Point3D p2)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();
            mesh.Positions.Add(p0);
            mesh.Positions.Add(p1);
            mesh.Positions.Add(p2);
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(2);
            Vector3D normal = CalculateNormal(p0, p1, p2);
            mesh.Normals.Add(normal);
            mesh.Normals.Add(normal);
            mesh.Normals.Add(normal);

            Material material;
            if (!IsBlackKey)
            {
                material = new DiffuseMaterial(
                new SolidColorBrush(Colors.White));
            }
            else
            {
                material = new DiffuseMaterial(
                new SolidColorBrush(Colors.Black));
            }
            Material specMat = new SpecularMaterial(Brushes.Red, 100.0);
            MaterialGroup matGrp = new MaterialGroup();

            //matGrp.Children.Add(specMat);
            //matGrp.Children.Add(material);
            
           //
            
            GeometryModel3D model = new GeometryModel3D(
                mesh,material);

            

            Model3DGroup group = new Model3DGroup();
            group.Children.Add(model);
            return group;
        }


        public bool IsNextKeyBlack
        {
            get
            {
                if (_noteShape == Note.C ||
                    _noteShape == Note.D ||
                    _noteShape == Note.F ||
                    _noteShape == Note.G ||
                    _noteShape == Note.A)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool IsBlackKey
        {
            get
            {
                if (_noteShape == Note.AS ||
                    _noteShape == Note.CS ||
                    _noteShape == Note.DS ||
                    _noteShape == Note.FS ||
                    _noteShape == Note.GS)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }



        public void Draw(Viewport3D mainViewport)
        {

 

            if (_isKeyDown )
            {
                _cube.Transform = _keyDown;
            }
            else
            {
                _cube.Transform = _keyUp;
            }

         

        }



        private Vector3D CalculateNormal(Point3D p0, Point3D p1, Point3D p2)
        {
            Vector3D v0 = new Vector3D(
                p1.X - p0.X, p1.Y - p0.Y, p1.Z - p0.Z);
            Vector3D v1 = new Vector3D(
                p2.X - p1.X, p2.Y - p1.Y, p2.Z - p1.Z);
            return Vector3D.CrossProduct(v0, v1);
        }



        public PianoKey(Viewport3D mainViewport, Byte b,float origin,float length,float height,float width)
        {
            _height = height;
            _width = width;
            _length = length;
            _origin = origin;


            

            _keyDown = new TranslateTransform3D(0, (double)-height, 0);
            _keyUp = new TranslateTransform3D(0,0,0);

            _isKeyDown = false; 

            NoteShape = (Note) (b % 12);
            NoteNumber = b;


            Point3D p0; Point3D p1; Point3D p2; Point3D p3; Point3D p4; Point3D p5; Point3D p6; Point3D p7; 

            if (!IsBlackKey)
            {
                _RightSideOfKey = _origin + _width;

                p0 = new Point3D(_origin, 0, 0);
                p1 = new Point3D(_RightSideOfKey, 0, 0);
                p2 = new Point3D(_RightSideOfKey, 0, _length);
                p3 = new Point3D(_origin, 0, _length);
                p4 = new Point3D(_origin, _height, 0);
                p5 = new Point3D(_RightSideOfKey, _height, 0);
                p6 = new Point3D(_RightSideOfKey, _height, _length);
                p7 = new Point3D(_origin, _height, _length);

                
            }
            else
            {


                if (_noteShape == Note.AS || _noteShape == Note.DS)
                {
                    _origin = _origin + (0.866F * _width);
                    _RightSideOfKey = _origin + (0.6F * _width);
                }

                if (_noteShape == Note.CS || _noteShape == Note.FS)
                {
                    _origin = _origin + (0.5F * _width);
                    _RightSideOfKey = _origin + (0.6F * _width);
                }

                if (_noteShape == Note.GS)
                {
                    _origin = _origin + (0.7F * _width);
                    _RightSideOfKey = _origin + (0.6F * _width);
                }

                _length = _length * 0.75F;
                float yheight = 0.4F;

                p0 = new Point3D(_origin, yheight, 0);
                p1 = new Point3D(_RightSideOfKey, yheight, 0);
                p2 = new Point3D(_RightSideOfKey, yheight, _length);
                p3 = new Point3D(_origin, yheight, _length);
                p4 = new Point3D(_origin, yheight + _height, 0);
                p5 = new Point3D(_RightSideOfKey, yheight + _height, 0);
                p6 = new Point3D(_RightSideOfKey, yheight + _height, _length);
                p7 = new Point3D(_origin, yheight + _height, _length);
            }

            //front side triangles
            _cube.Children.Add(CreateTriangleModel(p3, p2, p6));
            _cube.Children.Add(CreateTriangleModel(p3, p6, p7));
            //right side triangles
            _cube.Children.Add(CreateTriangleModel(p2, p1, p5));
            _cube.Children.Add(CreateTriangleModel(p2, p5, p6));
            //back side triangles
            _cube.Children.Add(CreateTriangleModel(p1, p0, p4));
            _cube.Children.Add(CreateTriangleModel(p1, p4, p5));
            //left side triangles
            _cube.Children.Add(CreateTriangleModel(p0, p3, p7));
            _cube.Children.Add(CreateTriangleModel(p0, p7, p4));
            //top side triangles
            _cube.Children.Add(CreateTriangleModel(p7, p6, p5));
            _cube.Children.Add(CreateTriangleModel(p7, p5, p4));
            //bottom side triangles
            _cube.Children.Add(CreateTriangleModel(p2, p3, p0));
            _cube.Children.Add(CreateTriangleModel(p2, p0, p1));


            
            _model.Content = _cube;
            mainViewport.Children.Add(_model);
        }

        private Byte _noteNumber;

        public Byte NoteNumber
        {
            get { return _noteNumber; }
            set { _noteNumber = value; }
        }

        private Note _noteShape;

        public Note NoteShape
        {
            get { return _noteShape; }
            set { _noteShape = value; }
        }


        private bool _KeyDown;

        public bool KeyDown
        {
            get { return _isKeyDown; }
            set { _isKeyDown = value; }
        }

        private float _RightSideOfKey;

        public float RightSideOfKey
        {
            get { return _RightSideOfKey; }
            set { _RightSideOfKey = value; }
        }
    }
}
