public class EntityCommand
{
    ///<summary>
    ///Adds entity to processing. (duh)
    ///</summary>
    public static void Instance (Entity entity)
    {
        MainClass.AddEntity(entity);
        entity.Init();
        entity.EnterTree();
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