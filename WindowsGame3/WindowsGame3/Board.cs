using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Foldit3D
{
    class Board
    {
        public enum BoardState { chooseEdge1, onEdge1, chooseEdge2, onEdge2,preFold, folding1, folding2 };

        public struct DividingVert
        {
            public float distance;
            public int small;
            public int big;
            public Vector3 position;
        }
        private VertexPositionNormalTexture[] vertices;
        private short[] indices;
        private int vertNum;
        private Texture2D texture;
        private Vector3 pointOnEdge;
        private DividingVert[] p;        
        private VertexPositionTexture[] ver;
        public Board one, two;
        private float angle = 0;
        private Vector3 center;
        private Effect effect;
        private Camera camera;
        private Matrix worldMatrix;
        private GraphicsDevice device;
        private InputHandler input;
        private BoardState state;

        public Board(Texture2D tex, Effect eff)
        {
            texture = tex;
            effect = eff;
        }

        #region properties

        public VertexPositionNormalTexture[] Vertices
        {
            get { return vertices; }
            set { vertices = value; }
        }

        public short[] Indices
        {
            get { return indices; }
            set { indices = value; }
        }

        public BoardState State
        {
            get { return state; }
            set { state = value; }
        }

        public Vector3 Center
        {
            get { return center; }            
        }

        public Vector3 getAxis()
        {
            Vector3 axis = p[0].position - p[1].position;
            axis.Normalize();
            return axis;
        }

        public Vector3 getAxisPoint()
        {
            return p[0].position;
        }


        public float getAngle()
        {
            return angle;
        }
        #endregion

        public void Initialize(int vNum, Vector3[] points, Vector2[] texCords)
        {
            pointOnEdge = Vector3.Zero;
            center = Vector3.Zero;
            foreach (Vector3 vec in points)
                center += vec;
            center /= vNum;
            int iCount = (vNum - 3) * 3 + 3;
            vertNum = vNum;
            
            vertices = new VertexPositionNormalTexture[vNum];            
            camera = Game1.camera;
            device = Game1.device;
            input = Game1.input;
            worldMatrix = Matrix.Identity;
            state = BoardState.chooseEdge1;

            indices = new short[iCount];
            for (int i = 0; i < vNum; i++)
            {
                vertices[i].Position = points[i];
                vertices[i].TextureCoordinate = texCords[i];
                vertices[i].Normal = Vector3.Up;
            }
            //create the indices. every 2 consecutive points makes an outer edge (x,y,z) -> xy , yz (not xz)
            short j = 0;
            for (short i = 0; i < iCount; i += 3)
            {
                indices[i] = (short)(j % vNum);
                indices[i + 1] = (short)((j + 1) % vNum);
                indices[i + 2] = (short)(((j != 4 ? j : 5) + 2) % vNum);
                j += 2;
            }
            ver = new VertexPositionTexture[3];
            p = new DividingVert[2];

        }

        //public void initLevel(List<IDictionary<string, string>> data)
        //{
        //    List<Vector3> vecs = new List<Vector3>();
        //    List<Vector2> tex = new List<Vector2>();
        //    foreach (IDictionary<string, string> item in data)
        //    {
        //       Vector3 temp = new Vector3();
        //       temp.X = Convert.To (item["x"])  ( item["x"];

        //    }
        //}

        public void Draw()
        {
            if (state == BoardState.folding1)
            {
                one.foldShape(angle);
                two.Draw();
                one.Draw();            
                angle += Game1.closeRate;
                if (angle > MathHelper.Pi - Game1.closeRate)
                    state = BoardState.folding2;
            }
            else
                if (state == BoardState.folding2)
                {
                    one.foldShape(angle);
                    two.Draw();
                    one.Draw();
                    angle -= Game1.openRate;
                    if (angle < 0)
                        state = BoardState.chooseEdge1;
                }
                else
                {

                    effect.CurrentTechnique = effect.Techniques["TexturedNoShading"];
                    effect.Parameters["xWorld"].SetValue(worldMatrix);
                    effect.Parameters["xView"].SetValue(camera.View);
                    effect.Parameters["xProjection"].SetValue(camera.Projection);
                    effect.Parameters["xTexture"].SetValue(texture);

                    foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                    {
                        pass.Apply();

                        device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0,
                            vertices.Length, indices, 0, indices.Length / 3, VertexPositionNormalTexture.VertexDeclaration);
                        device.DrawUserPrimitives(PrimitiveType.TriangleList, ver, 0, 1, VertexPositionNormalTexture.VertexDeclaration);

                    }
                }
        }

        #region Collision
        public bool collideWithEdge(Vector3 point)
        {
            DividingVert minVert = LineIntersectPoint(vertices[0].Position, vertices[1].Position, point);
            minVert.small = 0;
            minVert.big = 1;
            for (int i = 1; i < vertNum; i++)
            {
                DividingVert currentVert = LineIntersectPoint(vertices[i].Position, vertices[(i + 1) % vertNum].Position, point);
                if (currentVert.distance < minVert.distance)
                {
                    minVert = currentVert;
                    minVert.small = i;
                    minVert.big = (i + 1) % vertNum;
                }
            }
            if (minVert.distance < 2f)
            {
                pointOnEdge = minVert.position;
                ver[0].Position = new Vector3(pointOnEdge.X - 0.5f, 0, pointOnEdge.Z - 0.5f);
                ver[0].TextureCoordinate = Vector2.Zero;
                ver[1].Position = new Vector3(pointOnEdge.X, 0f, pointOnEdge.Z + 0.5f);
                ver[1].TextureCoordinate = Vector2.Zero;
                ver[2].Position = new Vector3(pointOnEdge.X + 0.5f, 0f, pointOnEdge.Z - 0.5f);
                ver[2].TextureCoordinate = Vector2.Zero;
                if ((state == BoardState.chooseEdge1) || (state == BoardState.onEdge1))
                    p[0] = minVert;
                else
                {
                    // both points not on the same edge
                    if ((p[0].small != minVert.small) && (p[0].big != minVert.big))
                        p[1] = minVert;
                    else
                        return false;
                }
                
                return true;
            }
            return false;
        }

        private DividingVert LineIntersectPoint(Vector3 v1, Vector3 v2, Vector3 point)
        {
            Vector2 A = new Vector2(v1.X, v1.Z);
            Vector2 B = new Vector2(v2.X, v2.Z);
            Vector2 p = new Vector2(point.X, point.Z);

            DividingVert edgePoint = new DividingVert();
            //get the normalized line segment vector
            Vector2 v = B - A;
            v.Normalize();

            //determine the point on the line segment nearest to the point p
            float distanceAlongLine = Vector2.Dot(p, v) - Vector2.Dot(A, v);
            Vector2 nearestPoint;
            if (distanceAlongLine < 0)
            {
                //closest point is A
                nearestPoint = A;
            }
            else if (distanceAlongLine > Vector2.Distance(A, B))
            {
                //closest point is B
                nearestPoint = B;
            }
            else
            {
                //closest point is between A and B... A + d  * ( ||B-A|| )
                nearestPoint = A + distanceAlongLine * v;
            }

            //Calculate the distance between the two points
            float actualDistance = Vector2.Distance(nearestPoint, p);
            edgePoint.distance = actualDistance;
            edgePoint.big = edgePoint.small = -1;
            edgePoint.position = new Vector3(nearestPoint.X, 0, nearestPoint.Y);
            return edgePoint;
        }

        //----------------------------------------------------------------
        // GetPickedPosition() - gets 3D position of mouse pointer
        //                     - always on the the Y = 0 plane     
        //----------------------------------------------------------------

        public Vector3 GetPickedPosition(Vector2 mousePosition)
        {

            // create 2 positions in screenspace using the cursor position. 0 is as
            // close as possible to the camera, 10 is as far away as possible
            Vector3 nearSource = new Vector3(mousePosition, 0f);
            Vector3 farSource = new Vector3(mousePosition, 1f);

            // find the two screen space positions in world space
            Vector3 nearPoint = device.Viewport.Unproject(nearSource, camera.Projection, camera.View, Matrix.Identity);

            Vector3 farPoint = device.Viewport.Unproject(farSource,
                                camera.Projection, camera.View, Matrix.Identity);

            // compute normalized direction vector from nearPoint to farPoint
            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();

            // create a ray using nearPoint as the source
            Ray r = new Ray(nearPoint, direction);

            // calculate the ray-plane intersection point
            Vector3 n = new Vector3(0f, 1f, 0f);
            Plane p = new Plane(n, 0f);

            // calculate distance of intersection point from r.origin
            float denominator = Vector3.Dot(p.Normal, r.Direction);
            float numerator = Vector3.Dot(p.Normal, r.Position) + p.D;
            float t = -(numerator / denominator);

            // calculate the picked position on the y = 0 plane
            Vector3 pickedPosition = nearPoint + direction * t;


            return pickedPosition;
        }
        #endregion

        #region Divide Shape
        public void Divide(DividingVert first, DividingVert second, out Board partOne, out Board partTwo)
        {
            //
            /* ADD TEST TO FIND IF NOT ON THE SAME EDGE*/
            //
            Board part1 = new Board(texture,effect);
            Board part2 = new Board(texture, effect);
            int p1_pNum, p2_pNum;
            p1_pNum = Math.Abs(first.big - second.small) + 3;
            if ((first.big == vertNum - 1) && (second.small == 0))
                p1_pNum -= 2;
            p2_pNum = indices.Length - p1_pNum + 2;
            Vector3[] p1_points = new Vector3[p1_pNum];
            Vector2[] p1_texCords = new Vector2[p1_pNum];
            p1_points[0] = first.position;
            p1_texCords[0] = findTexCords(first);
            p1_points[p1_pNum - 1] = second.position;
            p1_texCords[p1_pNum - 1] = findTexCords(second);
            for (int i = 0; i < p1_pNum - 2; i++)
            {
                p1_points[i + 1] = vertices[(first.big + i) % vertNum].Position;
                p1_texCords[i + 1] = vertices[(first.big + i) % vertNum].TextureCoordinate;
            }
            Vector3[] p2_points = new Vector3[p2_pNum];
            Vector2[] p2_texCords = new Vector2[p2_pNum];
            p2_points[0] = second.position;
            p2_texCords[0] = p1_texCords[p1_pNum - 1]; //alreadt calculated before a moment
            p2_points[p2_pNum - 1] = first.position;
            p2_texCords[p2_pNum - 1] = p1_texCords[0]; //alreadt calculated before a moment
            for (int i = 0; i < p2_pNum - 2; i++)
            {
                p2_points[i + 1] = vertices[(second.big + i) % vertNum].Position;
                p2_texCords[i + 1] = vertices[(second.big + i) % vertNum].TextureCoordinate;
            }
            part1.Initialize(p1_pNum, p1_points, p1_texCords);
            part2.Initialize(p2_pNum, p2_points, p2_texCords);
            // storing the smaller part in parOne , bigger -> partTwo
            if (Vector3.Distance(center,part1.center) >= Vector3.Distance(center,part2.center))
            {
                partOne = part1;
                partTwo = part2;
            } else
            {
                partOne = part2;
                partTwo = part1;
            }
        }

        private Vector2 findTexCords(DividingVert divVert)
        {
            float smallX = vertices[divVert.small].Position.X;
            float smallCordX = vertices[divVert.small].TextureCoordinate.X;
            float bigX = vertices[divVert.big].Position.X;
            float bigCordX = vertices[divVert.big].TextureCoordinate.X;
            float vertX = divVert.position.X;

            float smallZ = vertices[divVert.small].Position.Z;
            float smallCordZ = vertices[divVert.small].TextureCoordinate.Y;
            float bigZ = vertices[divVert.big].Position.Z;
            float bigCordZ = vertices[divVert.big].TextureCoordinate.Y;
            float vertZ = divVert.position.Z;

            float xSize = Math.Abs(bigX - smallX);
            float zSize = Math.Abs(bigZ - smallZ);
            float xRel = (xSize != 0 ? Math.Abs(Math.Min(smallX, bigX) - vertX) / xSize : smallX);
            float zRel = (zSize != 0 ? Math.Abs(Math.Min(smallZ, bigZ) - vertZ) / zSize : smallZ);
            float xCsize = Math.Abs(bigCordX - smallCordX);
            float zCsize = Math.Abs(bigCordZ - smallCordZ);
            float xCrel = (xCsize != 0 ? xRel * xCsize : smallCordX);
            float zCrel = (zCsize != 0 ? zRel * zCsize : smallCordZ);
            return new Vector2(xCrel, zCrel);

            //Vector2 distFromSmall = new Vector2(Math.Abs(divVert.position.X - vertices[divVert.small].Position.X),
            //                       Math.Abs(divVert.position.Z - vertices[divVert.small].Position.Z));
            //Vector2 sizeOfLine = new Vector2(Math.Abs(vertices[divVert.small].Position.X - vertices[divVert.big].Position.X),
            //                Math.Abs(vertices[divVert.small].Position.Z - vertices[divVert.big].Position.Z));
            //distFromSmall /= new Vector2((sizeOfLine.X != 0 ? sizeOfLine.X : 1), (sizeOfLine.Y != 0 ? sizeOfLine.Y : 1));
            //sizeOfLine = new Vector2(Math.Abs(vertices[divVert.small].TextureCoordinate.X - 
            //    vertices[divVert.big].TextureCoordinate.X),
            //    Math.Abs(vertices[divVert.small].TextureCoordinate.Y - vertices[divVert.big].TextureCoordinate.Y));
            //distFromSmall *= sizeOfLine;
            //if (divVert.position.X == vertices[divVert.small].TextureCoordinate.X)
            //    distFromSmall.X = vertices[divVert.small].TextureCoordinate.X;
            //if (vertices[divVert.small].TextureCoordinate.Y == vertices[divVert.big].TextureCoordinate.Y)
            //    distFromSmall.Y = vertices[divVert.small].TextureCoordinate.Y;
            //return distFromSmall;
        }

        #endregion

        #region Folding
        public void foldShape(float angle)
        {
            Vector3 axis = vertices[0].Position - vertices[vertNum - 1].Position;
            axis.Normalize();

            worldMatrix = Matrix.Identity;
            worldMatrix *= Matrix.CreateTranslation(-vertices[0].Position);
            worldMatrix *= Matrix.CreateFromAxisAngle(axis, angle);
            worldMatrix *= Matrix.CreateTranslation(vertices[0].Position);            
        }

        #endregion
        public BoardState update()
        {
            Vector3 mouse = GetPickedPosition(
                new Vector2((float)input.MouseHandler.MouseState.X, (float)input.MouseHandler.MouseState.Y));
            bool onEdge = collideWithEdge(mouse);
            if ((state == BoardState.chooseEdge1) && (onEdge))
            {               
                    if (input.MouseHandler.WasLeftButtonClicked())
                        state = BoardState.chooseEdge2;
                    else
                        state = BoardState.onEdge1;                                    
            } else
            if ((state == BoardState.onEdge1))
            {
                if (onEdge)                
                {
                    if (input.MouseHandler.WasLeftButtonClicked())
                        state = BoardState.chooseEdge2;
                } else
                    state = BoardState.chooseEdge1;
            } else
            if ((state == BoardState.chooseEdge2) && (onEdge))
            {
                if (input.MouseHandler.WasLeftButtonClicked())
                    state = BoardState.preFold;
                 else
                    state = BoardState.onEdge2;
            } else
            if ((state == BoardState.onEdge2))
            {
                if (onEdge)
                    {
                        if (input.MouseHandler.WasLeftButtonClicked())
                            state = BoardState.preFold;
                    } else
                        state = BoardState.chooseEdge2;
            } else
            if (state == BoardState.preFold)
            {                
                Divide(p[0], p[1], out one, out two);
                state = BoardState.folding1;
            }
            if ((input.MouseHandler.WasRightButtonClicked()))
            {
                state = BoardState.chooseEdge1;
                angle = 0;
            }

            return state;

        }
    }
}
