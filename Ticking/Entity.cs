public interface Entity
{
    bool IsDrawable {get;}

    bool IsTickable{get;}

    //Indicates wether or not the entity has been terminated
    bool IsDestroyed{get;set;}

    //What will execute when it is instanced, even before next frame.

    int ZValue {get;set;}

    void EnterTree();

    void Tick ();

    DrawableObject GetDrawable ();
}