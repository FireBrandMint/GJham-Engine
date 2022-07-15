public interface Entity
{
    ///<summary>
    ///Wether or not this entity is possibly drawable.
    ///Cannot be changed once set.
    ///</summary>
    bool IsDrawable {get;}

    ///<summary>
    ///Wether or not this entity is possibly able to tick.
    ///Cannot be changed once set.
    ///</summary>
    bool IsTickable{get;}

    ///<summary>
    ///If this values is set to false, the entity will not tick.
    ///</summary>
    bool CanProcess{get;set;}

    ///<summary>
    ///True if the entity was terminated.
    ///</summary>
    bool IsDestroyed{get;set;}

    ///<summary>
    ///Render order value.
    ///</summary>

    int ZValue {get;set;}

    ///<summary>
    ///Function that is executed BEFORE ALL THE OTHERS
    //when the object is instanced to ensure that the object is able to function.
    ///</summary>
    void Init();

    ///<summary>
    ///Function that is executed when the object is instanced.
    ///</summary>
    void EnterTree();

    ///<summary>
    ///Function that is executed when the object is terminated.
    ///</summary>
    void LeaveTree();

    ///<summary>
    ///Function that is executed every tick while the object is instanced. (duh)
    ///</summary>
    void Tick ();

    ///<summary>
    ///Gets the object to draw from entities that are drawable.
    ///</summary>
    DrawableObject GetDrawable ();
}