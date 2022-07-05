public interface Entity
{
    bool IsDrawable {get;}

    bool IsTickable{get;}

    bool CanProcess{get;set;}

    //Indicates wether or not the entity has been terminated
    bool IsDestroyed{get;set;}

    //What will execute when it is instanced, even before next frame.

    int ZValue {get;set;}

    //Function that is executed BEFORE ALL THE OTHERS to assure that the object is 100% functionable
    void Init();

    void EnterTree();

    void LeaveTree();

    void Tick ();

    DrawableObject GetDrawable ();
}