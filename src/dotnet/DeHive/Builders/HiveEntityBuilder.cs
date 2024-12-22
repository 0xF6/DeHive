using DeHive.Abstractions.Hives;

namespace DeHive.Creation;

public record HiveEntityBuilder(HiveEntityId entityId, FileInfo filePath);