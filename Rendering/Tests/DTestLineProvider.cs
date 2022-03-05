public class DTestLineProvider : Entity
{
    //test for drawable entity

    public bool IsDrawable{get => true;}

    public bool IsTickable{get => true;}

    public bool IsDestroyed{get;set;}

    public Vector2[] line = null;

    public void EnterTree()
    {

    }

    public void Tick ()
    {

    }

    public DrawableObject GetDrawable ()
    {
        if (line == null) return null;
        return new DrawableTestLine(line[0], line[1]);
    }
}