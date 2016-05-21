using Bootstrap.Ninject;
using Ninject.Extensions.Conventions;
using Ninject;

namespace MusicSorter
{
    public class NinjectRegistration : INinjectRegistration
    {
        public void Register(IKernel container)
        {
            container.Bind(x => x.FromThisAssembly()
                .SelectAllClasses()
                .BindToFactory()
                .Configure(c => c.InSingletonScope()));
        }
    }
}