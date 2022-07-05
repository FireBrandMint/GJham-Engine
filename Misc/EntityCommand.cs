public class EntityCommand
{
    public static void Instance (Entity entity)
    {
        MainClass.AddEntity(entity);
        entity.Init();
        entity.EnterTree();
    }

    public static void Destroy (Entity entity)
    {
        MainClass.RemoveEntity(entity);
    }

    //Executed the a set amount of ticks in the same moment
    public static void RollEntitiesForward (int amountOfTimes)
    {
        for (int i = 0; i< amountOfTimes ; ++i)
        {
            MainClass.ProcessEntities();
        }
    }

    //Rolls all entities forward except the entity in the argument
    public static void RollEntitiesForwardExcept (int amountOfTimes, Entity entity)
    {
        bool CanProcess = entity.CanProcess;

        entity.CanProcess = false;

        RollEntitiesForward(amountOfTimes);

        entity.CanProcess = CanProcess;
    }
}