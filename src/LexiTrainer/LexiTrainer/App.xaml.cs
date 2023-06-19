using Autofac;
using LexiTrainer.Db;
using LexiTrainer.Pages.ViewModels;
using LexiTrainer.Popups.ViewModels;
using LexiTrainer.Services;
using System.Windows;

namespace LexiTrainer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        // TODO: Move strings to the resources.
        // TODO: Add base class for all page view models and use it in IWizard.
        // TODO: Add error handling.
        // TODO: Add unit tests.

        public IContainer AppContainer { get; private set; }

        private void OnStartup(object sender, StartupEventArgs args)
        {
            var builder = new ContainerBuilder();

            RegisterDependencies(builder);

            AppContainer = builder.Build();
        }

        private void OnExit(object sender, ExitEventArgs args)
        {
            AppContainer.Dispose();
        }

        private void RegisterDependencies(ContainerBuilder builder)
        {
            builder.RegisterType<Session>().SingleInstance();
            builder.RegisterType<Wizard>().As<IWizard>().SingleInstance();
            builder.RegisterType<DatabaseProvider>().As<IDatabaseProvider>();
            builder.RegisterType<DictionaryService>().As<IDictionaryService>();

            builder.RegisterType<WordsWindowViewModel>();
            builder.RegisterType<MainViewModel>();
            builder.RegisterType<LoginPageViewModel>();
            builder.RegisterType<RegistrationPageViewModel>();
            builder.RegisterType<LanguageSelectionPageViewModel>();
            builder.RegisterType<DictionarySelectionPageViewModel>();
            builder.RegisterType<TrainingModeSelectionPageViewModel>();
            builder.RegisterType<WordLearningPageViewModel>();
            builder.RegisterType<FinishTrainingPageViewModel>();
            builder.RegisterType<ChangePasswordPageViewModel>();
        }
    }
}
