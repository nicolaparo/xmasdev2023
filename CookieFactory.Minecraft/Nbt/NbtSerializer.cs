using CookieFactory.Minecraft.Nbt.Tags;
using System.Text.Json;

namespace CookieFactory.Minecraft.Nbt
{
    public class NbtSerializer
    {
        public static NbtTag Deserialize(string snbt) => SnbtParser.Parse(snbt, false);
        public static object Deserialize(string snbt, Type targetType) => Deserialize(snbt).ToJson().Deserialize(targetType);
        public static T Deserialize<T>(string snbt) => Deserialize(snbt).ToJson().Deserialize<T>();
    }
}
