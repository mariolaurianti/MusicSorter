namespace MusicSorter.Factories.Interfaces
{
    public interface ICreateFolderFactory
    {
        void CreateFolder(Song song, string newDestinationPath);
    }
}