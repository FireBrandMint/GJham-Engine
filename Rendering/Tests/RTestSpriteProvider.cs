using System;
using SFML.System;
using System.Collections.Generic;
using System.Diagnostics;
using GJham.Rendering.Optimization;

public class RTestSpriteProvider : RenderEntity
{
    bool HasTexture = false;

    string _TexturePath = "";

    bool TextureChanged = false;

    public string TexturePath
    {
        get
        {
            return _TexturePath;
        }
        set
        {
            if (inTree)
            {
                if(_TexturePath != "" && _TexturePath != null)
                {
                    TextureHolder.UnregisterTextureRef(ref _TexturePath);
                    HasTexture = false;
                }
            
                if (value != "" && value != null)
                {
                    TextureHolder.RegisterTextureRef(ref value, 5000);
                    HasTexture = true;
                }

                TextureChanged = true;
            }

            _TexturePath = value;
        }
    }

    public bool IsStatic = false;

    Vector2 LastPosition;

    public Vector2 Position = new Vector2();

    public FInt Rotation = (FInt)0;

    public bool Rotate = true;

    bool inTree = false;

    public Vector2u TextureAreaTopLeft = new Vector2u();
    public Vector2u TextureAreaBottomRight = new Vector2u();

    public bool BoundriesSet = false;

    ConvexPolygon poly;

    static List<Shape> PolyList = new List<Shape>(1000);

    static bool p = true;
    bool player;

    CullingAABB RenderOpt = new CullingAABB(new AABB(new Vector2(0, 0), new Vector2(50, 50)));

    public override void Init()
    {
        base.Init();
        LastPosition = Position;

        RenderOpt.ChangePosition(Position - new Vector2(25, 25));

        if(_TexturePath != null && _TexturePath != "")
        {
            TextureHolder.RegisterTextureRef(ref _TexturePath, 5000);
            HasTexture = true;
        }

        poly = ConvexPolygon.CreateRect(new Vector2((FInt) 50, (FInt) 50), new FInt(), Position);
        
        PolyList.Add(poly);

        if(p)
        {
            p = false;
            player = true;
        } else player = false;


        if(!player)
        {
            FInt rot = (FInt) 0;

            poly.Rotation = rot;

            Rotation = rot;

            Processable = false;
        }
        else
        {
            IsStatic = false;
        }

        inTree = true;
    }

    public override void Tick()
    {
        base.Tick();

        //if(!Rotate) return;

        //Rotation += 1;

        //if(Rotation > 360) Rotation -= 360;

        if(player)
        {
            LastPosition = Position;

            var inputs = new int[]
            {
                (int)SFML.Window.Keyboard.Key.W,
                (int)SFML.Window.Keyboard.Key.A,
                (int)SFML.Window.Keyboard.Key.S,
                (int)SFML.Window.Keyboard.Key.D,
            };

            FInt speed = FInt.Create(10);

            foreach(int inp in MainClass.CurrentKeys)
            {
                if(inp == inputs[0])
                {
                    Position += new Vector2(0, -speed);
                }
                if(inp == inputs[1])
                {
                    Position += new Vector2(-speed, 0);
                }
                if(inp == inputs[2])
                {
                    Position += new Vector2(0, speed);
                }
                if(inp == inputs[3])
                {
                    Position += new Vector2(speed, 0);
                }
            }

            poly.Position = Position;

            bool collided = false;

            /*CollisionResult result = new CollisionResult();

            for(int i = 0; i < PolyList.Count; ++i)
            {
                var curr = PolyList[i];
                
                if(curr == poly) continue;

                result.Intersects = false;

                poly.IntersectsInfo(curr, ref result);

                if(result.Intersects)
                {
                    FInt factor;

                    factor.RawValue = 4140L;

                    Position = Position + result.Separation * factor;

                    poly.Position = Position;

                    collided = true;
                }
            }*/

            var stopwatch = Stopwatch.StartNew();

            var colResults = Shape.GetShapesInGrid(poly);

            var colCount = colResults.Length;

            if(colCount != 0)
            {
                CollisionResult result = new CollisionResult();

                for(int i = 0; i < colCount; ++i)
                {
                    var curr = colResults[i];

                    if(curr == poly) continue;

                    result.Intersects = false;

                    poly.IntersectsInfo(curr, ref result);

                    if(result.Intersects)
                    {
                        Position += result.Separation;

                        poly.Position = Position;

                        collided = true;
                    }
                }
            }

            if(collided)
            {
                double time = ((double) stopwatch.ElapsedTicks / Stopwatch.Frequency) * 1000;

                Console.WriteLine($"PLAYER INTERSECTS! TOOK {time}MS!");
            }
            stopwatch.Stop();

            RenderOpt.ChangePosition(Position - new Vector2(25, 25));

            if(Camera.MainCamera != null)
            {
                Camera.MainCamera.Position = Position;
            }

            string posMsg = $"In position {Position.ToString()}";

            AntiConsoleSpam.antiConsoleSpam.WriteLine(ref posMsg, 1032, 30);
        }
    }

    DrawableSprite2D drawable = null;
    bool dInitialized = false;

    public override DrawableObject GetDrawable ()
    {
        if(!HasTexture) return null;

        if(!dInitialized)
        {
            drawable = new DrawableSprite2D(
                ZValue, TexturePath,
                LastPosition, Position, new Vector2(1, 1),
                new Vector2u[]{TextureAreaTopLeft, TextureAreaBottomRight},
                BoundriesSet, Rotation, IsStatic);
            
            dInitialized = true;
        }
        else
        {
            drawable.SetPosValues(Position, LastPosition);
            drawable.z = ZValue;

            if(TextureChanged) drawable.ChangeTexturePath(_TexturePath);

            drawable.SetRotation(Rotation);
        }

        return drawable;
    }

    public override void LeaveTree()
    {
        //Unregisters texture reference.
        TexturePath = null;
        
        inTree = false;

        if(drawable != null)
        {
            drawable.DisposeResources();
            drawable = null;
        }

        RenderOpt.Dispose();
    }

    protected override bool Tickable()
    {
        return true;
    }

    protected override bool TrulyVisible() => RenderOpt.CanRender();
}