public class EntityCommand
{
    /// <summary>
    /// Value that determines entity ID, please change
    /// this value back to the original of the tick wich
    /// you're loading state, and also the IDNEXT of the Shape
    /// class.
    /// </summary>
    public static int IDNEXT = 0;

    ///<summary>
    ///Adds entity to processing. (duh)
    ///</summary>
    public static void Instance (Entity entity)
    {
        entity.Init();

        if(entity.Children != null) InitChildrenInternal(entity.Children);
        MainClass.AddEntity(entity);
    }

    static void InitChildrenInternal (NodeChildren<Entity> children)
    {
        for(int i = 0; i < children.Count; ++i)
        {
            var currEntity = children[i];
            
            currEntity.Init();

            var eChildren = currEntity.Children;

            if(eChildren != null) InitChildrenInternal(eChildren);
        }
    }

    ///<summary>
    ///Removes entity from processing. (duh duh)
    ///</summary>
    public static void Destroy (Entity entity)
    {
        MainClass.RemoveEntity(entity);
    }

    ///<summary>
    ///Executes the a set amount of ticks in the same moment.
    ///</summary>
    public static void RollEntitiesForward (int amountOfTimes)
    {
        for (int i = 0; i< amountOfTimes ; ++i)
        {
            MainClass.ProcessEntities();
        }
    }

    public static void SetID(Entity entity, int ID)
    {
        while(entity.Parent!= null)
        {
            entity = entity.Parent;
        }

        entity.ID = ID;
        entity.IDSet = true;

        if (entity.Children != null) SetIDInternal(entity.Children, ID);
    }

    static void SetIDInternal(NodeChildren<Entity> children, int ID)
    {
        for(int i = 0; i < children.Count; ++i)
        {
            var currEntity = children[i];

            currEntity.ID = ID;

            if (currEntity.Children != null) SetIDInternal(currEntity.Children, ID);
        }
    }

    ///<summary>
    ///Rolls all entities forward except the entity in the argument.
    ///</summary>
    public static void RollEntitiesForwardExcept (int amountOfTimes, Entity entity)
    {
        bool CanProcess = entity.CanProcess;

        entity.CanProcess = false;

        RollEntitiesForward(amountOfTimes);

        entity.CanProcess = CanProcess;
    }
}