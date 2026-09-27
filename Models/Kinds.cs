namespace MediaShrink.Models;

// Screen names for each media kind.
public static class Kinds
{
    public static string Name(MediaKind kind)
    {
        switch (kind)
        {
            case MediaKind.Photo: return "Фото";
            case MediaKind.Audio: return "Аудио";
            case MediaKind.Video: return "Видео";
            default: return "Другое";
        }
    }
}
