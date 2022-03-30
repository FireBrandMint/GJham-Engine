using SFML.Graphics;

public class DTestLineProvider : Entity
{
    //test for drawable entity

    public bool IsDrawable{get => true;}

    public bool IsTickable{get => true;}

    public bool IsDestroyed{get;set;}

    public bool CantProcess{get;set;}

    public int ZValue {get;set;}

    public Vector2[] line = null;

    Vector2 Position = new Vector2();

    Vector2 LastPosition;

    public void Init()
    {
        LastPosition = Position;
    }

    public void EnterTree()
    {

    }

    int t = 0;

    public void Tick ()
    {
        LastPosition = Position;

        int factor = (((int)Engine.MaxTPS) / 10);
        
        if (t < 20)
        {
            Position += new Vector2((FInt)10,(FInt)0);

            ++t;
        }
        else
        {
            Position -= new Vector2((FInt)t * 10,(FInt)0);

            t=0;
        }
    }

    public DrawableObject GetDrawable ()
    {
        if (line == null) return null;
        return new DrawableLine2D(line[0], line[1], Color.Cyan, Color.Red, Position, LastPosition);
    }
}