using System.IO.MemoryMappedFiles;

namespace Ara3D.IfcParser
{
    public class StepData
    {
        private MemoryMappedFile file;
        private MemoryMappedViewAccessor acc;

        public StepData(string filePath)
        {
            file = MemoryMappedFile.CreateFromFile(filePath, FileMode.Open);
            acc = file.CreateViewAccessor();
        }
    }
}
