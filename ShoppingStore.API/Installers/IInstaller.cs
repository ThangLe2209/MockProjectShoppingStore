namespace ShoppingStore.API.Installers
{
    public interface IInstaller
    {
        void InstallService(IServiceCollection services, IConfiguration configuration);
    }
}
