using System.IO;
using System;
using System.Collections.Generic;

using System.Reflection;

using System.Linq;

using System.Diagnostics;

delegate Entity ETypeCreate (ByteReader reader, int propertyCount);

///<summary>
///This is a class with a list of entity types so the program
///can find wich entity is wich.
///It does this by running the program with the doListing
///initialization argument wich rewrites the code in this
///and closes the program. Then it relaunches again with
///a second request to initialize the program after the first one
///to compile the new code.
public static class EntityTypeList
{
    //https://stackoverflow.com/questions/752/how-to-create-a-new-object-instance-from-a-type

    public static void InstanceEntityType(String typeName, ByteReader reader)
    {
        EntityInstancers[typeName].Invoke(reader, reader.ReadInt32());
    }

    public static void ListAll()
    {
        Stopwatch watch = Stopwatch.StartNew();

        string currFile = Directory.GetCurrentDirectory() + @"\Prefab system\EntityTypeList.cs";

        string fileText = File.ReadAllText(currFile);

        String marker = "//Activation methods!";

        int markerIndex = fileText.LastIndexOf(marker) + marker.Length;

        fileText = fileText.Remove(markerIndex);

        //Console.WriteLine(currFile);

        Type[] array = GetTypesWithInterface(Assembly.GetExecutingAssembly()).ToArray<Type>();

        string entityName = typeof(Entity).Name;

        string list = "   private static GDictionary<String, ETypeCreate> EntityInstancers = new GDictionary<string, ETypeCreate>()\n    {";

        for (int i = 0; i < array.Length; ++i)
        {
            Type type = array[i];

            string typeName = type.Name;

            if (typeName == entityName || type.IsAbstract) continue;

            FieldInfo[] fields = type.GetFields(
                BindingFlags.Instance|
                BindingFlags.Public
            );

            list += $"\n        {{\"{typeName}\", Instance{typeName}}},";

            fileText += $"\n    private static Entity Instance{typeName} (ByteReader reader, int propertyCount)\n    {{\n        {typeName} entity = new {typeName}();";

            fileText += "\n        for(int i =0; i< propertyCount; ++i) { int fieldHashcode = reader.ReadInt32(); switch(fieldHashcode){";

            for(int i2 = 0; i2 < fields.Length; ++i2)
            {
                String valueMethod;

                FieldInfo field = fields[i2];

                if (field.IsInitOnly) continue;

                if (AcceptedTypesOut.TryGetValue(field.GetType(), out valueMethod))
                {
                    fileText+= $"case {field.Name.GetDeterministicHashCode()}: entity.{field.Name} = {valueMethod}; break;";
                }
            }

            PropertyInfo[] properties = type.GetProperties(
                BindingFlags.Instance|
                BindingFlags.Public
            );

            for(int i2 = 0; i2 < properties.Length; ++i2)
            {
                String valueMethod;

                PropertyInfo property = properties[i2];

                if (!property.CanWrite) continue;

                if (AcceptedTypesOut.TryGetValue(property.PropertyType, out valueMethod))
                {
                    fileText+= $"case {property.Name.GetDeterministicHashCode()}: entity.{property.Name} = {valueMethod}; break; ";
                }
            }

            fileText+="}}";

            fileText+= $"\n        return entity;\n    }}";
        }

        list+= "\n    };";

        fileText+= $"\n\n {list}";

        fileText+= "\n}";

        File.WriteAllText(currFile, fileText);

        double timeTook = (double)watch.ElapsedTicks / ((double)Stopwatch.Frequency / 1000.0);

        Console.WriteLine($"Took {timeTook}MS to map entity instancers to EntityTypeList class.");

        watch.Stop();
    }

    ///<summary>
    ///Acceped types of fields that prefab can modify.
    ///Method to extract them by reading bytes from a buffer
    ///must be specified as value.
    ///</summary>
    private static Dictionary<Type, String> AcceptedTypesOut  = new Dictionary<Type, string>()
    {
        {typeof(Int32), "reader.ReadInt32()"},
        {typeof(Int64), "reader.ReadInt64()"},
        {typeof(Single), "reader.ReadFloat()"},
        {typeof(Double), "reader.ReadDouble()"},
        {typeof(Char), "reader.ReadByte()"},
        {typeof(String), "reader.ReadString()"},
        {typeof(Byte), "reader.ReadByte()"},
        {typeof(Boolean), "reader.ReadBool()"},
        {typeof(FInt), "reader.ReadFInt()"},
    };

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

