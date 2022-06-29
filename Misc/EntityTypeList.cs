using System.IO;
using System;
using System.Collections.Generic;

using System.Reflection;

using System.Linq;

delegate Entity ETypeCreate ();

public class EntityTypeList
{
    //https://stackoverflow.com/questions/752/how-to-create-a-new-object-instance-from-a-type

    public static void ListAll()
    {
        string currFile = Directory.GetCurrentDirectory() + @"\Misc\EntityTypeList.cs";

        string fileText = File.ReadAllText(currFile);

        String marker = "//Activation methods!";

        int markerIndex = fileText.LastIndexOf(marker) + marker.Length;

        fileText = fileText.Remove(markerIndex);

        //Console.WriteLine(currFile);

        Type[] array = GetTypesWithInterface(Assembly.GetExecutingAssembly()).ToArray<Type>();

        string entityName = typeof(Entity).Name;

        string list = "   private static Dictionary<String, ETypeCreate> EntityInstancers = new Dictionary<string, ETypeCreate>()\n    {";

        for (int i = 0; i < array.Length; ++i)
        {
            Type type = array[i];

            string typeName = type.Name;

            if (typeName == entityName) continue;

            PropertyInfo[] properties = type.GetProperties(
                BindingFlags.Instance|
                BindingFlags.Public
            );

            list += $"\n        {{\"{typeName}\", Instance{typeName}}},";

            fileText += $"\n    private static Entity Instance{typeName} () => new {typeName}();";
        }

        list+= "\n    };";

        fileText+= $"\n\n {list}";

        fileText+= "\n}";

        File.WriteAllText(currFile, fileText);
    }

    public static Entity GetEntity(String className)
    {
        return  EntityInstancers[className].Invoke();
    }

    //https://stackoverflow.com/questions/26733/getting-all-types-that-implement-an-interface

    private static IEnumerable<Type> GetLoadableTypes(Assembly assembly) {
        if (assembly == null) throw new ArgumentNullException("assembly");
        try {
            return assembly.GetTypes();
        } catch (ReflectionTypeLoadException e) {
            return e.Types.Where(t => t != null);
        }
    }

    private static IEnumerable<Type> GetTypesWithInterface(Assembly asm) {
        var it = typeof (Entity);
        return GetLoadableTypes(asm).Where(it.IsAssignableFrom).ToList();
    }

    //Activation methods!
    private static Entity InstanceDTestLineProvider () => new DTestLineProvider();

    private static Dictionary<String, ETypeCreate> EntityInstancers = new Dictionary<string, ETypeCreate>()
    {
        {"DTestLineProvider", InstanceDTestLineProvider},
    };
}