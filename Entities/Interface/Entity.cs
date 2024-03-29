public interface Entity
{
    /// <summary>
    /// Wether or not the entity has an alto generated
    /// negative index instead of a positive one.
    /// The negative index is intended for
    /// elements that are not a part of the gameplay
    /// so that, in a online context, a UI element for example
    /// does not take the ID of a gameplay element and desyncs
    /// the entire game.
    /// </summary>
    /// <value></value>
    bool NegativeIndexIdentity {get; set;}

    ///<summary>
    ///Is the ID already set?
    ///</summary>
    bool IDSet{get; set;}

    ///<summary>
    ///The ID of the object in the main entity list,
    ///as long as the entity is added with the same ID on the
    ///main entity list, the execution will be deterministic.
    ///IMPORTANT: Change the ID of the entity in front AKA the entity
    ///that is the parent of all potential children to change its ID.
    ///Changing the ID of the parent of all entities while its still
    ///instanced WILL lead to a crash.
    ///</summary>
    int ID {get; set;}

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

    bool IsVisible{get;set;}

    int ZValue {get;set;}

    Vector2 Position {get;set;}

    Entity Parent {get;set;}

    

    NodeChildren<Entity> Children {get;set;}

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
    ///Function that is executed every tick while the object is instanced. (duh)
    ///</summary>
    void Tick ();

    ///<summary>
    ///Function that is executed when the object is terminated.
    ///</summary>
    void LeaveTree();


    ///<summary>
    ///Gets the object to draw from entities that are drawable.
    ///</summary>
    DrawableObject GetDrawable ();
}