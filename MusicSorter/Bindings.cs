using MusicSorter.Factories;
using MusicSorter.Factories.Interfaces;
using Ninject.Extensions.Factory;
using Ninject.Modules;

namespace MusicSorter
{
    public class Bindings : NinjectModule
    {
        public override void Load()
        {
            Bind<ISterilizeStringFactory>().To<SterilizeStringFactory>();
            Bind<IEntityIdFactory>().To<EntityIdFactory>();
            Bind<ICreateFolderFactory>().To<CreateFolderFactory>();
        }
    }
}