    public static int GetDeterministicHashCode(this string str)
    {
        unchecked
        {
            int hash1 = 5381;
            int hash2 = hash1;

            for(int i = 0; i < str.Length && str[i] != '\0'; i += 2)
            {
                hash1 = ((hash1 << 5) + hash1) ^ str[i];
                if (i == str.Length - 1 || str[i+1] == '\0')
                    break;
                hash2 = ((hash2 << 5) + hash2) ^ str[i+1];
            }

            return hash1 + (hash2*1566083941);
        }
    }

    //Below line marks where the program rewrites the code, DO NOT modify it even a little bit.
    //Activation methods!
    private static Entity InstanceCamera (ByteReader reader, int propertyCount)
    {
        Camera entity = new Camera();
        for(int i =0; i< propertyCount; ++i) { int fieldHashcode = reader.ReadInt32(); switch(fieldHashcode){case 1716406495: entity.IDSet = reader.ReadBool(); break; case -1137859919: entity.ID = reader.ReadInt32(); break; case -811270619: entity.IsMain = reader.ReadBool(); break; case 98611051: entity.CanProcess = reader.ReadBool(); break; case -1877121003: entity.IsDestroyed = reader.ReadBool(); break; case -1976236243: entity.ZValue = reader.ReadInt32(); break; }}
        return entity;
    }
    private static Entity InstanceDTestLineProvider (ByteReader reader, int propertyCount)
    {
        DTestLineProvider entity = new DTestLineProvider();
        for(int i =0; i< propertyCount; ++i) { int fieldHashcode = reader.ReadInt32(); switch(fieldHashcode){case 1716406495: entity.IDSet = reader.ReadBool(); break; case -1137859919: entity.ID = reader.ReadInt32(); break; case -1877121003: entity.IsDestroyed = reader.ReadBool(); break; case 98611051: entity.CanProcess = reader.ReadBool(); break; case -1976236243: entity.ZValue = reader.ReadInt32(); break; case 428904464: entity.IsVisible = reader.ReadBool(); break; }}
        return entity;
    }
    private static Entity InstanceRTestSpriteProvider (ByteReader reader, int propertyCount)
    {
        RTestSpriteProvider entity = new RTestSpriteProvider();
        for(int i =0; i< propertyCount; ++i) { int fieldHashcode = reader.ReadInt32(); switch(fieldHashcode){case -63831078: entity.TexturePath = reader.ReadString(); break; case 1716406495: entity.IDSet = reader.ReadBool(); break; case -1137859919: entity.ID = reader.ReadInt32(); break; case -1877121003: entity.IsDestroyed = reader.ReadBool(); break; case 98611051: entity.CanProcess = reader.ReadBool(); break; case -1976236243: entity.ZValue = reader.ReadInt32(); break; case 428904464: entity.IsVisible = reader.ReadBool(); break; }}
        return entity;
    }
    private static Entity InstanceUITest (ByteReader reader, int propertyCount)
    {
        UITest entity = new UITest();
        for(int i =0; i< propertyCount; ++i) { int fieldHashcode = reader.ReadInt32(); switch(fieldHashcode){case 1716406495: entity.IDSet = reader.ReadBool(); break; case -1137859919: entity.ID = reader.ReadInt32(); break; case -1877121003: entity.IsDestroyed = reader.ReadBool(); break; case 98611051: entity.CanProcess = reader.ReadBool(); break; case -1976236243: entity.ZValue = reader.ReadInt32(); break; case 428904464: entity.IsVisible = reader.ReadBool(); break; }}
        return entity;
    }

    private static GDictionary<String, ETypeCreate> EntityInstancers = new GDictionary<string, ETypeCreate>()
    {
        {"Camera", InstanceCamera},
        {"DTestLineProvider", InstanceDTestLineProvider},
        {"RTestSpriteProvider", InstanceRTestSpriteProvider},
        {"UITest", InstanceUITest},
    };
}