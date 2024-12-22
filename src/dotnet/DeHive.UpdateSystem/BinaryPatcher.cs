namespace DeHive.UpdateSystem;

using Octodiff.Core;
using Octodiff.Diagnostics;

public class BinaryPatcher
{
    public static int GenerateSignatures(DirectoryInfo sourceDir, DirectoryInfo signatureDir)
    {
        var returnCode = 0;
        foreach (var filePath in Directory.GetFiles(sourceDir.FullName, "*", SearchOption.AllDirectories))
        {
            var relativePath = Path.GetRelativePath(sourceDir.FullName, filePath);
            var signaturePath = Path.Combine(signatureDir.FullName, relativePath + ".sig");

            if (!Directory.Exists(Path.GetDirectoryName(signaturePath)!))
                Directory.CreateDirectory(Path.GetDirectoryName(signaturePath)!);

            using var fileStream = File.OpenRead(filePath);
            using var signatureStream = File.Create(signaturePath);
            try
            {
                var builder = new SignatureBuilder();
                var signatureWriter = new SignatureWriter(signatureStream);
                builder.Build(fileStream, signatureWriter);

                Console.WriteLine($"Signature for '{relativePath}' created.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                returnCode++;
            }
        }

        return returnCode;
    }

    public static int GenerateDeltas(string[] directories)
    {
        var returnCode = 0;
        string sourceDirectory = directories[0],
            signatureDirectory = directories[1],
            deltaDirectory = directories[2];
        foreach (var filePath in Directory.GetFiles(sourceDirectory, "*", SearchOption.AllDirectories))
        {
            var relativePath = Path.GetRelativePath(sourceDirectory, filePath);
            var signaturePath = Path.Combine(signatureDirectory, relativePath + ".sig");
            var deltaPath = Path.Combine(deltaDirectory, relativePath + ".delta");

            if (!File.Exists(signaturePath))
            {
                Console.WriteLine($"Signature '{relativePath}' not found. skip.");
                continue;
            }

            if (!Directory.Exists(Path.GetDirectoryName(deltaPath)!))
                Directory.CreateDirectory(Path.GetDirectoryName(deltaPath)!);

            using var newFileStream = File.OpenRead(filePath);
            using var signatureStream = File.OpenRead(signaturePath);
            using var deltaStream = File.OpenWrite(deltaPath);

            try
            {
                var signatureReader = new SignatureReader(signatureStream, new ConsoleProgressReporter());
                var deltaWriter = new BinaryDeltaWriter(deltaStream);

                var deltaBuilder = new DeltaBuilder();
                deltaBuilder.BuildDelta(newFileStream, signatureReader, deltaWriter);

                Console.WriteLine($"Delta-file '{relativePath}' created.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                returnCode++;
            }
        }

        return returnCode;
    }


    public static int ApplyDeltas(string[] directories)
    {
        var returnCode = 0;
        string basisDirectory = directories[0],
            deltaDirectory = directories[1],
            outputDirectory = directories[2];
        foreach (var deltaFilePath in Directory.GetFiles(deltaDirectory, "*.delta", SearchOption.AllDirectories))
        {
            var relativePath = Path.GetRelativePath(deltaDirectory, deltaFilePath);
            var basisFilePath = Path.Combine(basisDirectory, relativePath.Replace(".delta", ""));
            var outputFilePath = Path.Combine(outputDirectory, relativePath.Replace(".delta", ""));

            if (!File.Exists(basisFilePath))
            {
                Console.WriteLine($"Original file '{relativePath}' not found. skip.");
                File.Delete(outputFilePath);
                File.Delete(deltaFilePath);
                continue;
            }

            if (!Directory.Exists(Path.GetDirectoryName(outputFilePath)!))
                Directory.CreateDirectory(Path.GetDirectoryName(outputFilePath)!);

            using var basisStream = File.OpenRead(basisFilePath);
            using var deltaStream = File.OpenRead(deltaFilePath);
            using var outputStream = File.Create(outputFilePath);

            try
            {
                var deltaReader = new BinaryDeltaReader(deltaStream, new ConsoleProgressReporter());
                var deltaApplier = new DeltaApplier();
                deltaApplier.Apply(basisStream, deltaReader, outputStream);

                Console.WriteLine($"File '{relativePath}' success update.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                returnCode++;
            }
        }

        return returnCode;
    }

}
