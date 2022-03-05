public class EntityCommand
{
    public static void Instance (Entity entity)
    {
        MainClass.AddEntity(entity);
    }

    public static void Destroy (Entity entity)
    {
        MainClass.RemoveEntity(entity);
    }
}